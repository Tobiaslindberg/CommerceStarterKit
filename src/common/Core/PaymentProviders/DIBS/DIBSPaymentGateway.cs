/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Specialized;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using EPiServer;
using EPiServer.Core;
using Mediachase.Commerce;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Plugins.Payment;

namespace OxxCommerceStarterKit.Core.PaymentProviders.DIBS
{
    public class DIBSPaymentGateway : AbstractPaymentGateway
    {
        public const string UserParameter = "MerchantID";
        public const string PasswordParameter = "Password";
        public const string ProcessingUrl = "ProcessingUrl";
        public const string KeyParameter = "Key";
        public const string InnerKeyParameter = "InnerKey";
        public const string OuterKeyParameter = "OuterKey";

        public const string MD5Key1 = "MD5Key1";
        public const string MD5Key2 = "MD5Key2";

        public const string DKK = "208";
        public const string EUR = "978";
        public const string USD = "840";
        public const string GBP = "826";
        public const string SEK = "752";
        public const string AUD = "036";
        public const string CAD = "124";
        public const string ISK = "352";
        public const string JPY = "392";
        public const string NZD = "554";
        public const string NOK = "578";
        public const string CHF = "756";
        public const string TRY = "949";
        public const string PaymentCompleted = "DIBS payment completed";

        private string _merchant;
        private string _password;
        private PaymentMethodDto _payment;
        private static string _key;
        private string _innerkey;
        private string _outerkey;

        /// <summary>
        /// Processes the payment.
        /// </summary>
        /// <param name="payment">The payment.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public override bool ProcessPayment(Mediachase.Commerce.Orders.Payment payment, ref string message)
        {

            // We need this for some of the properties on this class that loads
            // payment method parameters (like MDS keys)
            _payment = PaymentManager.GetPaymentMethod(payment.PaymentMethodId); // .GetPaymentMethodBySystemName("DIBS", SiteContext.Current.LanguageName);

            if (payment.Parent.Parent is PurchaseOrder)
            {
                if (payment.TransactionType == TransactionType.Capture.ToString())
                {
                    //return true meaning the capture request is done,
                    //actual capturing must be done on DIBS.
                    string result = PostCaptureRequest(payment);
                    //result containing ACCEPTED means the the request was successful
                    if (result.IndexOf("ACCEPTED") == -1)
                    {
                        message = "There was an error while capturing payment with DIBS";
                        return false;
                    }
                    return true;
                }

                if (payment.TransactionType == TransactionType.Credit.ToString())
                {
                    var transactionID = payment.TransactionID;
                    if (string.IsNullOrEmpty(transactionID) || transactionID.Equals("0"))
                    {
                        message = "TransactionID is not valid or the current payment method does not support this order type.";
                        return false;
                    }
                    //The transact must be captured before refunding
                    string result = PostRefundRequest(payment);
                    if (result.IndexOf("ACCEPTED") == -1)
                    {
                        message = "There was an error while refunding with DIBS";
                        return false;
                    }
                    return true;
                }
                //right now we do not support processing the order which is created by Commerce Manager
                message = "The current payment method does not support this order type.";
                return false;
            }

            Cart cart = payment.Parent.Parent as Cart;
            if (cart != null && cart.Status == PaymentCompleted)
            {
                //return true because this shopping cart has been paid already on DIBS
                return true;
            }

            if (HttpContext.Current != null)
            {
                var pageRef = DataFactory.Instance.GetPage(PageReference.StartPage)["DIBSPaymentPage"] as PageReference;
                PageData page = DataFactory.Instance.GetPage(pageRef);
                HttpContext.Current.Response.Redirect(page.LinkURL);
            }
            else
            {
                throw new NullReferenceException("Cannot redirect to payment page without Http Context");
            }

            return true;
        }

