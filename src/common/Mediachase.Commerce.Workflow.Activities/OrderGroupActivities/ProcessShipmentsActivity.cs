using System;
using System.ComponentModel;
using System.Linq;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;

using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Markets;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Shared;
using System.Threading;

namespace EPiCode.Commerce.Workflow.Activities
{
    /// <summary>
    /// This activity is responsible for calculating the shipping prices for shipments defined for order group.
    /// It calls the appropriate interface defined by the shipping option table and passes the method id and shipment object.
    /// </summary>
    public partial class ProcessShipmentsActivity : OrderGroupActivityBase
    {
        public static DependencyProperty ProcessingShipmentEvent = DependencyProperty.Register("ProcessingShipment", typeof(EventHandler), typeof(ProcessShipmentsActivity));
        public static DependencyProperty ProcessedShipmentEvent = DependencyProperty.Register("ProcessedShipment", typeof(EventHandler), typeof(ProcessShipmentsActivity));

        private const string EventsCategory = "Handlers";

        #region Public Events


        /// <summary>
        /// Occurs when [processing shipment].
        /// </summary>
        [ValidationOption(ValidationOption.Optional)]
        [Description("The ProcessingShipment event is raised before a Shipment is processed.")]
        [Category(EventsCategory)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler ProcessingShipment
        {
            add
            {
                base.AddHandler(ProcessShipmentsActivity.ProcessingShipmentEvent, value);
            }
            remove
            {
                base.RemoveHandler(ProcessShipmentsActivity.ProcessingShipmentEvent, value);
            }
        }


        /// <summary>
        /// Occurs when [processed shipment].
        /// </summary>
        [ValidationOption(ValidationOption.Optional)]
        [Description("The ProcessedShipment event is raised after the Shipment has been processed.")]
        [Category(EventsCategory)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler ProcessedShipment
        {
            add
            {
                base.AddHandler(ProcessShipmentsActivity.ProcessedShipmentEvent, value);
            }
            remove
            {
                base.RemoveHandler(ProcessShipmentsActivity.ProcessedShipmentEvent, value);
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessShipmentsActivity"/> class.
        /// </summary>
        public ProcessShipmentsActivity()
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
                // Raise the ProcessingShipmentEvent event to the parent workflow or activity
                base.RaiseEvent(ProcessShipmentsActivity.ProcessingShipmentEvent, this, EventArgs.Empty);

                // Validate the properties at runtime
                this.ValidateRuntime();

                // Process Shipment now
                this.ProcessShipments();

                // Raise the ProcessedShipmentEvent event to the parent workflow or activity
                base.RaiseEvent(ProcessShipmentsActivity.ProcessedShipmentEvent, this, EventArgs.Empty);

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
        /// Processes the shipments.
        /// </summary>
        private void ProcessShipments()
        {
            ShippingMethodDto methods = ShippingManager.GetShippingMethods(Thread.CurrentThread.CurrentUICulture.Name);

            OrderGroup order = OrderGroup;

            // request rates, make sure we request rates not bound to selected delivery method
            foreach (OrderForm form in order.OrderForms)
            {
                foreach (Shipment shipment in form.Shipments)
                {
                    bool processThisShipment = true;

                    string discountName = "@ShipmentSkipRateCalc";
                    // If you find the shipment discount which represents 
                    if (shipment.Discounts.Cast<ShipmentDiscount>().Any(x => x.ShipmentId == shipment.ShipmentId && x.DiscountName.Equals(discountName)))
                    {
                        processThisShipment = false;
                    }

                    if (!processThisShipment)
                    {
                        continue;
                    }

                    ShippingMethodDto.ShippingMethodRow row = methods.ShippingMethod.FindByShippingMethodId(shipment.ShippingMethodId);

                    // If shipping method is not found, set it to 0 and continue
                    if (row == null)
                    {
                        Logger.Info(String.Format("Total shipment is 0 so skip shipment calculations."));
                        shipment.ShipmentTotal = 0;
                        continue;
                    }

                    // Check if package contains shippable items, if it does not use the default shipping method instead of the one specified
                    Logger.Debug(String.Format("Getting the type \"{0}\".", row.ShippingOptionRow.ClassName));
                    Type type = Type.GetType(row.ShippingOptionRow.ClassName);
                    if (type == null)
                    {
                        throw new TypeInitializationException(row.ShippingOptionRow.ClassName, null);
                    }
                    Logger.Debug(String.Format("Creating instance of \"{0}\".", type.Name));
                    IShippingGateway provider = null;
                    var orderMarket = ServiceLocator.Current.GetInstance<IMarketService>().GetMarket(order.MarketId);
                    if (orderMarket != null)
                    {
                        provider = (IShippingGateway)Activator.CreateInstance(type, orderMarket);
                    }
                    else
                    {
                        provider = (IShippingGateway)Activator.CreateInstance(type);
                    }

                    Logger.Debug(String.Format("Calculating the rates."));
                    string message = String.Empty;
                    ShippingRate rate = provider.GetRate(row.ShippingMethodId, shipment, ref message);
                    if (rate != null)
                    {
                        Logger.Debug(String.Format("Rates calculated."));
                        // check if shipment currency is convertable to Billing currency, and then convert it
                        if (!CurrencyFormatter.CanBeConverted(rate.Money, order.BillingCurrency))
                        {
                            Logger.Debug(String.Format("Cannot convert selected shipping's currency({0}) to current currency({1}).", rate.Money.Currency.CurrencyCode, order.BillingCurrency));
                            throw new Exception(String.Format("Cannot convert selected shipping's currency({0}) to current currency({1}).", rate.Money.Currency.CurrencyCode, order.BillingCurrency));
                        }
                        else
                        {
                            Money convertedRate = CurrencyFormatter.ConvertCurrency(rate.Money, order.BillingCurrency);
                            shipment.ShipmentTotal = convertedRate.Amount;
                        }
                    }
                    else
                    {
                        Warnings[String.Concat("NoShipmentRateFound-", shipment.ShippingMethodName)] =
                            String.Concat("No rates have been found for ", shipment.ShippingMethodName);
                        Logger.Debug(String.Format("No rates have been found."));
                    }
                }
            }
        }
    }
}
