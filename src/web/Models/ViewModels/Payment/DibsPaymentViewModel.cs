/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using log4net;
using Mediachase.Commerce;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Website.Helpers;
using OxxCommerceStarterKit.Core.PaymentProviders;
using OxxCommerceStarterKit.Core.PaymentProviders.DIBS;
using OxxCommerceStarterKit.Web.Business;
using OxxCommerceStarterKit.Web.Models.PageTypes;
using OxxCommerceStarterKit.Web.Models.PageTypes.Payment;

namespace OxxCommerceStarterKit.Web.Models.ViewModels.Payment
{
    public class DibsPaymentViewModel : PageViewModel<DibsPaymentPage>
    {
        protected static ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Cart _currentCart = null;
        private Mediachase.Commerce.Orders.Payment _payment;
        private PaymentMethodDto _paymentMethod;

        //private string _acceptUrl;
        //private string _cancelUrl;
        //private string _callbackUrl;

        public const string DIBSSystemName = "DIBS";
        public string ProcessingUrl { get; set; }
        public string MD5K1 { get; set; }
        public string MD5K2 { get; set; }
        public string CallbackUrl { get; set; }
		public bool IsTest { get; set; }

        /// <summary>
        /// Gets or sets the order line data sent to DIBS
        /// </summary>
        /// <remarks>
        /// This will never be null, but can be empty
        /// </remarks>
        /// <value>
        /// The semi colon separated list of products
        /// </value>
        public List<string> Products { get; set; }
        public string DibsSystemName { get { return DIBSSystemName; } }
        /// <summary>
        /// The MAC is calculated using the HMAC algorithm with SHA-256. The HMAC algorithm requires a message and one key. 
        /// </summary>
        /// <remarks>
        /// See http://tech.dibspayment.com/DPW_hosted_HMAC_calculation for more information
        /// </remarks>
        /// <value>
        /// The mac.
        /// </value>
        public string MAC { get; set; }
        public string AcceptReturnUrl { get; set; }
        public string CancelReturnUrl { get; set; }
        /// <summary>
        /// Gets the order ID.
        /// </summary>
        /// <value>The order ID.</value>
        public string OrderID { get; private set; }
        public string OrderInfo { get; set; }
        public string OuterKey { get; set; }
        public string InnerKey { get; set; }
        public string Key { get; set; }

        private Cart CurrentCart
        {
            get
            {
                if (_currentCart == null)
                {
                    _currentCart = new CartHelper(Cart.DefaultName).Cart;
                }

                return _currentCart;
            }
            set { _currentCart = value; }
        }


        /// <summary>
        /// Gets the merchant ID.
        /// </summary>
        /// <value>The merchant ID.</value>
        public string MerchantID
        {
            get
            {
                return DIBSPaymentGateway.GetParameterByName(_paymentMethod, DIBSPaymentGateway.UserParameter).Value;
            }
        }

        /// <summary>
        /// Gets the MD5 authentication key to send to DIBS.
        /// </summary>
        public string MD5Key
        {
            get
            {
                return DIBSPaymentGateway.GetMD5Key(MerchantID, OrderID, Currency, Amount, _paymentMethod);
            }
        }


        /// <summary>
        /// Gets the amount.
        /// </summary>
        /// <value>The amount.</value>
        public string Amount
        {
            get
            {
                return (_payment != null) ? (_payment.Amount * 100).ToString("#") : string.Empty;
            }
        }

        /// <summary>
        /// Gets the currency code.
        /// </summary>
        /// <value>The currency code.</value>
        public Currency Currency
        {
            get
            {
                if (_payment == null)
                {
                    return string.Empty;
                }
                return string.IsNullOrEmpty(_payment.Parent.Parent.BillingCurrency) ?
                    SiteContext.Current.Currency : new Currency(_payment.Parent.Parent.BillingCurrency);
            }
        }


