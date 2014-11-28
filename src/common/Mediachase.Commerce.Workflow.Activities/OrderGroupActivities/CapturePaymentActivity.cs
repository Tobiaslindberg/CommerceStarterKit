using System;
using System.ComponentModel;
using System.Linq;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;
using Mediachase.BusinessFoundation.Data;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.MetaDataPlus.Configurator;

namespace EPiCode.Commerce.Workflow.Activities
{
    /// <summary>
    /// This activity is responsible for calculating the shipping prices for Payments defined for order group.
    /// It calls the appropriate interface defined by the shipping option table and passes the method id and Payment object.
    /// </summary>
	public partial class CapturePaymentActivity : OrderGroupActivityBase
	{
		public static DependencyProperty ShipmentProperty = DependencyProperty.Register("Shipment", typeof(Shipment), typeof(CapturePaymentActivity));
		public static DependencyProperty ProcessingPaymentEvent = DependencyProperty.Register("ProcessingPayment", typeof(EventHandler), typeof(CapturePaymentActivity));
		public static DependencyProperty ProcessedPaymentEvent = DependencyProperty.Register("ProcessedPayment", typeof(EventHandler), typeof(CapturePaymentActivity));

        private const string EventsCategory = "Handlers";

		/// <summary>
		/// Gets or sets the shipment.
		/// </summary>
		/// <value>The shipment.</value>
		[ValidationOption(ValidationOption.Optional)]
		[Browsable(true)]
		public Shipment Shipment
		{
			get
			{
				return (Shipment)(base.GetValue(CapturePaymentActivity.ShipmentProperty));
			}
			set
			{
				base.SetValue(CapturePaymentActivity.ShipmentProperty, value);
			}
		}

        #region Public Events


        /// <summary>
        /// Occurs when [processing Payment].
        /// </summary>
        [ValidationOption(ValidationOption.Optional)]
        [Description("The ProcessingPayment event is raised before a Payment is processed.")]
        [Category(EventsCategory)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler ProcessingPayment
        {
            add
            {
				base.AddHandler(CapturePaymentActivity.ProcessingPaymentEvent, value);
            }
            remove
            {
				base.RemoveHandler(CapturePaymentActivity.ProcessingPaymentEvent, value);
            }
        }


        /// <summary>
        /// Occurs when [processed payment].
        /// </summary>
        [ValidationOption(ValidationOption.Optional)]
        [Description("The ProcessedPayment event is raised after the payment has been processed.")]
        [Category(EventsCategory)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler ProcessedPayment
        {
            add
            {
				base.AddHandler(CapturePaymentActivity.ProcessedPaymentEvent, value);
            }
            remove
            {
				base.RemoveHandler(CapturePaymentActivity.ProcessedPaymentEvent, value);
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CapturePaymentActivity"/> class.
        /// </summary>
		public CapturePaymentActivity()
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
                // Raise the ProcessingPaymentEvent event to the parent workflow or activity
				base.RaiseEvent(CapturePaymentActivity.ProcessingPaymentEvent, this, EventArgs.Empty);
				var orderForm = this.OrderGroup.OrderForms[0];
				if (orderForm.CapturedPaymentTotal < orderForm.Total)
				{
					// Validate the properties at runtime
					this.ValidateRuntime();

					// Process payment now
					this.ProcessPayment();
				}

                // Raise the ProcessedPaymentEvent event to the parent workflow or activity
				base.RaiseEvent(CapturePaymentActivity.ProcessedPaymentEvent, this, EventArgs.Empty);

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
		/// Validates the order properties.
		/// </summary>
		/// <param name="validationErrors">The validation errors.</param>
		protected override void ValidateOrderProperties(ValidationErrorCollection validationErrors)
		{
			// Validate the To property
			if (this.OrderGroup == null)
			{
				ValidationError validationError = ValidationError.GetNotSetValidationError(CapturePaymentActivity.OrderGroupProperty.Name);
				validationErrors.Add(validationError);
			}

			var orderForm = this.OrderGroup.OrderForms[0];
			decimal shipmentTotal = CalculateShipmentTotal();
			var totalPaid = orderForm.AuthorizedPaymentTotal + orderForm.CapturedPaymentTotal;
			if (totalPaid < shipmentTotal)
			{
				Logger.Error(String.Format("Defective authorization total."));
				ValidationError validationError = new ValidationError("Defective authorization total", 205, false);
				validationErrors.Add(validationError);
			}
		}

        /// <summary>
        /// Processes the payment.
        /// </summary>
        private void ProcessPayment()
        {
            OrderGroup order = OrderGroup;

			decimal shipmentTotal = CalculateShipmentTotal();
		
			//Calculate payment total
			var formPayments = order.OrderForms[0].Payments.ToArray();
			var resultingAuthorizedPayments = PaymentTransactionTypeManager.GetResultingPaymentsByTransactionType(formPayments, TransactionType.Authorization);
			var authorizedPayments = resultingAuthorizedPayments.Where(x => PaymentStatusManager.GetPaymentStatus(x) == PaymentStatus.Processed);

			//find intire authorization
			var intirePayment = authorizedPayments.OrderBy(x => x.Amount).FirstOrDefault(x => x.Amount >= shipmentTotal);
			if (intirePayment == null)
			{
				var payments = authorizedPayments.OrderByDescending(x => x.Amount);
				foreach (Payment partialPayment in payments)
				{
					if (partialPayment.Amount < shipmentTotal)
					{
						DoCapture(partialPayment, partialPayment.Amount);
						shipmentTotal -= partialPayment.Amount;
					}
					else
					{
						DoCapture(partialPayment, shipmentTotal);
						break;
					}
				}
			}
			else
			{
				DoCapture(intirePayment, shipmentTotal);
			}
        }

		private decimal CalculateShipmentTotal()
		{
			decimal retVal = 0;
			retVal = Shipment.SubTotal + Shipment.ShipmentTotal - Shipment.ShippingDiscountAmount;
			return Math.Floor(retVal * 100) * 0.01m;
		}

		private void DoCapture(Payment authorizePayment, decimal amount)
		{
			if (String.IsNullOrEmpty(authorizePayment.TransactionID))
				authorizePayment.TransactionID = Guid.NewGuid().ToString();

			Type paymentType = null;

			PaymentMethodDto paymentMethodDto = PaymentManager.GetPaymentMethod(authorizePayment.PaymentMethodId, true);
			string className = paymentMethodDto.PaymentMethod[0].PaymentImplementationClassName;

			paymentType = AssemblyUtil.LoadType(className);

			Payment payment = this.OrderGroup.OrderForms[0].Payments.AddNew(paymentType);

			foreach (MetaField field in authorizePayment.MetaClass.MetaFields)
			{
				if (!field.Name.Equals("PaymentId", StringComparison.InvariantCultureIgnoreCase))
					payment[field.Name] = authorizePayment[field.Name];
			}

			payment.Amount = amount;
			payment.TransactionType = TransactionType.Capture.ToString();
			payment.Status = PaymentStatus.Pending.ToString();
		}

	}
}
