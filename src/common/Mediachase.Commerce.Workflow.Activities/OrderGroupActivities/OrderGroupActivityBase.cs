using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.Security;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;
using EPiServer.ServiceLocation;
using log4net;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Inventory;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Pricing;


namespace EPiCode.Commerce.Workflow.Activities
{
    public partial class OrderGroupActivityBase : Activity
    {
        #region Dependency properties
        public static DependencyProperty OrderGroupProperty = DependencyProperty.Register("OrderGroup", typeof(OrderGroup), typeof(OrderGroupActivityBase));

        /// <summary>
        /// Gets or sets the order group.
        /// </summary>
        /// <value>The order group.</value>
        [ValidationOption(ValidationOption.Required)]
        [Browsable(true)]
        public OrderGroup OrderGroup
        {
            get
            {
                return (OrderGroup)(base.GetValue(OrderGroupActivityBase.OrderGroupProperty));
            }
            set
            {
                base.SetValue(OrderGroupActivityBase.OrderGroupProperty, value);
            }
        }

        public static DependencyProperty WarningsProperty = DependencyProperty.Register("Warnings", typeof(StringDictionary), typeof(OrderGroupActivityBase));

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        [ValidationOption(ValidationOption.Required)]
        [Browsable(true)]
        public StringDictionary Warnings
        {
            get
            {
                return (StringDictionary)(base.GetValue(OrderGroupActivityBase.WarningsProperty));
            }
            set
            {
                base.SetValue(OrderGroupActivityBase.WarningsProperty, value);
            }
        }
        #endregion


		protected readonly ILog Logger;

        public OrderGroupActivityBase()
        {
			Logger = LogManager.GetLogger(GetType());
            InitializeComponent();
        }

        /// <summary>
        /// Validates the runtime.
        /// </summary>
        /// <returns></returns>
        protected virtual bool ValidateRuntime()
        {
            // Create a new collection for storing the validation errors
            ValidationErrorCollection validationErrors = new ValidationErrorCollection();

            // Validate the Order Properties
            this.ValidateOrderProperties(validationErrors);

            // Raise an exception if we have ValidationErrors
            if (validationErrors.HasErrors)
            {
                string validationErrorsMessage = String.Empty;

                foreach (ValidationError error in validationErrors)
                {
                    validationErrorsMessage +=
                        string.Format("Validation Error:  Number {0} - '{1}' \n",
                        error.ErrorNumber, error.ErrorText);
                }

                // Throw a new exception with the validation errors.
                throw new WorkflowValidationFailedException(validationErrorsMessage, validationErrors);

            }
            // If we made it this far, then the data must be valid.
            return true;
        }

        protected virtual void CheckMultiWarehouse()
        {
            List<IWarehouse> warehouses = ServiceLocator.Current.GetInstance<IWarehouseRepository>().List()
                .Where(w => (OrderGroup.ApplicationId == w.ApplicationId) && w.IsActive && w.IsFulfillmentCenter).ToList();
            if (warehouses.Count > 1)
            {
                throw new NotSupportedException("Multiple fulfillment centers without custom fulfillment process.");
            }
        }

        /// <summary>
        /// Validates the order properties.
        /// </summary>
        /// <param name="validationErrors">The validation errors.</param>
        protected virtual void ValidateOrderProperties(ValidationErrorCollection validationErrors)
        {
            // Validate the To property
            if (this.OrderGroup == null)
            {
                ValidationError validationError = ValidationError.GetNotSetValidationError(OrderGroupActivityBase.OrderGroupProperty.Name);
                validationErrors.Add(validationError);
            }
        }

        protected Money? GetItemPrice(CatalogEntryDto.CatalogEntryRow entry, LineItem lineItem, CustomerContact customerContact)
        {
            List<CustomerPricing> customerPricing = new List<CustomerPricing>();
            customerPricing.Add(CustomerPricing.AllCustomers);
            if (customerContact != null)
            {
                MembershipUser customerUser = CustomerContext.Current.GetUserForContact(customerContact);
                if (customerUser != null && !string.IsNullOrEmpty(customerUser.UserName))
                {
                    customerPricing.Add(new CustomerPricing(CustomerPricing.PriceType.UserName, customerUser.UserName));
                }

                if (!string.IsNullOrEmpty(customerContact.EffectiveCustomerGroup))
                {
                    customerPricing.Add(new CustomerPricing(CustomerPricing.PriceType.PriceGroup, customerContact.EffectiveCustomerGroup));
                }
            }

            IPriceService priceService = ServiceLocator.Current.GetInstance<IPriceService>();

            PriceFilter priceFilter = new PriceFilter()
            {
                Currencies = new List<Currency>() { new Currency(lineItem.Parent.Parent.BillingCurrency) },
                Quantity = lineItem.Quantity,
                CustomerPricing = customerPricing,
                ReturnCustomerPricing = false // just want one value
            };
            // Get the lowest price among all the prices matching the parameters
            IPriceValue priceValue = priceService
                .GetPrices(lineItem.Parent.Parent.MarketId, FrameworkContext.Current.CurrentDateTime, new CatalogKey(entry), priceFilter)
                .OrderBy(pv => pv.UnitPrice)
                .FirstOrDefault();

            if (priceValue == null)
            {
                return null;
            }
            else
            {
                return priceValue.UnitPrice;
            }
        }

        protected void PopulateInventoryInfo(IWarehouseInventory inv, LineItem lineItem)
        {
            if (inv != null)
            {
                lineItem.AllowBackordersAndPreorders = inv.AllowBackorder | inv.AllowPreorder;
                // Init quantities once
                lineItem.BackorderQuantity = inv.BackorderQuantity;
                lineItem.InStockQuantity = inv.InStockQuantity - inv.ReservedQuantity;
                lineItem.PreorderQuantity = inv.PreorderQuantity;
                lineItem.InventoryStatus = (int)inv.InventoryStatus;
            }
            else
            {
                var baseEntry = CatalogContext.Current.GetCatalogEntry(lineItem.CatalogEntryId,
                    new CatalogEntryResponseGroup(
                        CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo |
                        CatalogEntryResponseGroup.ResponseGroup.Variations));
                lineItem.AllowBackordersAndPreorders = false;
                lineItem.InStockQuantity = 0;
                lineItem.PreorderQuantity = 0;
                lineItem.InventoryStatus = (int)baseEntry.InventoryStatus;
            }
        }

        protected void AddWarningSafe(StringDictionary warnings, string key, string value)
        {
            string uniqueKey, uniqueKeyPrefix = key + '-';

            int counter = 1;
            do
            {
                string suffix = counter.ToString(CultureInfo.InvariantCulture);
                uniqueKey = uniqueKeyPrefix + suffix;
                ++counter;
            }
            while (warnings.ContainsKey(uniqueKey));

            warnings.Add(uniqueKey, value);
        }
    }
}
