/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections;
using System.Threading;
using System.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.Web;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Website;
using Mediachase.Commerce.Website.Helpers;

namespace OxxCommerceStarterKit.Core.PaymentProviders
{
    internal static class Utilities
    {
        private const string CurrentCartKey = "CurrentCart";
        private const string CurrentContextKey = "CurrentContext";

        /// <summary>
        /// Get display name with current language
        /// </summary>
        /// <param name="item">The line item of oder</param>
        /// <param name="maxSize">The number of character to get display name</param>
        /// <returns>Display name with current language</returns>
        public static string GetDisplayNameOfCurrentLanguage(this LineItem item, int maxSize)
        {
            Entry entry = CatalogContext.Current.GetCatalogEntry(item.CatalogEntryId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
            // if the entry is null (product is deleted), return item display name
            var displayName = (entry != null) ? StoreHelper.GetEntryDisplayName(entry).StripPreviewText(maxSize <= 0 ? 100 : maxSize) : 
                item.DisplayName.StripPreviewText(maxSize <= 0 ? 100 : maxSize);

            return displayName;
        }

        /// <summary>
        /// Update display name with current language
        /// </summary>
        /// <param name="po">The purchase order</param>
        public static void UpdateDisplayNameWithCurrentLanguage(this PurchaseOrder po)
        {
            if (po != null)
            {
                if (po.OrderForms != null && po.OrderForms.Count > 0)
                {
                    foreach (OrderForm orderForm in po.OrderForms)
                    {
                        if (orderForm.LineItems != null && orderForm.LineItems.Count > 0)
                        {
                            foreach (LineItem item in orderForm.LineItems)
                            {
                                item.DisplayName = item.GetDisplayNameOfCurrentLanguage(100);
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Uses parameterized thread to update the cart instance id otherwise will get an "workflow already existed" exception.
        /// Passes the cart and the current HttpContext as parameter in call back function to be able to update the instance id and also can update the HttpContext.Current if needed.
        /// </summary>
        /// <param name="cart">The cart to update.</param>
        /// <remarks>
        /// This method is used internal for payment methods which has redirect standard for processing payment e.g: PayPal, DIBS
        /// </remarks>
        internal static void UpdateCartInstanceId(Cart cart)
        {
            ParameterizedThreadStart threadStart = UpdateCartCallbackFunction;
            var thread = new Thread(threadStart);
            var cartInfo = new Hashtable();
            cartInfo[CurrentCartKey] = cart;
            cartInfo[CurrentContextKey] = HttpContext.Current;
            thread.Start(cartInfo);
            thread.Join();
        }

        /// <summary>
        /// Callback function for updating the cart. Before accept all changes of the cart, update the HttpContext.Current if it is null somehow.
        /// </summary>
        /// <param name="cartArgs">The cart agruments for updating.</param>
        private static void UpdateCartCallbackFunction(object cartArgs)
        {
            var cartInfo = cartArgs as Hashtable;
            if (cartInfo == null || !cartInfo.ContainsKey(CurrentCartKey))
            {
                return;
            }

            var cart = cartInfo[CurrentCartKey] as Cart;
            if (cart != null)
            {
                cart.InstanceId = Guid.NewGuid();
                if (HttpContext.Current == null && cartInfo.ContainsKey(CurrentContextKey))
                {
                    HttpContext.Current = cartInfo[CurrentContextKey] as HttpContext;
                }
                try
                {
                    cart.AcceptChanges();
                }
                catch (System.Exception ex)
                {
                    ErrorManager.GenerateError(ex.Message);
                }
            }
        }

        /// <summary>
        /// Strips a text to a given length without splitting the last word.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="maxLength">Max length of the text</param>
        /// <returns>A shortened version of the given string</returns>
        /// <remarks>Will return empty string if input is null or empty</remarks>
        public static string StripPreviewText(this string source, int maxLength)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            if (source.Length <= maxLength)
            {
                return source;
            }
            source = source.Substring(0, maxLength);
            // The maximum number of characters to cut from the end of the string.
            var maxCharCut = (source.Length > 15 ? 15 : source.Length - 1);
            var previousWord = source.LastIndexOfAny(new char[] { ' ', '.', ',', '!', '?' }, source.Length - 1, maxCharCut);
            if (previousWord >= 0)
            {
                source = source.Substring(0, previousWord);
            }
            return source + " ...";
        }

        public static string GetUrlValueFromStartPage(string propertyName)
        {
            var startPageData = DataFactory.Instance.GetPage(PageReference.StartPage);
            if (startPageData == null)
            {
                return PageReference.StartPage.GetFriendlyUrl();
            }

            string result = string.Empty;
            var property = startPageData.Property[propertyName];
            if (property != null && !property.IsNull)
            {
                if (property.PropertyValueType == typeof(PageReference))
                {
                    var propertyValue = property.Value as PageReference;
                    if (propertyValue != null)
                    {
                        result = propertyValue.GetFriendlyUrl();
                    }
                }
            }
            return string.IsNullOrEmpty(result) ? PageReference.StartPage.GetFriendlyUrl() : result;
        }

        /// <summary>
        /// Gets friendly url of the page.
        /// </summary>
        /// <param name="pageReference">The page reference.</param>
        /// <returns>The friendly url of page if UrlRewriteProvider.IsFurlEnabled</returns>
        public static string GetFriendlyUrl(this PageReference pageReference)
        {
            if (pageReference == null)
            {
                return string.Empty;
            }

            var page = DataFactory.Instance.GetPage(pageReference);

            if (UrlRewriteProvider.IsFurlEnabled)
            {
                //var urlBuilder = new UrlBuilder(new Uri(page.LinkURL, UriKind.RelativeOrAbsolute));
                var url = UriSupport.AddLanguageSelection(page.LinkURL, ContentLanguage.PreferredCulture.Name);

                UrlBuilder urlBuilder = new UrlBuilder(UriSupport.AbsoluteUrlBySettings(url));
                Global.UrlRewriteProvider.ConvertToExternal(urlBuilder, page.PageLink, System.Text.Encoding.UTF8);
                return urlBuilder.ToString();
            }
            else
            {
                return page.LinkURL;
            }
        }
    }
}
