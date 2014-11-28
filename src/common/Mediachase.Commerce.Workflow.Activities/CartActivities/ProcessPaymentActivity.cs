using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Exceptions;
using Mediachase.Commerce.Orders.Managers;

namespace EPiCode.Commerce.Workflow.Activities
{
    /// <summary>
    /// This activity handles processing different types of payments. It will call the appropriate 
    /// payment handler configured in the database and raise exceptions if something goes wrong.
    /// It also deals with removing sensitive data for credit card types of payments depending on the 
    /// configuration settings.
    /// </summary>
	public partial class ProcessPaymentActivity : CartActivityBase
	{
		public static DependencyProperty PaymentProperty = DependencyProperty.Register("Payment", typeof(Payment), typeof(ProcessPaymentActivity));
        public static DependencyProperty ProcessingPaymentEvent = DependencyProperty.Register("ProcessingPayment", typeof(EventHandler), typeof(ProcessPaymentActivity));
        public static DependencyProperty ProcessedPaymentEvent = DependencyProperty.Register("ProcessedPayment", typeof(EventHandler), typeof(ProcessPaymentActivity));

        private const string EventsCategory = "Handlers";

        // Define private constants for the Validation Errors 
        private const int TotalPaymentMismatch = 1;


		/// <summary>
		/// Gets or sets the payment.
		/// </summary>
		/// <value>The payment.</value>
		[ValidationOption(ValidationOption.Optional)]
		[Browsable(true)]
		public Payment Payment
		{
			get
			{
				return (Payment)(base.GetValue(ProcessPaymentActivity.PaymentProperty));
			}
			set
			{
				base.SetValue(ProcessPaymentActivity.PaymentProperty, value);
			}
		}

        #region Public Events