        /// <summary>
        /// Posts the request to DIBS API.
        /// </summary>
        /// <param name="payment">The payment.</param>
        /// <param name="url">The URL.</param>
        /// <returns>A string contains result from DIBS API</returns>
        private string PostRequest(Mediachase.Commerce.Orders.Payment payment, string url)
        {
            WebClient webClient = new WebClient();
            NameValueCollection request = new NameValueCollection();
            PurchaseOrder po = payment.Parent.Parent as PurchaseOrder;
            string orderid = po.TrackingNumber;
            string transact = payment.TransactionID;
            string amount = (payment.Amount * 100).ToString();
            request.Add("merchant", Merchant);
            request.Add("transact", transact);
            request.Add("amount", amount);

            request.Add("currency", payment.Parent.Parent.BillingCurrency);
            request.Add("orderId", orderid);
            string md5 = GetMD5KeyRefund(Merchant, orderid, transact, amount, _payment);
            request.Add("md5key", md5);
            request.Add("force", "yes");
            request.Add("textreply", "yes");
            webClient.Credentials = new NetworkCredential(Merchant, Password);
            byte[] responseArray = webClient.UploadValues(url, "POST", request);
            return Encoding.ASCII.GetString(responseArray);
        }

        /// <summary>
        /// Posts the capture request to DIBS API.
        /// </summary>
        /// <param name="payment">The payment.</param>
        /// <returns>Return string from DIBS API</returns>
        private string PostCaptureRequest(Mediachase.Commerce.Orders.Payment payment)
        {
            return PostRequest(payment, "https://payment.architrade.com/cgi-bin/capture.cgi");
        }

        /// <summary>
        /// Posts the refund request to DIBS API.
        /// </summary>
        /// <param name="payment">The payment.</param>
        private string PostRefundRequest(Mediachase.Commerce.Orders.Payment payment)
        {
            return PostRequest(payment, "https://payment.architrade.com/cgi-adm/refund.cgi");
        }

        /// <summary>
        /// Gets the payment.
        /// </summary>
        /// <value>The payment.</value>
        public PaymentMethodDto Payment
        {
            get
            {
                if (_payment == null)
                {
                    _payment = PaymentManager.GetPaymentMethodBySystemName("DIBS", SiteContext.Current.LanguageName);
                }
                return _payment;
            }
        }

        /// <summary>
        /// Gets the merchant.
        /// </summary>
        /// <value>The merchant.</value>
        public string Merchant
        {
            get
            {
                if (String.IsNullOrEmpty(_merchant))
                {
                    _merchant = GetParameterByName(Payment, DIBSPaymentGateway.UserParameter).Value;
                }
                return _merchant;
            }
        }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password
        {
            get
            {
                if (String.IsNullOrEmpty(_password))
                {
                    _password = GetParameterByName(Payment, DIBSPaymentGateway.PasswordParameter).Value;
                }
                return _password;
            }
        }

        public string Key
        {
            get
            {
                if (string.IsNullOrEmpty(_key))
                {
                    _key = GetParameterByName(Payment, KeyParameter).Value;
                }
                return _key;
            }
        }

        public string InnerKey
        {
            get
            {
                if (string.IsNullOrEmpty(_innerkey))
                {
                    _innerkey = GetParameterByName(Payment, InnerKeyParameter).Value;
                }
                return _innerkey;
            }
        }

        public string OuterKey
        {
            get
            {
                if (string.IsNullOrEmpty(_outerkey))
                {
                    _outerkey = GetParameterByName(Payment, OuterKeyParameter).Value;
                }
                return _outerkey;
            }
        }




        /// <summary>
        /// Gets the M d5 key refund.
        /// </summary>
        /// <param name="merchant">The merchant.</param>
        /// <param name="orderId">The order id.</param>
        /// <param name="transact">The transact.</param>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public static string GetMD5KeyRefund(string merchant, string orderId, string transact, string amount, PaymentMethodDto payment)
        {
            string hashString = string.Format("merchant={0}&orderid={1}&transact={2}&amount={3}", merchant,
                                                        orderId, transact, amount);
            return GetMD5Key(hashString, payment);
        }

