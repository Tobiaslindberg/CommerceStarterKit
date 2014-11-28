using System;
using System.Linq;
using System.Workflow.ComponentModel;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;

namespace EPiCode.Commerce.Workflow.Activities
{
	public partial class CalculateTotalsActivity : OrderGroupActivityBase
	{

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculateTotalsActivity"/> class.
        /// </summary>
        public CalculateTotalsActivity()
			: base()
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

                // Calculate order totals
                this.CalculateTotals();

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
        /// Calculates the totals.
        /// </summary>
        private void CalculateTotals()
        {
            decimal subTotal = 0m;
            //decimal discountTotal = 0m;
            decimal shippingTotal = 0m;
            decimal handlingTotal = 0m;
            decimal taxTotal = 0m;
            decimal total = 0m;

            // Get the property, since it is expensive process, make sure to get it once
            OrderGroup order = OrderGroup;

            // Calculate totals for OrderForms
            foreach (OrderForm form in order.OrderForms)
            {
                // Calculate totals for order form
                CalculateTotalsOrderForms(form);

                subTotal += form.SubTotal;
                //discountTotal += form.DiscountAmount;
                shippingTotal += form.ShippingTotal;
                handlingTotal += form.HandlingTotal;
                taxTotal += form.TaxTotal;
                total += form.Total;
            }

            // calculate OrderGroup totals
            order.SubTotal = subTotal;
            order.ShippingTotal = shippingTotal;
            order.TaxTotal = taxTotal;
            order.Total = total;
            order.HandlingTotal = handlingTotal;
        }

        /// <summary>
        /// Calculates the totals order forms.
        /// </summary>
        /// <param name="form">The form.</param>
        private void CalculateTotalsOrderForms(OrderForm form)
        {
            decimal subTotal = 0m;
            decimal discountTotal = 0m;
            decimal shippingDiscountTotal = 0m;
            decimal shippingTotal = 0m;

            foreach (LineItem item in form.LineItems)
            {
                decimal lineItemDiscount = item.LineItemDiscountAmount + item.OrderLevelDiscountAmount;
                item.ExtendedPrice = item.PlacedPrice * item.Quantity - lineItemDiscount;
                subTotal += item.ExtendedPrice;
                discountTotal += lineItemDiscount;
            }

            foreach (Shipment shipment in form.Shipments)
            {
				shipment.SubTotal = CalculateShipmentSubtotal(shipment);
                shippingTotal += shipment.ShipmentTotal;
                shippingTotal -= shipment.ShippingDiscountAmount;
                shippingDiscountTotal += shipment.ShippingDiscountAmount;

            }

            form.ShippingTotal = shippingTotal;
            form.DiscountAmount = discountTotal + shippingDiscountTotal;
            form.SubTotal = subTotal;

            form.Total = subTotal + shippingTotal;

			//Calculate payment total
			var formPayments = form.Payments.ToArray();
			var resultingAuthorizedPayments = PaymentTransactionTypeManager.GetResultingPaymentsByTransactionType(formPayments, TransactionType.Authorization);
			var resultingCapturedPayments = PaymentTransactionTypeManager.GetResultingPaymentsByTransactionType(formPayments, TransactionType.Capture);
			var resultingSalsePayments = PaymentTransactionTypeManager.GetResultingPaymentsByTransactionType(formPayments, TransactionType.Sale);
			var resultingCreditPayments = PaymentTransactionTypeManager.GetResultingPaymentsByTransactionType(formPayments, TransactionType.Credit);

			form.AuthorizedPaymentTotal = resultingAuthorizedPayments.Where(x => PaymentStatusManager.GetPaymentStatus(x) == PaymentStatus.Processed).Sum(y => y.Amount);
			form.CapturedPaymentTotal = resultingSalsePayments.Where(x => PaymentStatusManager.GetPaymentStatus(x) == PaymentStatus.Processed).Sum(y => y.Amount);
			form.CapturedPaymentTotal += resultingCapturedPayments.Where(x => PaymentStatusManager.GetPaymentStatus(x) == PaymentStatus.Processed).Sum(y => y.Amount);
			form.CapturedPaymentTotal -= resultingCreditPayments.Where(x => PaymentStatusManager.GetPaymentStatus(x) == PaymentStatus.Processed).Sum(y => y.Amount);
        }


		private static decimal CalculateShipmentSubtotal(Shipment shipment)
		{
			var retVal = 0m;
			foreach (var lineItem in Shipment.GetShipmentLineItems(shipment))
			{
				if(lineItem.Quantity > 0 )
				{
					retVal += lineItem.ExtendedPrice / lineItem.Quantity * Shipment.GetLineItemQuantity(shipment, lineItem.LineItemId);
				}
			}
			return Math.Floor(retVal * 100) * 0.01m;
		}

	}


}
