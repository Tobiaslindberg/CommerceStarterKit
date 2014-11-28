using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Security;
using System.Workflow.ComponentModel;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Customers.Profile;
using Mediachase.Commerce.Inventory;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Commerce.Marketing.Objects;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Security;

namespace EPiCode.Commerce.Workflow.Activities
{
    /// <summary>
    /// This is an activity that calculates and applies discounts to a particular order group.
    /// This can be used out of the box or as a basis for a different promotion engine.
    /// </summary>
    public partial class CalculateDiscountsActivity : OrderGroupActivityBase
    {
        private static Injected<IWarehouseRepository> WarehouseRepository { get; set; }
        private static Injected<IWarehouseInventoryService> InventoryService { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculateDiscountsActivity"/> class.
        /// </summary>
        public CalculateDiscountsActivity()
			:base()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Executes the specified execution context.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <returns></returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            //try
            //{

            if (OrderGroup is Cart)
            {
                // Validate the properties at runtime
                this.ValidateRuntime();

                // Initialize Marketing Context
                this.InitMarketingContext();

                // Calculate order discounts
                this.CalculateDiscounts();


                // Retun the closed status indicating that this activity is complete.
            }

            return ActivityExecutionStatus.Closed;

            //}
            /*catch (Exception ex)
            {
				Logger.Error(GetType().Name + ": " + ex.Message, ex);
                // An unhandled exception occured.  Throw it back to the WorkflowRuntime.
                throw;
            }*/
        }


        /// <summary>
        /// Inits the marketing context.
        /// </summary>
        private void InitMarketingContext()
        {
            OrderGroup group = this.OrderGroup;
            SetContext(MarketingContext.ContextConstants.ShoppingCart, group);

            // Set customer segment context
            MembershipUser user = SecurityContext.Current.CurrentUser;
            if (user != null)
            {
                CustomerProfileWrapper profile = SecurityContext.Current.CurrentUserProfile as CustomerProfileWrapper;

                if (profile != null)
                {
                    SetContext(MarketingContext.ContextConstants.CustomerProfile, profile);

                    CustomerContact customerContact = CustomerContext.Current.GetContactForUser(user);
                    if ((Guid)customerContact.PrimaryKeyId != group.CustomerId)
                    {
                        customerContact = CustomerContext.Current.GetContactById(group.CustomerId);
                    }
                    if (customerContact != null)
                    {
                        SetContext(MarketingContext.ContextConstants.CustomerContact, customerContact);

                        Guid accountId = (Guid)customerContact.PrimaryKeyId;
                        Guid organizationId = Guid.Empty;
                        if (customerContact.ContactOrganization != null)
                        {
                            organizationId = (Guid)customerContact.ContactOrganization.PrimaryKeyId;
                        }

                        SetContext(MarketingContext.ContextConstants.CustomerSegments, MarketingContext.Current.GetCustomerSegments(accountId, organizationId));
                    }
                }
            }

            // Set customer promotion history context
            SetContext(MarketingContext.ContextConstants.CustomerId, this.OrderGroup.CustomerId);

            // Now load current order usage dto, which will help us determine the usage limits
            // Load existing usage Dto for the current order
            PromotionUsageDto usageDto = PromotionManager.GetPromotionUsageDto(0, Guid.Empty, group.OrderGroupId);
            SetContext(MarketingContext.ContextConstants.PromotionUsage, usageDto);
        }

        /// <summary>
        /// Sets the context.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="val">The val.</param>
        private void SetContext(string key, object val)
        {
            if (!MarketingContext.Current.MarketingProfileContext.ContainsKey(key))
                MarketingContext.Current.MarketingProfileContext.Add(key, val);
            else
                MarketingContext.Current.MarketingProfileContext[key] = val;
        }

