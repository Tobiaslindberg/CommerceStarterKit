using System;
using System.Linq;
using System.Workflow.ComponentModel;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Inventory;
using Mediachase.Commerce.Markets;
using Mediachase.Commerce.Orders;

namespace EPiCode.Commerce.Workflow.Activities
{
	public partial class ValidateLineItemsActivity : OrderGroupActivityBase
	{        
        
        /// <summary>
		/// Initializes a new instance of the <see cref="ValidateLineItemsActivity"/> class.
		/// </summary>
		public ValidateLineItemsActivity()
			:base()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Called by the workflow runtime to execute an activity.
		/// </summary>
		/// <param name="executionContext">The <see cref="T:System.Workflow.ComponentModel.ActivityExecutionContext"/> to associate with this <see cref="T:System.Workflow.ComponentModel.Activity"/> and execution.</param>
		/// <returns>
		/// The <see cref="T:System.Workflow.ComponentModel.ActivityExecutionStatus"/> of the run task, which determines whether the activity remains in the executing state, or transitions to the closed state.
		/// </returns>
		protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
		{
			try
			{
				// Validate the properties at runtime
				this.ValidateRuntime();

                if (OrderGroup is Cart)
                {
                    // Calculate order discounts
                    this.ValidateItems();
                }

				// Retun the closed status indicating that this activity is complete.
				return ActivityExecutionStatus.Closed;
			}
			catch (Exception ex)
			{
				Logger.Error(GetType().Name + ": " + ex.Message, ex);
				// An unhandled exception occured.  Throw it back to the WorkflowRuntime.
				throw;
			}
		}

		private void ValidateItems()
		{
			CatalogRelationDto relationDto = null;
			CatalogDto.CatalogRow catalogRow = null;

            var marketTester = new ExcludedCatalogEntryMarketsField();
            var orderMarket = ServiceLocator.Current.GetInstance<IMarketService>().GetMarket(OrderGroup.MarketId);
			var orderForms = OrderGroup.OrderForms.ToArray();
			var lineItems = orderForms.SelectMany(x => x.LineItems.ToArray());
			var validLineItems = lineItems.Where(x => x.CatalogEntryId != "0" && !String.IsNullOrEmpty(x.CatalogEntryId) && !x.CatalogEntryId.StartsWith("@"));
			foreach (var lineItem in validLineItems)
			{
				var entryRow = GetEntryRowForLineItem(lineItem);

				if (entryRow == null)
				{
					AddWarningSafe(Warnings,"LineItemCodeRemoved-" + lineItem.Id, 
						String.Format("The catalog entry code that maps to the line item {0} has been removed or changed.  The line item is no longer valid", lineItem.CatalogEntryId));
					DeleteInvalidItem(orderForms,lineItem);
                    continue;
				}

                if (!marketTester.IsValidForMarket(entryRow, orderMarket))
                {
                    AddWarningSafe(Warnings, "LineItemRemoved-" + lineItem.LineItemId.ToString(), 
                        String.Format("Item \"{0}\" has been removed from the cart because it is not available in your market.", 
                        lineItem.DisplayName));
                    DeleteInvalidItem(orderForms, lineItem);
                }
                else if (entryRow.IsActive 
					&& entryRow.StartDate < FrameworkContext.Current.CurrentDateTime 
					&& entryRow.EndDate > FrameworkContext.Current.CurrentDateTime)
				{
                    if (catalogRow == null || catalogRow.CatalogId != entryRow.CatalogId)
                    {
                        var catalogDto = CatalogContext.Current.GetCatalogDto(entryRow.CatalogId);
                        catalogRow = catalogDto.Catalog.FirstOrDefault();
                    }

                    // check if catalog is visible
                    if (catalogRow != null && catalogRow.IsActive
                        && catalogRow.StartDate < FrameworkContext.Current.CurrentDateTime
                        && catalogRow.EndDate > FrameworkContext.Current.CurrentDateTime)
                    {
                        relationDto = CatalogContext.Current.GetCatalogRelationDto(entryRow.CatalogEntryId);
                        // populate item
                        lineItem.Catalog = catalogRow.Name;
                        lineItem.ParentCatalogEntryId = GetParentCatalogEntryId(entryRow.CatalogEntryId, relationDto);
                        //Inventory Info
                        IWarehouseInventory aggregateInventory = ServiceLocator.Current.GetInstance<IWarehouseInventoryService>()
                            .GetTotal(new CatalogKey(entryRow));
                        PopulateInventoryInfo(aggregateInventory, lineItem);
                        //Variation Info
                        PopulateVariationInfo(entryRow, lineItem);
                    }
				}
			}
		}

