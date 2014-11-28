using System;
using System.Linq;
using System.Workflow.ComponentModel;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Inventory;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;
using System.Diagnostics;

namespace EPiCode.Commerce.Workflow.Activities
{
    public partial class GetFulfillmentWarehouseActivity : HandoffActivityBase
    {
        private static Injected<IWarehouseRepository> WarehouseRepository { get; set; }
        private static Injected<IWarehouseInventoryService> InventoryService { get; set; }
        private enum CheckInventoryMode { Ignore, Check }
        private static readonly CatalogEntryResponseGroup CatalogEntryResponseGroup_InfoWithVariations = new CatalogEntryResponseGroup(
            CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo |
            CatalogEntryResponseGroup.ResponseGroup.Variations
            );

        public GetFulfillmentWarehouseActivity()
			:base()
        {
            InitializeComponent();
        }

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
			try
			{
				// Validate the properties at runtime
				this.ValidateRuntime();
				this.GetFulfillmentWarehouse();
				return ActivityExecutionStatus.Closed;
			}
			catch (Exception ex)
			{
				Logger.Error(GetType().Name + ": " + ex.Message, ex);
				throw;
			}
        }

        private void GetFulfillmentWarehouse()
        {
            foreach (OrderForm form in this.OrderGroup.OrderForms)
            {
                foreach (LineItem lineItem in form.LineItems)
                {
                    var warehouseCode = GetFulfillmentWarehouseForLineItem(this.OrderGroup, lineItem);
                    if (CatalogContext.Current.GetCatalogEntryDto(lineItem.CatalogEntryId, CatalogEntryResponseGroup_InfoWithVariations) != null &&
                        !string.IsNullOrEmpty(warehouseCode))
                    {
                        lineItem.WarehouseCode = warehouseCode;
                        // Note: LineItem's CatalogEntryId is really the Code, not the ID
                        lineItem.InventoryStatus = (int)GetInventoryStatus(lineItem.CatalogEntryId, warehouseCode);
                    }
                    else
                    {
                        form.RemoveLineItemFromShipments(lineItem);
                        lineItem.Delete();
                    }
                }
            }
        }

        /// <summary>
        /// Validates the current warehouse selection for a line item or assigns and returns the appropriate warehouse if needed
        /// </summary>
        /// <param name="orderGroup">The order group that contains the line item.</param>
        /// <param name="lineItem">The item being purchased.</param>
        /// <returns>The Code of the warehouse that should provide the line item, or the empty string if no warehouse can supply the item</returns>
        /// <remarks>
        /// This is the main processing step for the fulfillment process.  Customer implementations that wish to override the fulfillment process
        /// with custom logic (e.g. to enable true multi-warehouse fulfillment) should start by looking here.
        /// IsValidPickupByShipment() and CanBeValidPickupByLineItem() are necessary to support the in-store pickup feature and generally will not
        /// require changes, but please verify that for your implementation.  IsValidPickupByShipment() checks line items that already have a shipment
        /// set, and CanBeValidPickupByLineItem() validates that an order with no shipment can be fulfilled as a pickup.  CanBeValidPickupByLineItem() 
        /// can generate false positives so it may need a closer look.
        /// IsValidFulfillment() is a straightforward check to validate that a (non-pickup) order with a warehouse already assigned can actually 
        /// provide the goods.  The only tricky point is that it does check that the line item is not configured for pickup, and that may not be 
        /// appropriate in some custom setups.
        /// TryDetermineFulfillmentWarehouse() is the holder for the actual warehouse assignment when it has not been assigned already (by the UI or
        /// in an earlier pass) and will hold the key logic for any custom fulfillment implementation.
        /// </remarks>
        private string GetFulfillmentWarehouseForLineItem(OrderGroup orderGroup, LineItem lineItem)
        {
            var returnCode = String.Empty;

            var catalogKey = new Mediachase.Commerce.Catalog.CatalogKey(lineItem.Parent.Parent.ApplicationId, lineItem.CatalogEntryId);
            IWarehouse warehouse = null;

            if (!String.IsNullOrEmpty(lineItem.WarehouseCode))
            {
                // If a warehouse is already assigned, still need to validate whether it can fulfill the order (e.g. activity, fulfillment/pickup) or not
                // Allowing the logic in CheckInventoryActivity deal with the inventory issues, as otherwise the double evaluation can lead to larger order amounts being rejected
                warehouse = WarehouseRepository.Service.Get(lineItem.WarehouseCode);
                if (IsValidPickupFromShipment(orderGroup, lineItem, CheckInventoryMode.Ignore) ||
                    IsValidPickupFromLineItem(orderGroup, lineItem, CheckInventoryMode.Ignore) ||
                    IsValidFulfillment(warehouse, lineItem, CheckInventoryMode.Ignore))
                {
                    return warehouse.Code;
                }
                // Redundant to the inventory amount that will be shown too, but provides useful detail
                Warnings.Add("LineItemUnfulfilled-" + lineItem.Id.ToString(), String.Format("Item \"{0}\" cannot be added to the cart because the assigned warehouse cannot fulfill or provide pickup services for the request.", lineItem.DisplayName));
                return string.Empty;
            }

            warehouse = GetValidPickupFromShipment(orderGroup, lineItem, CheckInventoryMode.Check);
            if (warehouse != null)
            {
                return warehouse.Code;
            }
            warehouse = GetFulfillmentWarehouse(lineItem, checkInventory: CheckInventoryMode.Check);
            if (warehouse != null)
            {
                return warehouse.Code;
            }
            warehouse = GetFulfillmentWarehouse(lineItem, CheckInventoryMode.Ignore);
            if (warehouse != null)
            {
                return warehouse.Code; //TODO: Validate - effectively gets overridden in CheckInventoryActivity, but lets us set an error for the bad config
            }

            // No fallback assignment, this should be an error case not a best guess
            Warnings.Add("LineItemUnfulfilled-" + lineItem.Id.ToString(), String.Format("Item \"{0}\" cannot be added to the cart because of a configuration error.", lineItem.DisplayName));

            return string.Empty;
        }

