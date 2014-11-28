/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Security;
using Mediachase.Commerce.Website.Helpers;
using Mediachase.Data.Provider;

namespace OxxCommerceStarterKit.Core.PaymentProviders.DIBS
{
    public partial class DIBSPayment : System.Web.UI.Page
    {
        private Cart _currentCart = null;
        public const string SessionLatestOrderIdKey = "LatestOrderId";
        public const string SessionLatestOrderNumberKey = "LatestOrderNumber";
        public const string SessionLatestOrderTotalKey = "LatestOrderTotal";
        public const string DIBSSystemName = "DIBS";
        
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
        }

        private Mediachase.Commerce.Orders.Payment payment;
        private PaymentMethodDto _paymentMethod;
        private string _acceptUrl;
        private string _cancelUrl;
        private string _callbackUrl;
        private string _orderNumber;

        private Injected<LocalizationService> LocalizationService
        {
            get;
            set;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            if (CurrentCart.OrderForms.Count == 0 || CurrentCart.OrderForms[0].Payments.Count == 0)
            {
                return;
            }
            var payments = CurrentCart.OrderForms[0].Payments.ToArray();
            //get DIBS payment by PaymentMethodName, instead of get the first payment in the list, which causes problem when checking out with gift card
            PaymentMethodDto dibs = PaymentManager.GetPaymentMethodBySystemName(DIBSSystemName, SiteContext.Current.LanguageName);
            _paymentMethod = dibs;
            payment = payments.Where(c => c.PaymentMethodId.Equals(dibs.PaymentMethod.Rows[0]["PaymentMethodId"])).FirstOrDefault();
            if (payment == null)
            {
                return;
            }

            string processingUrl = DIBSPaymentGateway.GetParameterByName(dibs, DIBSPaymentGateway.ProcessingUrl).Value;
            string MD5K1 = DIBSPaymentGateway.GetParameterByName(dibs, DIBSPaymentGateway.MD5Key1).Value;
            string MD5K2 = DIBSPaymentGateway.GetParameterByName(dibs, DIBSPaymentGateway.MD5Key2).Value;

            // Get DIBSPaymentLanding url
            if (!Request.IsAuthenticated)
            {
                _acceptUrl = "~/Templates/Sample/Pages/CheckoutConfirmationStep.aspx";
            }
            else
            {
                var referencePage = DataFactory.Instance.GetPage(PageReference.StartPage)["DIBSPaymentLandingPage"] as PageReference;
                var landingPage = DataFactory.Instance.GetPage(referencePage);
                _acceptUrl = landingPage != null ? landingPage.LinkURL : string.Empty;
            }

            _cancelUrl = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : DataFactory.Instance.GetPage(PageReference.StartPage).LinkURL;
            //In case the user cancels the payment, he'll be redirected back to checkout page.
            //We need to set cancel url to post form.
            cancelurl.Value = _cancelUrl;
            
            paymentForm.Action = processingUrl;            

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");

            //Process successful transaction                        
            if (CurrentCart != null && Request.Form["authkey"] != null 
                && Request.Form["amount"] != null && Request.Form["currency"] != null &&
                MD5K1 != null && MD5K2 != null
                && Request.Form["orderid"] != null && Request.Form["transact"] != null)
            {
                _orderNumber = Request["orderid"];
                string myHash = DIBSPaymentGateway.GetMD5Key(Request.Form["transact"], Request.Form["amount"],
                                                new Currency(Request.Form["currency"]), dibs);
                if (myHash.Equals(Request.Form["authkey"]))
                {
                    ProcessSuccessfulTransaction();
                }
            }
            //Process unsuccessful transaction
            else if (CurrentCart != null && Request.Form["merchant"] != null && Request.Form["orderid"] != null
                && Request.Form["amount"] != null && Request.Form["currency"] != null &&
                Request.Form["md5key"] != null)
            {
                string myHash = DIBSPaymentGateway.GetMD5Key(Request.Form["merchant"], Request.Form["orderid"],
                                                new Currency(Request.Form["currency"]), Request.Form["amount"], dibs);
                if (myHash.Equals(Request.Form["md5key"]))
                {
                    ProcessUnsuccessfulTransaction(string.Empty);
                }
            }
            
            if (payment == null)
                Response.Redirect(_cancelUrl);
            else if (payment != null)
            {
                Cart cart = payment.Parent.Parent as Cart;
                // process request from Checkout
                _orderNumber = GenerateOrderNumber(cart);

                Utilities.UpdateCartInstanceId(cart);
            }
            Page.ClientScript.RegisterClientScriptBlock(typeof(DIBSPayment), "submitScript", "window.onload = function (evt) { document.forms[0].submit(); }", true);
            base.OnLoad(e);
        }

