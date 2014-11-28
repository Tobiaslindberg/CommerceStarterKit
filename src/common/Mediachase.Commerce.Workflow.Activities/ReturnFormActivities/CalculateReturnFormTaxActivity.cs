using System;
using System.Collections.Generic;
using System.Threading;
using System.Workflow.ComponentModel;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Orders;

namespace EPiCode.Commerce.Workflow.Activities
{
	public partial class CalculateReturnFormTaxActivity : ReturnFormBaseActivity
	{
		public CalculateReturnFormTaxActivity()
			:base()
		{
			InitializeComponent();
		}

		protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
		{
			try
			{

				// Calculate sale tax
				this.CalculateSaleTaxes();

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

		/// <summary>
		/// Calculates the sale taxes.
		/// </summary>
		private void CalculateSaleTaxes()
		{

			decimal totalTaxes = 0;
			foreach (Shipment shipment in base.ReturnOrderForm.Shipments)
			{
				totalTaxes += CalculateShipmentSaleTaxes(shipment);
			}
			base.ReturnOrderForm.TaxTotal = totalTaxes;
		}

		private decimal CalculateShipmentSaleTaxes(Shipment shipment)
		{
			decimal retVal = 0;
			List<LineItem> items = Shipment.GetShipmentLineItems(shipment);
			foreach (LineItem item in items)
			{
				retVal += CalculateLineItemSaleTaxes(item, shipment.ShippingAddressId);
			}

			return retVal;
		}

		private decimal CalculateLineItemSaleTaxes(LineItem lineItem, string shipmentAddressId)
		{
			decimal retVal = 0;
			// Try getting an address
			OrderAddress address = GetAddressByName(base.ReturnOrderForm, shipmentAddressId);
			if (address == null) // no taxes if there is no address
			{
				return 0m;
			}
			// Try getting an entry
			CatalogEntryDto entryDto = CatalogContext.Current.GetCatalogEntryDto(lineItem.CatalogEntryId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
			if (entryDto.CatalogEntry.Count == 0) // no entry, no tax category, no tax
			{
				return 0m;
			}
			CatalogEntryDto.VariationRow[] variationRows = entryDto.CatalogEntry[0].GetVariationRows();
			if (variationRows.Length == 0)
			{
				return 0m;
			}
			string taxCategory = CatalogTaxManager.GetTaxCategoryNameById(variationRows[0].TaxCategoryId);
			TaxValue[] taxes = OrderContext.Current.GetTaxes(Guid.Empty, taxCategory, Thread.CurrentThread.CurrentCulture.Name, address.CountryCode, address.State, address.PostalCode, address.RegionCode, String.Empty, address.City);
			if (taxes.Length == 0)
			{
				return 0m;
			}

			foreach (TaxValue tax in taxes)
			{
				if (tax.TaxType == TaxType.SalesTax)
				{
                    var taxFactor = 1 + ((decimal)tax.Percentage / 100);

                    retVal += lineItem.ExtendedPrice - (lineItem.ExtendedPrice / taxFactor);   
				//	retVal += lineItem.ExtendedPrice * ((decimal)tax.Percentage / 100);
				}
			}
			return retVal;
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
	}
}