        /// <summary>
        /// Convert the site language to a language DIBS can support.
        /// </summary>
        /// <value>The current language in a format usable by DIBS.</value>
        public new string Language
        {
            get
            {
                if (SiteContext.Current.LanguageName.StartsWith("da", StringComparison.OrdinalIgnoreCase))
                    return "da";
                else if (SiteContext.Current.LanguageName.StartsWith("sv", StringComparison.OrdinalIgnoreCase))
                    return "sv";
                else if (SiteContext.Current.LanguageName.StartsWith("no", StringComparison.OrdinalIgnoreCase))
                    return "no";
                else if (SiteContext.Current.LanguageName.StartsWith("en", StringComparison.OrdinalIgnoreCase))
                    return "en";
                else if (SiteContext.Current.LanguageName.StartsWith("nl", StringComparison.OrdinalIgnoreCase))
                    return "nl";
                else if (SiteContext.Current.LanguageName.StartsWith("de", StringComparison.OrdinalIgnoreCase))
                    return "de";
                else if (SiteContext.Current.LanguageName.StartsWith("fr", StringComparison.OrdinalIgnoreCase))
                    return "fr";
                else if (SiteContext.Current.LanguageName.StartsWith("fi", StringComparison.OrdinalIgnoreCase))
                    return "fi";
                else if (SiteContext.Current.LanguageName.StartsWith("es", StringComparison.OrdinalIgnoreCase))
                    return "es";
                else if (SiteContext.Current.LanguageName.StartsWith("it", StringComparison.OrdinalIgnoreCase))
                    return "it";
                else if (SiteContext.Current.LanguageName.StartsWith("fo", StringComparison.OrdinalIgnoreCase))
                    return "fo";
                else if (SiteContext.Current.LanguageName.StartsWith("pl", StringComparison.OrdinalIgnoreCase))
                    return "pl";
                return "en";
            }
        }


        public DibsPaymentViewModel(IContentRepository contentRepository, DibsPaymentPage currentPage, OrderInfo orderInfo, Cart cart) : base(currentPage)
        {
            SiteConfiguration configuration = SiteConfiguration.Current();
            PaymentMethodDto dibs = PaymentManager.GetPaymentMethodBySystemName(DIBSSystemName, SiteContext.Current.LanguageName);
            _paymentMethod = dibs;
            _currentCart = cart;

            DIBSPaymentGateway gw = new DIBSPaymentGateway();

            orderInfo.Merchant = gw.Merchant;
           
            var paymentRedirectUrl = GetViewUrl(currentPage.ContentLink);
            IsTest = orderInfo.IsTest;

            var baseUrl = GetBaseUrl();

            AcceptReturnUrl = baseUrl + paymentRedirectUrl + "ProcessPayment";
            orderInfo.AcceptReturnUrl = AcceptReturnUrl;

            CancelReturnUrl = baseUrl + paymentRedirectUrl + "CancelPayment";
            orderInfo.CancelReturnUrl = CancelReturnUrl;
            
			Mediachase.Commerce.Orders.Payment[] payments;
			if (CurrentCart != null && CurrentCart.OrderForms != null && CurrentCart.OrderForms.Count > 0)
			{
				payments = CurrentCart.OrderForms[0].Payments.ToArray();
			}
			else
			{
				payments = new Mediachase.Commerce.Orders.Payment[0];
			}
            _payment = payments.FirstOrDefault(c => c.PaymentMethodId.Equals(dibs.PaymentMethod.Rows[0]["PaymentMethodId"]));
            ProcessingUrl = DIBSPaymentGateway.GetParameterByName(dibs, DIBSPaymentGateway.ProcessingUrl).Value;
            MD5K1 = DIBSPaymentGateway.GetParameterByName(dibs, DIBSPaymentGateway.MD5Key1).Value;
            MD5K2 = DIBSPaymentGateway.GetParameterByName(dibs, DIBSPaymentGateway.MD5Key2).Value;
            Key = gw.Key;
            InnerKey = gw.InnerKey;
            OuterKey = gw.OuterKey;
            Key = gw.Key;
            OrderID = orderInfo.OrderId;
            
            ShaCalculator calculator = new ShaCalculator(InnerKey, OuterKey, Key);

            if (CurrentCart != null && CurrentCart.OrderForms != null && CurrentCart.OrderForms.Count > 0)
            {
                // Note, the orderinfo is changed inside this method
                Products = GenerateLineItemInformation(CurrentCart.OrderForms[0].LineItems, ref orderInfo);
            }
            else
            {
                Products = new List<string>();
            }

            MAC = calculator.GetHex(orderInfo);
#if DEBUG
            string macVerification = calculator.GetMac(orderInfo);
            if(string.Compare(MAC, macVerification, StringComparison.InvariantCulture) != 0)
            {
                throw new SecurityException("Cannot verify HMAC calculation");
            }
#endif
                      
            this.OrderInfo = orderInfo.ToString();
        }