        /// <summary>
        /// Processes the successful transaction.
        /// </summary>
        private void ProcessSuccessfulTransaction()
        {
            PurchaseOrder po = null;
            
            Cart cart = CurrentCart;
            string email = new CartHelper(cart).PrimaryAddress.Email;
            if (cart != null)
            {
                // Make sure to execute within transaction
                using (TransactionScope scope = new TransactionScope())
                {
                    cart.Status = DIBSPaymentGateway.PaymentCompleted;
                    
                    // Change status of payments to processed.
                    // It must be done before execute workflow to ensure payments which should mark as processed.
                    // To avoid get errors when executed workflow.
                    PaymentStatusManager.ProcessPayment(payment);

                    // Execute CheckOutWorkflow with parameter to ignore running process payment activity again.
                    var isIgnoreProcessPayment = new Dictionary<string, object>();
                    isIgnoreProcessPayment.Add("IsIgnoreProcessPayment", true);
                    OrderGroupWorkflowManager.RunWorkflow(cart, OrderGroupWorkflowManager.CartCheckOutWorkflowName, true, isIgnoreProcessPayment);
                    
                    //Save the transact from DIBS to payment.
                    payment.TransactionID = Request["transact"];
                    payment.AcceptChanges();

                    // Save changes
                    cart.OrderNumberMethod = new Cart.CreateOrderNumber((c) => _orderNumber);
                    //this might cause problem when checkout using multiple shipping address because ECF workflow does not handle it. Modify the workflow instead of modify in this payment
                    po = cart.SaveAsPurchaseOrder();
                    if (CustomerContext.Current.CurrentContact != null)
                    {
                        CustomerContext.Current.CurrentContact.LastOrder = po.Created;
                        CustomerContext.Current.CurrentContact.SaveChanges();
                    }

                    AddNoteToPurchaseOrder("New order placed by {0} in {1}", po, SecurityContext.Current.CurrentUserName, "Public site");                

                    // Remove old cart
                    cart.Delete();
                    cart.AcceptChanges();

                    // Update display name of product by current language
                    po.UpdateDisplayNameWithCurrentLanguage();

                    po.AcceptChanges();

                    // Commit changes
                    scope.Complete();

                    if (HttpContext.Current.Session != null)
                    {
                        HttpContext.Current.Session.Remove("LastCouponCode");
                    }
                }
                _acceptUrl = UriSupport.AddQueryString(_acceptUrl, "success", "true");
                _acceptUrl = UriSupport.AddQueryString(_acceptUrl, "ordernumber", po.TrackingNumber);
                _acceptUrl = UriSupport.AddQueryString(_acceptUrl, "email", email);
                Response.Redirect(_acceptUrl, true);
            }
            else
            {
                Response.Redirect(_cancelUrl, true);
            }
        }

        /// <summary>
        /// Adds the note to purchase order.
        /// </summary>
        /// <param name="note">Name of the note.</param>
        /// <param name="purchaseOrder">The purchase order.</param>
        /// <param name="parmeters">The parmeters.</param>
        protected void AddNoteToPurchaseOrder(string note, PurchaseOrder purchaseOrder, params string[] parmeters)
        {
            var noteDetail = String.Format(note, parmeters);
            OrderNotesManager.AddNoteToPurchaseOrder(purchaseOrder, noteDetail, OrderNoteTypes.System, SecurityContext.Current.CurrentUserId);
        }

        private void ProcessUnsuccessfulTransaction(string error)
        {
            Response.Redirect(_cancelUrl, true);
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
        /// Generates the order number.
        /// </summary>
        /// <param name="cart">The cart.</param>
        /// <returns></returns>
        private string GenerateOrderNumber(Cart cart)
        {
            string str = new Random().Next(100, 999).ToString();
            return string.Format("PO{0}{1}", cart.OrderGroupId, str);
        }

        /// <summary>
        /// Gets the merchant ID.
        /// </summary>
        /// <value>The merchant ID.</value>
        public string MerchantID
        {
            get
            {
                PaymentMethodDto dibs = PaymentManager.GetPaymentMethodBySystemName("DIBS", SiteContext.Current.LanguageName);
                return DIBSPaymentGateway.GetParameterByName(dibs, DIBSPaymentGateway.UserParameter).Value;
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
                return (payment != null) ? (payment.Amount*100).ToString("#") : string.Empty;
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
                if (payment == null)
                {
                    return string.Empty;
                }
                return string.IsNullOrEmpty(payment.Parent.Parent.BillingCurrency) ?
                    SiteContext.Current.Currency : new Currency(payment.Parent.Parent.BillingCurrency);
            }
        }

        /// <summary>
        /// Gets the order ID.
        /// </summary>
        /// <value>The order ID.</value>
        public string OrderID
        {
            get
            {
                return _orderNumber;
            }
        }

        /// <summary>
        /// Gets the callback URL, which is used by DIBS to call back when
        /// transaction is approved.
        /// </summary>
        /// <value>The callback URL.</value>
        public string CallbackUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_callbackUrl))
                {
                    _callbackUrl = Utilities.GetUrlValueFromStartPage("DIBSPaymentPage");
                }
                return _callbackUrl;
            }
        }

        /// <summary>
        /// Convert the site language to the language which DIBS can support.
        /// </summary>
        /// <value>The language.</value>
        public string Language
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

    }
}