        /// <summary>
        /// Validates that an order line item has a valid in-store pickup set as its shipment method
        /// </summary>
        /// <param name="orderGroup">The order group parent for the line item.</param>
        /// <param name="lineItem">The item being processed.</param>
        /// <param name="checkInventory">Set to false to override the check against current stock.</param>
        /// <returns>true if the shipment is set and is a valid pickup; otherwise false.</returns>
        /// <remarks>
        /// In the standard workflow process the inventory is checked in a later step by the CheckInventoryActivity, so it may be 
        /// useful to override the inventory processing here.  It also allows us to distinguish between insufficient inventory and 
        /// an error in the warehouse configuration.
        /// </remarks>
        private bool IsValidPickupFromShipment(OrderGroup orderGroup, LineItem lineItem, CheckInventoryMode checkInventory)
        {
            return (GetValidPickupFromShipment(orderGroup, lineItem, checkInventory) != null);
        }

        /// <summary>
        /// Matches the shipment information attached to an order line item to against the pickup warehouses to find the source warehouse, or 
        /// validates the assigned warehouse if one is set already
        /// </summary>
        /// <param name="orderGroup">The order group parent for the line item.</param>
        /// <param name="lineItem">The item being processed.</param>
        /// <param name="checkInventory">Set to false to override the check against current stock.</param>
        /// <returns>the matching warehouse if all of the following are true: the shipment is set, is a valid active pickup location, and matches the line 
        /// item warehouse (if set); otherwise null.</returns>
        private IWarehouse GetValidPickupFromShipment(OrderGroup orderGroup, LineItem lineItem, CheckInventoryMode checkInventory)
        {
            var shippingMethod = ShippingManager.GetShippingMethod(lineItem.ShippingMethodId).ShippingMethod.FirstOrDefault();
            if (shippingMethod == null || !ShippingManager.IsHandoffShippingMethod(shippingMethod.Name))
            {
                return null;
            }

            var pickupAddress = orderGroup.OrderAddresses.ToArray().FirstOrDefault(x => x.Name == lineItem.ShippingAddressId);
            if (pickupAddress == null)
            {
                return null;
            }

            var pickupWarehouse = ShippingManager.GetHandoffLocationFromAddressName(pickupAddress.Name);
            if (pickupWarehouse == null || (!string.IsNullOrEmpty(lineItem.WarehouseCode) && (pickupWarehouse.Code != lineItem.WarehouseCode)))
            {
                return null;
            }

            var lineApplicationId = lineItem.Parent.Parent.ApplicationId;
            if ((pickupWarehouse.ApplicationId != lineApplicationId) || !pickupWarehouse.IsActive || (!pickupWarehouse.IsPickupLocation && !pickupWarehouse.IsDeliveryLocation))
            {
                return null;
            }

            if (checkInventory == CheckInventoryMode.Check)
            {
                var catalogKey = new Mediachase.Commerce.Catalog.CatalogKey(lineApplicationId, lineItem.CatalogEntryId);
                var pickupInventory = InventoryService.Service.Get(catalogKey, pickupWarehouse);

                if (pickupInventory == null)
                {
                    return null;
                }
                if (!IsEnoughQuantity(pickupInventory, lineItem))
                {
                    pickupWarehouse = null;
                }
            }

            return pickupWarehouse;
        }