        /// <summary>
        /// Occurs when [processing payment].
        /// </summary>
        [ValidationOption(ValidationOption.Optional)]
        [Description("The ProcessingPayment event is raised before a payment is processed.")]
        [Category(EventsCategory)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler ProcessingPayment
        {
            add
            {
                base.AddHandler(ProcessPaymentActivity.ProcessingPaymentEvent, value);
            }
            remove
            {
                base.RemoveHandler(ProcessPaymentActivity.ProcessingPaymentEvent, value);
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
                base.AddHandler(ProcessPaymentActivity.ProcessedPaymentEvent, value);
            }
            remove
            {
                base.RemoveHandler(ProcessPaymentActivity.ProcessedPaymentEvent, value);
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessPaymentActivity"/> class.
        /// </summary>
        public ProcessPaymentActivity()
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
            try
            {
                // Raise the ProcessingPaymentEvent event to the parent workflow or activity
                base.RaiseEvent(ProcessPaymentActivity.ProcessingPaymentEvent, this, EventArgs.Empty);

                // Validate the properties at runtime
                this.ValidateRuntime();

                // Process payment now
                this.ProcessPayment();

                // Raise the ProcessedPaymentEvent event to the parent workflow or activity
                base.RaiseEvent(ProcessPaymentActivity.ProcessedPaymentEvent, this, EventArgs.Empty);

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
        /// Validates the order properties.
        /// </summary>
        /// <param name="validationErrors">The validation errors.</param>
        protected override void ValidateOrderProperties(ValidationErrorCollection validationErrors)
        {
            // Validate the To property
            if (this.OrderGroup == null)
            {
                ValidationError validationError = ValidationError.GetNotSetValidationError(ProcessPaymentActivity.OrderGroupProperty.Name);
                validationErrors.Add(validationError);
            }

			if (this.Payment == null)
			{
				// Cycle through all Order Forms and check total, it should be equal to total of all payments
				decimal paymentTotal = 0;
				foreach (OrderForm orderForm in OrderGroup.OrderForms)
				{
					foreach (Payment payment in orderForm.Payments)
					{
						paymentTotal += payment.Amount;
					}
				}

				if (paymentTotal < OrderGroup.Total)
				{
					Logger.Error(String.Format("Payment Total Price less that order total price."));
					ValidationError validationError = new ValidationError("The payment total and the order total do not not match. Please adjust your payment.",
										TotalPaymentMismatch, false);
					validationErrors.Add(validationError);
				}
			}
        }

        /// <summary>
        /// Processes the payment.
        /// </summary>
        private void ProcessPayment()
        {
            // If total is 0, we do not need to proceed
            if (OrderGroup.Total == 0 || OrderGroup is PaymentPlan)
                return;

            // Start Charging!
            PaymentMethodDto methods = PaymentManager.GetPaymentMethods(/*Thread.CurrentThread.CurrentCulture.Name*/String.Empty);
            foreach (OrderForm orderForm in OrderGroup.OrderForms)
            {
                foreach (Payment payment in orderForm.Payments)
                {
					if (this.Payment != null && !this.Payment.Equals(payment))
						continue;

					//Do not process payments with status Processing and Fail
					var paymentStatus = PaymentStatusManager.GetPaymentStatus(payment);
					if (paymentStatus != PaymentStatus.Pending)
						continue;

					PaymentMethodDto.PaymentMethodRow paymentMethod = methods.PaymentMethod.FindByPaymentMethodId(payment.PaymentMethodId);

                    // If we couldn't find payment method specified, generate an error
					if (paymentMethod == null)
                    {
                        throw new MissingMethodException(String.Format("Specified payment method \"{0}\" has not been defined.", payment.PaymentMethodId));
                    }

					Logger.Debug(String.Format("Getting the type \"{0}\".", paymentMethod.ClassName));
					Type type = Type.GetType(paymentMethod.ClassName);
                    if (type == null)
						throw new TypeLoadException(String.Format("Specified payment method class \"{0}\" can not be created.", paymentMethod.ClassName));

                    Logger.Debug(String.Format("Creating instance of \"{0}\".", type.Name));
                    IPaymentGateway provider = (IPaymentGateway)Activator.CreateInstance(type);

					provider.Settings = CreateSettings(paymentMethod);

                    string message = "";
                    Logger.Debug(String.Format("Processing the payment."));
					if (provider.ProcessPayment(payment, ref message))
					{
						Mediachase.Commerce.Orders.Managers.PaymentStatusManager.ProcessPayment(payment);
					}
					else
                    {
                        throw new PaymentException(PaymentException.ErrorType.ProviderError, "", String.Format(message));
                    }
                    Logger.Debug(String.Format("Payment processed."));
                    PostProcessPayment(payment);

                    // TODO: add message to transaction log
                }
            }
        }

        /// <summary>
        /// Pres the process payment. Unencrypts the data if needed.
        /// </summary>
        private void PostProcessPayment(Payment payment)
        {
            // We only care about credit cards here, all other payment types should be encrypted by default
            if (payment.GetType() == typeof(CreditCardPayment))
            {
                // for partial type, remove everything but last 4 digits
                if (OrderConfiguration.Instance.SensitiveDataMode == SensitiveDataPersistance.Partial)
                {
                    string ccNumber = ((CreditCardPayment)payment).CreditCardNumber;
                    if (!String.IsNullOrEmpty(ccNumber) && ccNumber.Length > 4)
                    {
                        ccNumber = ccNumber.Substring(ccNumber.Length - 4);
                        ((CreditCardPayment)payment).CreditCardNumber = ccNumber;
                    }
                }
                else if (OrderConfiguration.Instance.SensitiveDataMode == SensitiveDataPersistance.DoNotPersist)
                {
                    ((CreditCardPayment)payment).CreditCardNumber = String.Empty; 
                }

                // Always remove pin
                ((CreditCardPayment)payment).CreditCardSecurityCode = String.Empty;
            }
        }

        /// <summary>
        /// Creates the settings.
        /// </summary>
        /// <param name="methodRow">The method row.</param>
        /// <returns></returns>
        private Dictionary<string, string> CreateSettings(PaymentMethodDto.PaymentMethodRow methodRow)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();

            PaymentMethodDto.PaymentMethodParameterRow[] rows = methodRow.GetPaymentMethodParameterRows();
            foreach (PaymentMethodDto.PaymentMethodParameterRow row in rows)
            {
                settings.Add(row.Parameter, row.Value);
            }

            return settings;
        }

	}
}