	    private static void DeleteInvalidItem(OrderForm[] orderForms, LineItem lineItem)
	    {
	        foreach (var form in orderForms)
	        {
	            form.RemoveLineItemFromShipments(lineItem);
	        }
	        lineItem.Delete();
	    }

	    private void PopulateVariationInfo(CatalogEntryDto.CatalogEntryRow entryRow, LineItem lineItem)
		{
			CatalogEntryDto.VariationRow variationRow = entryRow.GetVariationRows().FirstOrDefault();

			if (variationRow != null)
			{
				lineItem.MaxQuantity = variationRow.MaxQuantity;
				lineItem.MinQuantity = variationRow.MinQuantity;
				CustomerContact customerContact = CustomerContext.Current.GetContactById(lineItem.Parent.Parent.CustomerId);

                Money? newListPrice = GetItemPrice(entryRow, lineItem, customerContact);
                if (newListPrice.HasValue)
                {
                    Money oldListPrice = new Money(Math.Round(lineItem.ListPrice, 2), lineItem.Parent.Parent.BillingCurrency);

                    if (oldListPrice != newListPrice.Value)
                    {
                        AddWarningSafe(Warnings, 
                            "LineItemPriceChange-" + lineItem.Parent.LineItems.IndexOf(lineItem).ToString(), 
                            string.Format("Price for \"{0}\" has been changed from {1} to {2}.", lineItem.DisplayName, oldListPrice.ToString(), newListPrice.ToString()));

                        // Set new price on line item.
                        lineItem.ListPrice = newListPrice.Value.Amount;
                        if (lineItem.Parent.Parent.ProviderId.ToLower().Equals("frontend"))
                        {
                            lineItem.PlacedPrice = newListPrice.Value.Amount;
                        }
                    }
                }                
			}
		}
        	
		private static string GetParentCatalogEntryId(int catalogEntryId, CatalogRelationDto relationDto)
		{
			string retVal = null;
			var entryRelationRows = relationDto.CatalogEntryRelation.Select(String.Format("ChildEntryId={0}", catalogEntryId)).Cast<CatalogRelationDto.CatalogEntryRelationRow>();
			if (entryRelationRows.Count() > 0)
			{
				CatalogEntryDto parentEntryDto = CatalogContext.Current.GetCatalogEntryDto(entryRelationRows.First().ParentEntryId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo));
				if (parentEntryDto.CatalogEntry.Count > 0)
				{
					retVal = parentEntryDto.CatalogEntry[0].Code;
				}
			}
			return retVal;
		}

		private static CatalogEntryDto.CatalogEntryRow GetEntryRowForLineItem(LineItem lineItem)
		{
			CatalogEntryDto.CatalogEntryRow retVal = null;
			// Remove cache before proceeding
			var responseGroup = new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull);
			string cacheKey = CatalogCache.CreateCacheKey("catalogentry", responseGroup.CacheKey, lineItem.CatalogEntryId.ToString());
			CatalogCache.Remove(cacheKey);
		
			// Now get the entry
			CatalogEntryDto entryDto = CatalogContext.Current.GetCatalogEntryDto(lineItem.CatalogEntryId, responseGroup);
			if (entryDto != null)
			{
				retVal = entryDto.CatalogEntry.FirstOrDefault();
			}

			return retVal;
		}        
	}
}