        /// <summary>
        /// Gets the MD5 key used to send to DIBS in authorization step.
        /// </summary>
        /// <param name="merchant">The merchant.</param>
        /// <param name="orderId">The order id.</param>
        /// <param name="currency">The currency.</param>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public static string GetMD5Key(string merchant, string orderId, Currency currency, string amount, PaymentMethodDto payment)
        {
            string hashString = string.Format("merchant={0}&orderid={1}&currency={2}&amount={3}", merchant,
                                                        orderId, currency.CurrencyCode, amount);
            return GetMD5Key(hashString, payment);
        }


        /// <summary>
        /// Gets the key used to verify response from DIBS when payment is approved.
        /// </summary>
        /// <param name="transact">The transact.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        public static string GetMD5Key(string transact, string amount, Currency currency, PaymentMethodDto payment)
        {
            string hashString = string.Format("transact={0}&amount={1}&currency={2}", transact, amount, GetCurrencyCode(currency));
            return GetMD5Key(hashString, payment);
        }

        private static string GetMD5Key(string hashString, PaymentMethodDto payment)
        {
            PaymentMethodDto dibs = payment;
            string key1 = GetParameterByName(dibs, MD5Key1).Value;
            string key2 = GetParameterByName(dibs, MD5Key2).Value;

            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(key1 + hashString);
            bs = x.ComputeHash(bs);
            StringBuilder s = new StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string firstHash = s.ToString();

            string secondHashString = key2 + firstHash;
            byte[] bs2 = System.Text.Encoding.UTF8.GetBytes(secondHashString);
            bs2 = x.ComputeHash(bs2);
            StringBuilder s2 = new StringBuilder();
            foreach (byte b in bs2)
            {
                s2.Append(b.ToString("x2").ToLower());
            }
            string secondHash = s2.ToString();
            return secondHash;
        }

        /// <summary>
        /// Gets the parameter by name.
        /// </summary>
        /// <param name="paymentMethodDto">The payment method dto.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static PaymentMethodDto.PaymentMethodParameterRow GetParameterByName(PaymentMethodDto paymentMethodDto, string name)
        {
            PaymentMethodDto.PaymentMethodParameterRow[] rowArray = (PaymentMethodDto.PaymentMethodParameterRow[])paymentMethodDto.PaymentMethodParameter.Select(string.Format("Parameter = '{0}'", name));
            if ((rowArray != null) && (rowArray.Length > 0))
            {
                return rowArray[0];
            }
            throw new ArgumentNullException("Parameter named " + name + " for DIBS payment cannot be null");
        }


        /// <summary>
        /// Convert the currency code of the site to
        /// the ISO4217 number for that currency for DIBS to understand.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        private static string GetCurrencyCode(Currency currency)
        {
            if (currency.Equals(Currency.DKK))
                return DIBSPaymentGateway.DKK;
            else if (currency.Equals(Currency.AUD))
                return DIBSPaymentGateway.AUD;
            else if (currency.Equals(Currency.CAD))
                return DIBSPaymentGateway.CAD;
            else if (currency.Equals(Currency.CHF))
                return DIBSPaymentGateway.CHF;
            else if (currency.Equals(Currency.EUR))
                return DIBSPaymentGateway.EUR;
            else if (currency.Equals(Currency.GBP))
                return DIBSPaymentGateway.GBP;
            else if (currency.Equals(Currency.ISK))
                return DIBSPaymentGateway.ISK;
            else if (currency.Equals(Currency.JPY))
                return DIBSPaymentGateway.JPY;
            else if (currency.Equals(Currency.NOK))
                return DIBSPaymentGateway.NOK;
            else if (currency.Equals(Currency.NZD))
                return DIBSPaymentGateway.NZD;
            else if (currency.Equals(Currency.SEK))
                return DIBSPaymentGateway.SEK;
            else if (currency.Equals(Currency.TRY))
                return DIBSPaymentGateway.TRY;
            else if (currency.Equals(Currency.USD))
                return DIBSPaymentGateway.USD;
            return string.Empty;
        }
    }
}