        /// <summary>
        /// Checks the warehouse assigned to an order line item to determine if it is a valid pickup location
        /// </summary>
        /// <param name="orderGroup">The order group parent for the line item.</param>
        /// <param name="lineItem">The item being processed.</param>
        /// <param name="checkInventory">Set to false to override the check against current stock.</param>
        /// <returns>true if the assigned warehouse is an active pickup location; otherwise false.</returns>
        /// <remarks>
        /// With no shipping method selected, this can only be a "could be valid" check rather than an 
        /// explicit "is valid", and thus this can report positives that will eventually be processed for
        /// delivery instead of pickups.
        /// </remarks>
        private bool IsValidPickupFromLineItem(OrderGroup orderGroup, LineItem lineItem, CheckInventoryMode checkInventory)
        {
            if (string.IsNullOrEmpty(lineItem.WarehouseCode))
            {
                return false;
            }

            var lineApplicationId = lineItem.Parent.Parent.ApplicationId;
            var lineItemWarehouse = WarehouseRepository.Service.List()
                .Where(w => w.ApplicationId == lineApplicationId
                    && w.Code == lineItem.WarehouseCode
                    && w.IsActive
                    && (w.IsPickupLocation || w.IsDeliveryLocation) // TODO: Validate that should be both rather than just pickup
                    )
                .FirstOrDefault();

            if (lineItemWarehouse == null)
            {
                return false;
            }

            // In case lineItem shipping method is InStorePickup , check if lineItemWarehouse is pickup location
            // If not, return false.
            if (lineItem.ShippingMethodName == ShippingManager.PickupShippingMethodName && !lineItemWarehouse.IsPickupLocation)
            {
                return false;
            }
            if (checkInventory == CheckInventoryMode.Ignore)
            {
                return true;
            }

            var catalogKey = new Mediachase.Commerce.Catalog.CatalogKey(lineApplicationId, lineItem.CatalogEntryId);
            var pickupInventory = InventoryService.Service.List(catalogKey).FirstOrDefault(i=>i.WarehouseCode.Equals(lineItem.WarehouseCode, StringComparison.OrdinalIgnoreCase));
            return IsEnoughQuantity(pickupInventory, lineItem);
        }

        /// <summary>
        /// Checks the warehouse assigned to an order line item to determine if it is a valid shipment fulfillment location
        /// (i.e. will ship items to a customer address)
        /// </summary>
        /// <param name="warehouse">The intended source warehouse.</param>
        /// <param name="lineItem">The order line item.</param>
        /// <param name="checkInventory">Set to false to override the check against current stock.</param>
        /// <returns>
        /// true if the warehouse can ship the item and a pickup method is not already chosen; otherwise false
        /// (i.e. the warehouse cannot ship the item, or the line item has an in-store pickup method selected).
        /// </returns>
        private bool IsValidFulfillment(IWarehouse warehouse, LineItem lineItem, CheckInventoryMode checkInventory)
        {
            if (warehouse == null || lineItem == null) { throw new ArgumentNullException(); }

            var shippingMethod = ShippingManager.GetShippingMethod(lineItem.ShippingMethodId).ShippingMethod.FirstOrDefault();
            if (shippingMethod != null && ShippingManager.IsHandoffShippingMethod(shippingMethod.Name))
            {
                return false;
            }

            var lineApplicationId = lineItem.Parent.Parent.ApplicationId;
            if ((warehouse.ApplicationId != lineApplicationId) || !warehouse.IsActive || !warehouse.IsFulfillmentCenter)
            {
                return false;
            }

            if (checkInventory == CheckInventoryMode.Ignore)
            {
                return true;
            }

            var catalogKey = new Mediachase.Commerce.Catalog.CatalogKey(lineItem.Parent.Parent.ApplicationId, lineItem.CatalogEntryId);
            var inventory = InventoryService.Service.Get(catalogKey, warehouse);
            return IsEnoughQuantity(inventory, lineItem);
        }