        /// <summary>
        /// Calculates the discounts.
        /// </summary>
        private void CalculateDiscounts()
        {
            // Get current context 
            Dictionary<string, object> context = MarketingContext.Current.MarketingProfileContext;

            // Parameter that tells if we need to use cached values for promotions or not
            bool useCache = false;
          
            // Constract the filter, ignore conditions for now
            PromotionFilter filter = new PromotionFilter();
            filter.IgnoreConditions = false;
            filter.IgnorePolicy = false;
            filter.IgnoreSegments = false;
            filter.IncludeCoupons = false;

            // Get property
            OrderGroup order = this.OrderGroup;

            decimal runningTotal = 0;
            foreach (OrderForm orderForm in order.OrderForms)
            {
                runningTotal +=
                    orderForm.LineItems.ToArray().Sum(x => x.Quantity * x.PlacedPrice);
            }

            // Create Promotion Context
            PromotionEntriesSet sourceSet = null;

            // Reuse the same context so we can track exclusivity properly
            PromotionContext promoContext = new PromotionContext(context, new PromotionEntriesSet(), new PromotionEntriesSet());
            promoContext.PromotionResult.RunningTotal = runningTotal;

            #region Determine Line item level discounts
            int totalNumberOfItems = 0;

            // Process line item discounts first
            foreach (OrderForm form in order.OrderForms)
            {
                foreach (OrderFormDiscount discount in form.Discounts)
                {
                    if (!discount.DiscountName.StartsWith("@")/* && discount.DiscountId == -1*/) // ignore custom entries
                        discount.Delete();
                }

                // Create source from current form
                sourceSet = CreateSetFromOrderForm(form);

                // Build dictionary to keep track of entry discount limit
                Dictionary<PromotionDto.PromotionRow, decimal?> entryDiscountApplicationCount = new Dictionary<PromotionDto.PromotionRow, decimal?>();
                PromotionDto promotionDto = PromotionManager.GetPromotionDto(FrameworkContext.Current.CurrentDateTime);
                foreach (PromotionDto.PromotionRow promotion in promotionDto.Promotion)
                {
                    if (!promotion.IsMaxEntryDiscountQuantityNull())
                    {
                        entryDiscountApplicationCount.Add(promotion, promotion.MaxEntryDiscountQuantity);
                    }
                }

                // Now cycle through each line item one by one
                IOrderedEnumerable<LineItem> highPlacedPriceFirst = form.LineItems.ToArray().OrderByDescending(x => x.PlacedPrice);
                int lineItemCount = highPlacedPriceFirst.Count();
                int i = 0;
                foreach (LineItem lineItem in highPlacedPriceFirst)
                {
                    i++;
                    // First remove items
                    foreach (LineItemDiscount discount in lineItem.Discounts)
                    {
                        if (!discount.DiscountName.StartsWith("@")/* && discount.DiscountId == -1*/) // ignore custom entries
                            discount.Delete();
                    }
                    //Exclude gift lineItems from evaluation discounts process
                    if (IsGiftLineItem(lineItem))
                    {
                        continue;
                    }

                    totalNumberOfItems++;

                    // Target only entry promotions
                    PromotionEntriesSet targetSet = new PromotionEntriesSet();
                    targetSet.OrderFormId = form.OrderFormId.ToString();
                    //ET [16.06.2009] If order contains two item with same code, in target hit only first
                    //targetSet.Entries.Add(sourceSet.FindEntryByCode(lineItem.CatalogEntryId));
                    targetSet.Entries.Add(CreatePromotionEntryFromLineItem(lineItem));

                    promoContext.SourceEntriesSet = sourceSet;
                    promoContext.TargetEntriesSet = targetSet;

                    promoContext.TargetGroup = PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Entry).Key;

                    // Evaluate conditions
                    bool checkEntryLevelDiscountLimit = i == lineItemCount;
                    MarketingContext.Current.EvaluatePromotions(useCache, promoContext, filter, entryDiscountApplicationCount, checkEntryLevelDiscountLimit);
                    // from now on use cache
                    useCache = true;
                }
            }
            #endregion

            #region Determine Order level discounts
            foreach (OrderForm form in order.OrderForms)
            {
                // Now process global order discounts
                // Now start processing it
                // Create source from current form
                sourceSet = CreateSetFromOrderForm(form);
                promoContext.SourceEntriesSet = sourceSet;
                promoContext.TargetEntriesSet = sourceSet;
                promoContext.TargetGroup = PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Order).Key;
            }

            // Evaluate conditions
            MarketingContext.Current.EvaluatePromotions(useCache, promoContext, filter, null, false);
            //Removing now not aplyied Gift discounts from Order 
            RemoveGiftPromotionFromOrder(order, promoContext);

            #endregion

