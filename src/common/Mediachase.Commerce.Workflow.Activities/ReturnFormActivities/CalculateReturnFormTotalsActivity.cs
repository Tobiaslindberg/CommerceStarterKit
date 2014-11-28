using System;
using System.Workflow.ComponentModel;
using Mediachase.Commerce.Orders;

namespace EPiCode.Commerce.Workflow.Activities
{
	public partial class CalculateReturnFormTotalsActivity : ReturnFormBaseActivity
	{
		public CalculateReturnFormTotalsActivity()
			:base()
		{
			InitializeComponent();
		}

	
		protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
		{
			try
			{
				// Calculate order totals
				this.CalculateTotalsReturnOrderForm();

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
		/// Calculates the totals order forms.
		/// </summary>
		private void CalculateTotalsReturnOrderForm()
		{
			decimal subTotal = 0m;
			decimal discountTotal = 0m;
			decimal shippingDiscountTotal = 0m;
			decimal shippingTotal = 0m;

			foreach (LineItem item in base.ReturnOrderForm.LineItems)
			{
                if (item.Quantity == item.ReturnQuantity)
                {
                    item.ExtendedPrice = item.PlacedPrice * item.ReturnQuantity - item.LineItemDiscountAmount - item.OrderLevelDiscountAmount;
                    subTotal += item.PlacedPrice * item.ReturnQuantity - item.LineItemDiscountAmount;
                    discountTotal += item.OrderLevelDiscountAmount;
                }
                else
                {
                    item.ExtendedPrice =
                        (item.PlacedPrice - Decimal.Round((item.LineItemDiscountAmount / item.Quantity), 2)) * item.ReturnQuantity -
                        Decimal.Round((item.OrderLevelDiscountAmount / item.Quantity), 2) * item.ReturnQuantity;
                    subTotal += (item.PlacedPrice - Decimal.Round((item.LineItemDiscountAmount / item.Quantity), 2)) * item.ReturnQuantity;
                    discountTotal += Decimal.Round((item.OrderLevelDiscountAmount / item.Quantity), 2) * item.ReturnQuantity;
                }
                //discountTotal += item.LineItemDiscountAmount;
			}

			foreach (Shipment shipment in base.ReturnOrderForm.Shipments)
			{
				shippingTotal += shipment.ShipmentTotal;
				shippingDiscountTotal += shipment.ShippingDiscountAmount;
			}

			discountTotal += shippingDiscountTotal;

			ReturnOrderForm.ShippingTotal = shippingTotal;
			ReturnOrderForm.DiscountAmount = discountTotal;
			ReturnOrderForm.SubTotal = subTotal;

			ReturnOrderForm.Total = subTotal + shippingTotal  - discountTotal;
		}

	}
}
