using System;
using System.Collections.Generic;
using System.Linq;
using System.Workflow.ComponentModel;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Markets;
using Mediachase.Commerce.Orders;

namespace EPiCode.Commerce.Workflow.Activities
{
	public partial class CalculateTaxActivity : OrderGroupActivityBase
	{

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculateTaxActivity"/> class.
        /// </summary>
        public CalculateTaxActivity()
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

                // Calculate sale tax
                this.CalculateSaleTaxes();

                // Retun the closed status indicating that this activity is complete.
                return ActivityExecutionStatus.Closed;
            }
            catch(Exception ex)
            {
				Logger.Error(GetType().Name + ": " + ex.Message, ex);
                // An unhandled exception occured.  Throw it back to the WorkflowRuntime.
                throw;
            }
        }

        /// <summary>
        /// Calculates the sale taxes.
        /// </summary>
        private void CalculateSaleTaxes()
        {
            // Get the property, since it is expensive process, make sure to get it once
            OrderGroup order = OrderGroup;
            
            foreach (OrderForm form in order.OrderForms)
            {
                
                decimal totalTaxes = 0;
                foreach (Shipment shipment in form.Shipments)
                {
                    List<LineItem> items = GetSplitShipmentLineItems(shipment);

                    // Calculate sales and shipping taxes per items
                    foreach (LineItem item in items)
                    {
                        // Try getting an address
                        OrderAddress address = GetAddressByName(form, shipment.ShippingAddressId);
                        if (address != null) // no taxes if there is no address
                        {
                            // Try getting an entry
                            CatalogEntryDto entryDto = CatalogContext.Current.GetCatalogEntryDto(item.CatalogEntryId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
                            if (entryDto.CatalogEntry.Count > 0) // no entry, no tax category, no tax
                            {
                                CatalogEntryDto.VariationRow[] variationRows = entryDto.CatalogEntry[0].GetVariationRows();
                                if (variationRows.Length > 0)
                                {
                                    string taxCategory = CatalogTaxManager.GetTaxCategoryNameById(variationRows[0].TaxCategoryId);
                                    IMarket market = ServiceLocator.Current.GetInstance<IMarketService>().GetMarket(order.MarketId);                                    
                                    TaxValue[] taxes = OrderContext.Current.GetTaxes(Guid.Empty, taxCategory, market.DefaultLanguage.Name, address.CountryCode, address.State, address.PostalCode, address.RegionCode, String.Empty, address.City);

                                    if (taxes.Length > 0)
                                    {
                                        foreach (TaxValue tax in taxes)
                                        {
                                            if(tax.TaxType == TaxType.SalesTax)
                                                totalTaxes += item.ExtendedPrice * ((decimal)tax.Percentage / 100);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                form.TaxTotal = Math.Round(totalTaxes,2);
            }         
        }

        /// <summary>
        /// Gets the name of the address by name.
        /// </summary>
        /// <param name="form">The form.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private OrderAddress GetAddressByName(OrderForm form, string name)
        {
            foreach (OrderAddress address in form.Parent.OrderAddresses)
            {
                if (address.Name.Equals(name))
                    return address;
            }

            return null;
        }

        private List<LineItem> GetSplitShipmentLineItems(Shipment shipment)
        {
            List<LineItem> items = Shipment.GetShipmentLineItems(shipment);
            return items.Select(x => new LineItem
            {
                CatalogEntryId = x.CatalogEntryId,
                ExtendedPrice = x.ExtendedPrice / x.Quantity * Shipment.GetLineItemQuantity(shipment, x.LineItemId), // Need the extended price for this shipment
                Quantity = Shipment.GetLineItemQuantity(shipment, x.LineItemId),
            }).ToList();

        }
	}
}