            #region Determine Shipping Discounts
            foreach (OrderForm form in order.OrderForms)
            {
                foreach (Shipment shipment in form.Shipments)
                {
                    // Remove old discounts if any
                    foreach (ShipmentDiscount discount in shipment.Discounts)
                    {
                        if (!discount.DiscountName.StartsWith("@")/* && discount.DiscountId == -1*/) // ignore custom entries
                            discount.Delete();
                    }

                    // Create content for current shipment
                    /*
                    sourceSet = CreateSetFromOrderForm(form);                    
                    promoContext.SourceEntriesSet.Entries = sourceSet.Entries;
                     * */
                    PromotionEntriesSet targetSet = CreateSetFromShipment(shipment);
                    promoContext.SourceEntriesSet = targetSet;
                    promoContext.TargetEntriesSet = targetSet;
                    promoContext.TargetGroup = PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Shipping).Key;

                    // Evaluate promotions
                    MarketingContext.Current.EvaluatePromotions(useCache, promoContext, filter, null, false);

                    // Set the total discount for the shipment
                    // shipment.ShippingDiscountAmount = GetDiscountPrice(order, promoContext.PromotionResult);
                }
            }

            #endregion




            #region Start Applying Discounts         
            foreach (PromotionItemRecord itemRecord in promoContext.PromotionResult.PromotionRecords)
            {
                if (itemRecord.Status != PromotionItemRecordStatus.Commited)
                    continue;

                // Pre process item record
                PreProcessItemRecord(order, itemRecord);

                // Applies discount and adjusts the running total
                if (itemRecord.AffectedEntriesSet.Entries.Count > 0)
                    runningTotal -= ApplyItemDiscount(order, itemRecord, runningTotal);
            }
            #endregion

            #region True up order level discounts (from Mark's fix for Teleflora)
            decimal orderLevelAmount = 0;
            decimal lineItemOrderLevelTotal = 0;
            foreach (OrderForm form in order.OrderForms)
            {
                orderLevelAmount += form.Discounts.Cast<OrderFormDiscount>().Where(y => !y.DiscountName.StartsWith("@")).Sum(x => x.DiscountValue);
                lineItemOrderLevelTotal += form.LineItems.ToArray().Sum(x => x.OrderLevelDiscountAmount);
                if (orderLevelAmount > lineItemOrderLevelTotal)
                {
                    form.LineItems[0].OrderLevelDiscountAmount += orderLevelAmount - lineItemOrderLevelTotal;
                }
            }
            #endregion
        }

        /// <summary>
        /// Pre processes item record adding additional LineItems if needed.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="record">The record.</param>
        private void PreProcessItemRecord(OrderGroup order, PromotionItemRecord record)
        {
            // We do special logic for the gift promotion reward
            if (record.PromotionReward is GiftPromotionReward)
            {
                // Check if item already in the cart, if not add
                if (((GiftPromotionReward)record.PromotionReward).AddStrategy == GiftPromotionReward.Strategy.AddWhenNeeded)
                {
                    // We assume that all affected entries are the gifts that need to be added to the cart
                    foreach (PromotionEntry entry in record.AffectedEntriesSet.Entries)
                    {
                        LineItem giftLineItem = FindGiftLineItemInOrder(order, entry.CatalogEntryCode, record);

                        if (!IsOrderHaveSpecifiedGiftPromotion(order, record))
                        {
                            // Didn't find, add it
                            if (giftLineItem == null)
                            {
                                // we should some kind of delegate or common implementation here so we can use the same function in both discount and front end
                                Entry catEntry = CatalogContext.Current.GetCatalogEntry(entry.CatalogEntryCode);
                                giftLineItem = AddNewGiftLineItemToOrder(order, catEntry, entry.Quantity);
                                AddGiftItemToAShipment(giftLineItem);
                                CatalogEntryResponseGroup responseGroup = new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull);
                                CatalogEntryDto entryDto = CatalogContext.Current.GetCatalogEntryDto(giftLineItem.CatalogEntryId, responseGroup);
                                CatalogEntryDto.CatalogEntryRow entryRow = entryDto.CatalogEntry[0];
                                Money? price = GetItemPrice(entryRow, giftLineItem, CustomerContext.Current.CurrentContact);
                                giftLineItem.ListPrice = price.HasValue ? price.Value.Amount : 0m;
                                giftLineItem.PlacedPrice = giftLineItem.ListPrice;
                                // populate inventory information for giftLineItem
                                IWarehouseInventory aggregateInventory = ServiceLocator.Current.GetInstance<IWarehouseInventoryService>().GetTotal(new CatalogKey(entryRow));
                                PopulateInventoryInfo(aggregateInventory, giftLineItem);                                                                
                            }
                            else
                            {
                                giftLineItem.Quantity = Math.Max(entry.Quantity, giftLineItem.Quantity);
                            }
                        }
                        else
                        {
                            entry.Quantity = giftLineItem != null ? Math.Min(entry.Quantity, giftLineItem.Quantity) : entry.Quantity;
                        }
                        entry.Owner = giftLineItem;
                        entry.CostPerEntry = giftLineItem != null ? giftLineItem.ListPrice : 0m;
                    }
                }
            }
        }
           
        private void AddGiftItemToAShipment(LineItem giftLineItem)
        {
            if (giftLineItem.Parent.Shipments.Count > 0)
            {
                giftLineItem.Parent.Shipments[0].AddLineItemIndex(giftLineItem.Parent.LineItems.Count - 1, giftLineItem.Quantity);
            }
        }

        private LineItem AddNewGiftLineItemToOrder(OrderGroup order, Entry entry, decimal quantity)
        {
            LineItem lineItem = order.OrderForms[0].LineItems.AddNew();

            // If entry has a parent, add parents name
            if (entry.ParentEntry != null)
            {
                lineItem.DisplayName = String.Format("{0}: {1}", entry.ParentEntry.Name, entry.Name);
                lineItem.ParentCatalogEntryId = entry.ParentEntry.ID;
            }
            else
            {
                lineItem.DisplayName = entry.Name;
                lineItem.ParentCatalogEntryId = String.Empty;
            }

            lineItem.CatalogEntryId = entry.ID;
            //Price price = StoreHelper.GetSalePrice(entry, quantity);
            //entry.ItemAttributes always null
            //lineItem.ListPrice = entry.ItemAttributes.ListPrice.Amount;
            lineItem.MaxQuantity = entry.ItemAttributes.MaxQuantity;
            lineItem.MinQuantity = entry.ItemAttributes.MinQuantity;
            lineItem.Quantity = quantity;
            
            // try to add warehouseCode from shipment record or other line items
            string warehouseCode = string.Empty;
            if (order.OrderForms[0].Shipments.Count > 0)
            {
                warehouseCode = order.OrderForms[0].Shipments[0].WarehouseCode;
            }
            // Add warehouse from existing items in case not specified for gift item
            if (String.IsNullOrEmpty(lineItem.WarehouseCode))
            {
                warehouseCode = order.OrderForms[0].LineItems[0].WarehouseCode;
            }
            if (!String.IsNullOrEmpty(warehouseCode))
            {
                lineItem.WarehouseCode = warehouseCode;
            }
            lineItem.InventoryStatus = (int)GetInventoryStatus(entry, warehouseCode);

            return lineItem;
        }

        private InventoryTrackingStatus GetInventoryStatus(Entry entry, string warehouseCode)
        {
            var warehouse = WarehouseRepository.Service.Get(warehouseCode);
            if (warehouse != null)
            {
                var inventory = InventoryService.Service.Get(new CatalogKey(entry), warehouse);
                if (inventory != null)
                {
                    return inventory.InventoryStatus;
                }
            }

            return entry.InventoryStatus;
        }

        /// <summary>
        /// Applies the item discount.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="record">The record.</param>
        /// <param name="totalAmount">The total amount.</param>
        /// <returns></returns>
        private decimal ApplyItemDiscount(OrderGroup order, PromotionItemRecord record, decimal totalAmount)
        {

            decimal discountAmount = 0;
            if (record.PromotionReward.RewardType == PromotionRewardType.AllAffectedEntries)
            {
                if (record.PromotionReward.AmountType == PromotionRewardAmountType.Percentage)
                {
                    discountAmount = record.AffectedEntriesSet.TotalCost * record.PromotionReward.AmountOff / 100;
                    decimal averageDiscountAmount = discountAmount / record.AffectedEntriesSet.TotalQuantity;
                    foreach (PromotionEntry entry in record.AffectedEntriesSet.Entries)
                    {
                        // Sasha: changed back, CostPerEntry does not change dynamically, while total cost does
                        // AddDiscountToLineItem(order, record, entry, entry.CostPerEntry * entry.Quantity * record.PromotionReward.AmountOff / 100m, 0);
                        // AddDiscountToLineItem(order, record, entry.CatalogEntryCode, averageDiscountAmount * entry.Quantity, 0);
                        AddDiscountToLineItem(order, record, entry, averageDiscountAmount * entry.Quantity, 0);
                    }
                }
                else // need to split discount between all items
                {
                    discountAmount = record.PromotionReward.AmountOff;
                    decimal averageDiscountAmount = record.PromotionReward.AmountOff / record.AffectedEntriesSet.TotalQuantity;
                    foreach (PromotionEntry entry in record.AffectedEntriesSet.Entries)
                    {
                        AddDiscountToLineItem(order, record, entry, averageDiscountAmount * entry.Quantity, 0);
                    }
                }
            }
            else if (record.PromotionReward.RewardType == PromotionRewardType.EachAffectedEntry)
            {
                if (record.PromotionReward.AmountType == PromotionRewardAmountType.Percentage)
                {
                    discountAmount = record.AffectedEntriesSet.TotalCost * record.PromotionReward.AmountOff / 100;
                    foreach (PromotionEntry entry in record.AffectedEntriesSet.Entries)
                    {
                        AddDiscountToLineItem(order, record, entry, entry.CostPerEntry * entry.Quantity * record.PromotionReward.AmountOff / 100, 0);
                    }
                }
                else
                {
                    discountAmount = record.AffectedEntriesSet.TotalQuantity * record.PromotionReward.AmountOff;
                    foreach (PromotionEntry entry in record.AffectedEntriesSet.Entries)
                    {
                        AddDiscountToLineItem(order, record, entry, record.PromotionReward.AmountOff * entry.Quantity, 0);
                    }
                }
            }
            else if (record.PromotionReward.RewardType == PromotionRewardType.WholeOrder)
            {
                decimal percentageOffTotal = 0;
                if (record.PromotionReward.AmountType == PromotionRewardAmountType.Percentage)
                {
                    // calculate percentage adjusted by the running amount, so it will be a little less if running amount is less than total
                    percentageOffTotal = (record.PromotionReward.AmountOff / 100) * (totalAmount / record.AffectedEntriesSet.TotalCost);
                    //percentageOffTotal = PromotionReward.AmountOff / 100;
                    discountAmount = totalAmount * record.PromotionReward.AmountOff / 100;
                }
                else
                {
                    // Calculate percentage off discount price
                    percentageOffTotal = record.PromotionReward.AmountOff / totalAmount;

                    // but since CostPerEntry is not an adjusted price, we need to take into account additional discounts already applied
                    percentageOffTotal = percentageOffTotal * (totalAmount / record.AffectedEntriesSet.TotalCost);

                    discountAmount = record.PromotionReward.AmountOff;
                }

                // Now distribute discount amount evenly over all entries taking into account running total
                // Special case for shipments, we consider WholeOrder to be a shipment
                if (!record.PromotionItem.DataRow.PromotionGroup.Equals(PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Shipping).Key, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (PromotionEntry entry in record.AffectedEntriesSet.Entries)
                    {
                        AddDiscountToLineItem(order, record, entry, 0, (((entry.CostPerEntry * entry.Quantity)/* - entry.Discount*/)) * percentageOffTotal);
                    }
                }
            }

            // Save discounts
            if (record.PromotionItem.DataRow.PromotionGroup.Equals(PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Order).Key, StringComparison.OrdinalIgnoreCase)
                || record.PromotionReward is GiftPromotionReward)
            {
                if (record.PromotionReward.RewardType == PromotionRewardType.WholeOrder)
                {
                    OrderFormDiscount discount = FindOrderFormDiscountById(order, record.PromotionItem.DataRow.PromotionId, Int32.Parse(record.AffectedEntriesSet.OrderFormId));
                    bool hasOrderFormDiscount = true;
                    if (discount == null)
                    {
                        discount = new OrderFormDiscount();
                        hasOrderFormDiscount = false;
                    }

                    var discountName = record.PromotionItem.DataRow.Name;
                    if (record.PromotionReward is GiftPromotionReward)
                    {
                        discountName = GetGiftPromotionName(record);
                    }
                    discount.DiscountName = discountName;

                    discount.DiscountAmount = record.PromotionReward.AmountOff;
                    discount.DiscountCode = record.PromotionItem.DataRow.CouponCode;
                    discount.DiscountValue = hasOrderFormDiscount ? discountAmount + discount.DiscountValue : discountAmount;
                    discount.DisplayMessage = GetDisplayName(record.PromotionItem.DataRow, Thread.CurrentThread.CurrentCulture.Name);
                    discount.OrderFormId = Int32.Parse(record.AffectedEntriesSet.OrderFormId);
                    discount.DiscountId = record.PromotionItem.DataRow.PromotionId;

                    foreach (OrderForm form in order.OrderForms)
                    {
                        if (form.OrderFormId == discount.OrderFormId && !hasOrderFormDiscount)
                            form.Discounts.Add(discount);
                    }
                }
            }
            else if (record.PromotionItem.DataRow.PromotionGroup.Equals(PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Shipping).Key, StringComparison.OrdinalIgnoreCase))
            {
                ShipmentDiscount discount = FindShipmentDiscountById(order, record.PromotionItem.DataRow.PromotionId, Int32.Parse(record.AffectedEntriesSet.ShipmentId));

                if (discount == null)
                    discount = new ShipmentDiscount();

                discount.DiscountAmount = record.PromotionReward.AmountOff;
                discount.DiscountCode = record.PromotionItem.DataRow.CouponCode;
                discount.DiscountName = record.PromotionItem.DataRow.Name;
                discount.DisplayMessage = GetDisplayName(record.PromotionItem.DataRow, Thread.CurrentThread.CurrentCulture.Name);
                discount.ShipmentId = Int32.Parse(record.AffectedEntriesSet.ShipmentId);
                discount.DiscountId = record.PromotionItem.DataRow.PromotionId;

                foreach (OrderForm form in order.OrderForms)
                {
                    foreach (Shipment shipment in form.Shipments)
                    {
                        if (shipment.ShipmentId == discount.ShipmentId)
                        {
                            shipment.Discounts.Add(discount);

                            if (record.PromotionReward.AmountType == PromotionRewardAmountType.Percentage)
                            {
                                discountAmount = shipment.ShipmentTotal * record.PromotionReward.AmountOff / 100;
                            }
                            else
                            {
                                discountAmount = Math.Min(record.PromotionReward.AmountOff, shipment.ShipmentTotal);
                            }

                            shipment.ShippingDiscountAmount += discountAmount;
                            // ShippingDiscountAmount will not be subtracted from the ShipmentTotal per discussions on 2/22/2012.
                            break;
                        }
                    }
                }
                discount.DiscountValue = discountAmount;
            }
            return discountAmount;
        }

        private void AddDiscountToLineItem(OrderGroup order, PromotionItemRecord itemRecord, PromotionEntry promotionEntry,
                                          decimal itemDiscount, decimal orderLevelDiscount)
        {
            orderLevelDiscount = Math.Floor(orderLevelDiscount * 100) * 0.01m;
            LineItem item = FindLineItemByPromotionEntry(order, promotionEntry);
            if (item != null)
            {
                //reset gift line item discount
                if (IsGiftLineItem(item))
                {
                    item.PlacedPrice = promotionEntry.CostPerEntry;
                    item.LineItemDiscountAmount = itemDiscount;
                    item.OrderLevelDiscountAmount = 0;
                    item.ExtendedPrice = item.PlacedPrice;
                }
                else
                {
                    // Add line item properties
                    item.LineItemDiscountAmount += itemDiscount;
                    item.OrderLevelDiscountAmount += orderLevelDiscount;
                    item.ExtendedPrice = item.ListPrice * item.Quantity - item.LineItemDiscountAmount - item.OrderLevelDiscountAmount;
                }

                if (itemRecord.PromotionItem.DataRow.PromotionGroup.Equals(PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Entry).Key, StringComparison.OrdinalIgnoreCase)
                    || itemRecord.PromotionReward is GiftPromotionReward
                    || (itemRecord.PromotionItem.DataRow.PromotionGroup.Equals(PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Order).Key,
                        StringComparison.OrdinalIgnoreCase) && itemRecord.PromotionReward.RewardType == PromotionRewardType.EachAffectedEntry))
                {
                    LineItemDiscount discount = FindLineItemDiscountById(order, itemRecord.PromotionItem.DataRow.PromotionId, item.LineItemId);

                    if (discount == null)
                    {
                        discount = new LineItemDiscount();
                        item.Discounts.Add(discount);
                    }

                    var discountName = itemRecord.PromotionItem.DataRow.Name;
                    if (itemRecord.PromotionReward is GiftPromotionReward)
                    {
                        discount.DiscountName = GetGiftPromotionName(itemRecord);
                    }
                    else
                    {
                        discount.DiscountName = String.Format("{0}{1}", itemRecord.PromotionItem.DataRow.Name,
                            itemRecord.PromotionItem.DataRow.OfferType == 1 ? ":PercentageBased" : ":ValueBased");
                    }

                    discount.DiscountAmount = itemRecord.PromotionReward.AmountOff;
                    discount.DiscountCode = itemRecord.PromotionItem.DataRow.CouponCode;

                    discount.DiscountValue = itemDiscount;
                    // use the promotion name if the localized display message is null or empty
                    discount.DisplayMessage = GetDisplayName(itemRecord.PromotionItem.DataRow, Thread.CurrentThread.CurrentCulture.Name);
                    if (string.IsNullOrEmpty(discount.DisplayMessage))
                        discount.DisplayMessage = itemRecord.PromotionItem.DataRow.Name;
                    discount.LineItemId = item.LineItemId;
                    discount.DiscountId = itemRecord.PromotionItem.DataRow.PromotionId;
                }
            }
        }

        private LineItem FindGiftLineItemInOrder(OrderGroup order, string catalogEntryId, PromotionItemRecord promoRecord)
        {
            var lineItems = order.OrderForms[0].LineItems.ToArray().Where(x => x.CatalogEntryId == catalogEntryId);
            foreach (var lineitem in lineItems)
            {
                foreach (LineItemDiscount discount in lineitem.Discounts)
                {
                    if (discount.DiscountName == GetGiftPromotionName(promoRecord))
                    {
                        return lineitem;
                    }
                }
            }
            return null;
        }


        private bool IsOrderHaveSpecifiedGiftPromotion(OrderGroup order, PromotionItemRecord promoRecord)
        {
            bool retVal = false;
            foreach (OrderFormDiscount discount in order.OrderForms[0].Discounts)
            {
                if (GetGiftPromotionName(promoRecord) == discount.DiscountName)
                {
                    retVal = true;
                    break;
                }
            }
            return retVal;
        }

        private static LineItem FindLineItemByPromotionEntry(OrderGroup order, PromotionEntry prmotionEntry)
        {
            LineItem retVal = null;

            foreach (OrderForm form in order.OrderForms)
            {
                foreach (LineItem item in form.LineItems)
                {
                    if (item == prmotionEntry.Owner)
                    {
                        retVal = item;
                        break;
                    }
                }

                if (retVal != null)
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// Finds the order form discount by id.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="promotionId">The promotion id.</param>
        /// <param name="orderFormId">The order form id.</param>
        /// <returns></returns>
        private OrderFormDiscount FindOrderFormDiscountById(OrderGroup order, int promotionId, int orderFormId)
        {
            foreach (OrderForm form in order.OrderForms)
            {
                if (form.OrderFormId == orderFormId)
                {
                    foreach (OrderFormDiscount discount in form.Discounts)
                    {
                        if (discount.DiscountId == promotionId)
                            return discount;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the shipment discount by id.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="promotionId">The promotion id.</param>
        /// <param name="shipmentId">The shipment id.</param>
        /// <returns></returns>
        private ShipmentDiscount FindShipmentDiscountById(OrderGroup order, int promotionId, int shipmentId)
        {
            
            foreach (OrderForm form in order.OrderForms)
            {
                foreach (Shipment shipment in form.Shipments)
                {
                    if (shipment.ShipmentId == shipmentId)
                    {
                        foreach (ShipmentDiscount discount in shipment.Discounts)
                        {
                            if (discount.DiscountId == promotionId)
                                return discount;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the line item discount by id.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="promotionId">The promotion id.</param>
        /// <param name="lineItemId">The line item id.</param>
        /// <returns></returns>
        private LineItemDiscount FindLineItemDiscountById(OrderGroup order, int promotionId, int lineItemId)
        {
            foreach (OrderForm form in order.OrderForms)
            {
                foreach (LineItem lineItem in form.LineItems)
                {
                    foreach (LineItemDiscount discount in lineItem.Discounts)
                    {
                        if (discount.DiscountId == promotionId && discount.LineItemId == lineItemId && discount.ObjectState != Mediachase.MetaDataPlus.MetaObjectState.Deleted)
                            return discount;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="languageCode">The language code.</param>
        /// <returns></returns>
        private string GetDisplayName(PromotionDto.PromotionRow row, string languageCode)
        {
            PromotionDto.PromotionLanguageRow[] langRows = row.GetPromotionLanguageRows();
            if (langRows != null && langRows.Length > 0)
            {
                foreach (PromotionDto.PromotionLanguageRow lang in langRows)
                {
                    if (lang.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase))
                    {
                        return lang.DisplayName;
                    }
                }
            }

            return row.Name;
        }

        /// <summary>
        /// Creates the set from order form.
        /// </summary>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        private PromotionEntriesSet CreateSetFromOrderForm(OrderForm form)
        {
            PromotionEntriesSet set = new PromotionEntriesSet();
            set.OrderFormId = form.OrderFormId.ToString();

            IOrderedEnumerable<LineItem> lineItemByPrice = form.LineItems.ToArray().Where(x => !IsGiftLineItem(x)).OrderByDescending(x => x.PlacedPrice);

            foreach (LineItem lineItem in lineItemByPrice)
            {
                set.Entries.Add(CreatePromotionEntryFromLineItem(lineItem));
            }

            return set;
        }

        /// <summary>
        /// Creates the set from shipment.
        /// </summary>
        /// <param name="shipment">The shipment.</param>
        /// <returns></returns>
        private PromotionEntriesSet CreateSetFromShipment(Shipment shipment)
        {
            PromotionEntriesSet set = new PromotionEntriesSet();
            set.ShipmentId = shipment.ShipmentId.ToString();
            set.OrderFormId = shipment.Parent.OrderFormId.ToString();
            foreach (string lineItemIndex in shipment.LineItemIndexes)
            {
                LineItem lineItem = shipment.Parent.LineItems[Int32.Parse(lineItemIndex)];

                if (lineItem != null && !IsGiftLineItem(lineItem))
                {
                    PromotionEntry entry = CreatePromotionEntryFromLineItem(lineItem);
                    set.Entries.Add(entry);
                }
            }

            return set;
        }

        /// <summary>
        /// Creates the promotion entry from line item.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <returns></returns>
        private PromotionEntry CreatePromotionEntryFromLineItem(LineItem lineItem)
        {
            string catalogNodes = String.Empty;
            string catalogs = String.Empty;
            string catalogName = lineItem.Catalog;
            string catalogNodeCode = lineItem.CatalogNode;
            // Now cycle through all the catalog nodes where this entry is present filtering by specified catalog and node code
            // The nodes are only populated when Full or Nodes response group is specified.

            // Request full response group so we can reuse the same cached item
            Entry entry = CatalogContext.Current.GetCatalogEntry(lineItem.CatalogEntryId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));

            if (entry != null && entry.Nodes != null && entry.Nodes.CatalogNode != null && entry.Nodes.CatalogNode.Length > 0)
            {
                foreach (CatalogNode node in entry.Nodes.CatalogNode)
                {
                    string entryCatalogName = CatalogContext.Current.GetCatalogDto(node.CatalogId).Catalog[0].Name;

                    // Skip filtered catalogs
                    if (!String.IsNullOrEmpty(catalogName) && !entryCatalogName.Equals(catalogName))
                        continue;

                    // Skip filtered catalogs nodes
                    if (!String.IsNullOrEmpty(catalogNodeCode) && !node.ID.Equals(catalogNodeCode, StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (String.IsNullOrEmpty(catalogs))
                        catalogs = entryCatalogName;
                    else
                        catalogs += ";" + entryCatalogName;

                    if (String.IsNullOrEmpty(catalogNodes))
                        catalogNodes = node.ID;
                    else
                        catalogNodes += ";" + node.ID;
                }
            }

            PromotionEntry result = new PromotionEntry(catalogs, catalogNodes, lineItem.CatalogEntryId, lineItem.ListPrice);
            var promotionEntryPopulateService = (IPromotionEntryPopulate)MarketingContext.Current.PromotionEntryPopulateFunctionClassInfo.CreateInstance();
            promotionEntryPopulateService.Populate(result, lineItem);

            return result;
        }

        /// <summary>
        /// Removing now not aplyied Gift discounts from Order 
        /// </summary>
        private void RemoveGiftPromotionFromOrder(OrderGroup order, PromotionContext promoContext)
        {
            var notApliedOldGiftDiscounts = new List<OrderFormDiscount>();

            if (0 < order.OrderForms.Count)
            {
                foreach (OrderFormDiscount discount in order.OrderForms[0].Discounts)
                {
                    var promoRecord = promoContext.PromotionResult.PromotionRecords.FirstOrDefault(x => GetGiftPromotionName(x) == discount.DiscountName);
                    if (promoRecord == null)
                    {
                        notApliedOldGiftDiscounts.Add(discount);
                    }
                }
            }

            foreach (OrderFormDiscount toRemoveDiscount in notApliedOldGiftDiscounts)
            {
                toRemoveDiscount.Delete();

                if (0 < order.OrderForms.Count)
                {
                    //remove Gift items from order
                    var lineitems = order.OrderForms[0].LineItems.ToArray();
                    foreach (LineItem lineItem in lineitems)
                    {
                        foreach (LineItemDiscount lineItemDiscount in lineItem.Discounts)
                        {
                            if (lineItemDiscount.DiscountName == toRemoveDiscount.DiscountName)
                            {
                                lineItem.Delete();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether [is gift line item] [the specified line item].
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <returns>
        /// 	<c>true</c> if [is gift line item] [the specified line item]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsGiftLineItem(LineItem lineItem)
        {
            bool retVal = false;
            foreach (LineItemDiscount discount in lineItem.Discounts)
            {
                if (discount.DiscountName.EndsWith(":Gift"))
                {
                    retVal = true;
                    break;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Gets the name of the gift promotion.
        /// </summary>
        /// <param name="promoRecord">The promo record.</param>
        /// <returns></returns>
        public string GetGiftPromotionName(PromotionItemRecord promoRecord)
        {
            return "@" + promoRecord.PromotionItem.DataRow.Name + ":Gift";
        }
    }
}