        private string GetViewUrl(ContentReference contentLink)
        {
            var url = UrlResolver.Current.GetUrl(
                contentLink,
                null,
                new VirtualPathArguments() {ContextMode = ContextMode.Default});
            return url;
        }

        /// <summary>
        /// Gets the base URL to be used for return and cancel actions sent to DIBS.
        /// </summary>
        /// <returns>A fully qualified url to the running site.</returns>
        protected string GetBaseUrl()
        {
            string baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string forcedHostUrl = Tools.GetAppSetting("DIBS.ForcedHostUrl");
            if (string.IsNullOrWhiteSpace(forcedHostUrl) == false)
            {
                baseUrl = forcedHostUrl.TrimEnd('/');
            }
            return baseUrl;
        }

        private List<string> GenerateLineItemInformation(LineItemCollection lineItems, ref OrderInfo orderInfo)
        {
            orderInfo.OrderLinesHeader = "QUANTITY;UNITCODE;DESCRIPTION;AMOUNT;ITEMID;VATPERCENT";
            var items = new List<string>();
            foreach (LineItem lineItem in lineItems)
            {
                var lineInfo = string.Format("{0};{1};{2};{3};{4};{5}", 
                    (int) lineItem.Quantity, 
                    "pcs",
                    lineItem.DisplayName, 
                    (int) (GetTotalPriceWithoutVat(lineItem, orderInfo.VatPercent) * 100), 
                    lineItem.LineItemId, 
                    (int) orderInfo.VatPercent * 100);
                items.Add(lineInfo);
                orderInfo.OrderLines.Add(lineInfo);
            }

            return items;

            // Examples
            //<input type="hidden" name="oiRow2" value="2;pcs;ACME Band Aid;1125;99;5025" />
            //<input type="hidden" name="oiRow3" value="3;meter;ACME Black wool fabric;101;45;5000" />
        }

        /// <summary>
        /// Calculates VAT according to http://tech.dibspayment.com/DPW_hosted_input_parameters_order_information
        /// </summary>
        /// <remarks>
        /// Important! This assumes the ExtendedPrice is without VAT.
        /// </remarks>
        /// <param name="lineItem"></param>
        /// <param name="vatPercent"></param>
        /// <returns></returns>
        private int CalculateVatAmount(LineItem lineItem, int vatPercent)
        {
            // Count: 2	Price: 11.25 VAT%:50.25
            // Product2 = Round (1125 * 2 * (1 + 5025/10000)) = Round (3380.625) = 3381

            throw new NotImplementedException("This method has not been verified. Please test before use.");
            //int priceNormalized = (int)lineItem.ExtendedPrice*100;
            //int vatPercentNormalized = vatPercent*100;
            //// IMPORTANT: The extended price is for all items (quantities)
            //// Hence we MUST NOT multiply by quantity
            //int total = (int)Math.Round((decimal)priceNormalized * (1 + vatPercentNormalized / 10000));
            //return total;
        }

        /// <summary>
        /// Return the total price for a line item without the VAT included
        /// </summary>
        /// <remarks>
        /// Use this to get the total price for 1 or more products without VAT if
        /// your prices include VAT by default.
        /// </remarks>
        /// <param name="lineItem"></param>
        /// <param name="vatPercent"></param>
        /// <returns></returns>
        private decimal GetTotalPriceWithoutVat(LineItem lineItem, int vatPercent)
        {
            decimal vatFactor = 1 + (vatPercent / 100m);
            // IMPORTANT: The extended price is for all items (quantities)
            // Hence we MUST NOT multiply by quantity. DIBS expect us to
            // give the price per item, so we need to divide by quantity)
            decimal linePriceWithoutVat = (lineItem.ExtendedPrice / vatFactor) / lineItem.Quantity;
            // Round, DIBS limits us to 2 decimal values
            return Math.Round(linePriceWithoutVat, 2);
        }


    }
}