        /// <summary>
        /// Attempts to pick the correct source warehouse that can ship an order line item to a customer location
        /// </summary>
        /// <param name="lineItem">The item being processed.</param>
        /// <param name="checkInventory">Set to false to override the check against current stock.</param>
        /// <returns>the assignable warehouse if one exists; otherwise null.</returns>
        /// <remarks>
        /// This represents the main logic for fulfilling orders via shipments.  Custom implementations (e.g. multi-warehouse fulfillment)
        /// should be focused here.
        /// </remarks>
        private IWarehouse GetFulfillmentWarehouse(LineItem lineItem, CheckInventoryMode checkInventory)
        {
            CheckMultiWarehouse();

            var applicationId = lineItem.Parent.Parent.ApplicationId;
            var foo = WarehouseRepository.Service.List();
            var defaultWarehouse = WarehouseRepository.Service.List().Where(w => (w.ApplicationId == applicationId) && w.IsActive && w.IsFulfillmentCenter)
                                                             .SingleOrDefault();

            if (defaultWarehouse == null)
            {
                return null;
            }

            if (IsValidFulfillment(defaultWarehouse, lineItem, checkInventory))
            {
                return defaultWarehouse;
            }
             
            return null;
        }

        /// <summary>
        /// Determines whether the specific inventory has enough quantity for the line item.
        /// </summary>
        /// <param name="inventory">The inventory.</param>
        /// <param name="lineItem">The line item.</param>
        /// <returns>
        ///   <c>true</c> if has enough quantity; otherwise, <c>false</c>.
        /// </returns>
        private bool IsEnoughQuantity(IWarehouseInventory inventory, LineItem lineItem)
        {
            //TODO: Consolidate with CheckInventoryActivity
            decimal quantity = lineItem.Quantity;
            if (lineItem.InventoryStatus == (int)InventoryTrackingStatus.Enabled)
            {
                var entry = CatalogContext.Current.GetCatalogEntry(lineItem.CatalogEntryId, CatalogEntryResponseGroup_InfoWithVariations);
                if (inventory == null)
                {
                    // Treat missing as zeroes
                    inventory = new WarehouseInventory();
                    ((WarehouseInventory)inventory).InventoryStatus = InventoryTrackingStatus.Enabled;
                }
                if (entry.StartDate > FrameworkContext.Current.CurrentDateTime)
                {
                    //not allow preorder or preorder is not available
                    if (!inventory.AllowPreorder || inventory.PreorderAvailabilityDate > FrameworkContext.Current.CurrentDateTime)
                    {
                        return false;
                    }
                    //allow preorder but quantity is not enough
                    if (quantity > inventory.PreorderQuantity)
                    {
                        return false;
                    }
                }
                if (inventory.InStockQuantity > 0 && inventory.InStockQuantity >= inventory.ReservedQuantity + quantity)
                {
                    return true;
                }

                //Not enough quantity in stock, check for backorder
                if (!inventory.AllowBackorder)
                {
                    return false;
                }
                //Backorder is not available
                if (inventory.BackorderAvailabilityDate > FrameworkContext.Current.CurrentDateTime)
                {
                    return false;
                }
                //Backorder quantity is not enough
                if (quantity > inventory.InStockQuantity -
                    inventory.ReservedQuantity + inventory.BackorderQuantity)
                {
                    return false;
                }
            }
            return true;
        }

        private InventoryTrackingStatus GetInventoryStatus(string catalogEntryCode, string warehouseCode)
        {
            var entry = CatalogContext.Current.GetCatalogEntry(catalogEntryCode, CatalogEntryResponseGroup_InfoWithVariations);
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

    }
}
