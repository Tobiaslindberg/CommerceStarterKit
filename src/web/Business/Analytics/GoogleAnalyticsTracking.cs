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
using EPiCode.GoogleAnalyticsTracking;
using EPiCode.GoogleAnalyticsTracking.FieldObjects;
using EPiServer.GoogleAnalytics.Commerce.Helpers;
using EPiServer.GoogleAnalytics.Web.Tracking.Ecommerce;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Orders;

namespace OxxCommerceStarterKit.Web.Business.Analytics
{
    public class GoogleAnalyticsTracking
    {
        public HttpContextBase Context { get; set; }

        public GoogleAnalyticsTracking(HttpContextBase context)
        {
            Context = context;
        }

        /// <summary>
        /// Tracks one product impression using the addImpression command for
        /// Google Analytics. See https://developers.google.com/analytics/devguides/collection/analyticsjs/enhanced-ecommerce#product-impression
        /// </summary>
        /// <example>
        /// Example on how the script code will look like
        /// ga('ec:addImpression', {
        ///   'id': 'P12345',                   // Product details are provided in an impressionFieldObject.
        ///   'name': 'Android Warhol T-Shirt',
        ///   'category': 'Apparel/T-Shirts',
        ///   'brand': 'Google',
        ///   'variant': 'black',
        ///   'list': 'Search Results',
        ///   'position': 1                     // 'position' indicates the product position in the list.
        /// });
        /// </example>
        /// <seealso cref="https://developers.google.com/analytics/devguides/collection/analyticsjs/enhanced-ecommerce#product-impression"/>
        public void ProductImpression(EPiServer.Commerce.Catalog.ContentTypes.EntryContentBase product)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("id", product.Code);
            dict.Add("name", product.DisplayName);

            EPiServer.GoogleAnalytics.Helpers.Extensions.AddAnalyticsCustom(Context, "addImpression", true, dict);
        }

        public void ProductImpression(string code, string name, string category = null, string brand = null, string variant = null, string list = null, int position = 0)
        {
            Tracking tracking = new Tracking();
            string script = tracking.TrackProductImpression(
                code: code, name: name,
                category: category, brand: brand, 
                variant: variant, list: list, 
                position: position);

            AddInteraction(script);
        }

        public void ProductAdd(string code, string name, string category = null, string brand = null, string variant = null, string coupon = null, int position = 0, double price = 0, int quantity = 0)
        {
            Tracking tracking = new Tracking();
            string script = tracking.TrackProductAdd(
                code: code, 
                name: name,
                category: category, 
                brand: brand, 
                variant: variant,  
                position: position,
                coupon: coupon,
                price: price,
                quantity: quantity);

            AddInteraction(script);
        }

        /// <summary>
        /// Tracks a purchase using the setAction and the purchase command.
        /// </summary>
        /// <remarks>
        /// You should use <seealso cref="ProductAdd"/> in additon to this, to register all
        /// the products that is part of the purchase
        /// </remarks>
        /// <see cref="https://developers.google.com/analytics/devguides/collection/analyticsjs/enhanced-ecommerce#measuring-transactions"/>
        public void Purchase(string trackingNumber, string affiliation = null, double revenue = 0, double tax = 0, double shipping = 0, string coupon = null)
        {
            /* The actionFieldObject that is part of the purchase
              'id': 'T12345',                         // (Required) Transaction id (string).
              'affiliation': 'Google Store - Online', // Affiliation (string).
              'revenue': '37.39',                     // Revenue (currency).
              'tax': '2.85',                          // Tax (currency).
              'shipping': '5.34',                     // Shipping (currency).
              'coupon': 'SUMMER2013' 
             */
            ActionFieldObject actionField = new ActionFieldObject()
            {
                Id = trackingNumber,
                Affiliation = affiliation,
                Revenue = revenue,
                Tax = tax,
                Shipping = shipping,
                Coupon = coupon
            };

            // ga('ec:setAction', 'purchase', { ... });
            Action("purchase", actionField.ToString());

        }

        public void Action(string action, string fieldObject = null)
        {
            Tracking tracking = new Tracking();
            string script = tracking.SetAction(action, fieldObject);

            AddInteraction(script);
        }

        public void Require(string library)
        {
            Tracking tracking = new Tracking();
            string script = tracking.Require(library);

            AddInteraction(script);
        }

        /// <summary>
        /// Clears the list of registered interactions.
        /// </summary>
        public void ClearInteractions()
        {
            var interactions = EPiServer.GoogleAnalytics.Helpers.Extensions.GetInteractions(Context);
            interactions.Clear();
        }

        /// <summary>
        /// Adds custom script.
        /// </summary>
        /// <param name="script">The script.</param>
        public void Custom(string script)
        {
            AddInteraction(script);
        }

        protected void AddInteraction(string script)
        {
            var interactions = EPiServer.GoogleAnalytics.Helpers.Extensions.GetInteractions(Context);
            UniversalAnalyticsInteraction productImpression = new UniversalAnalyticsInteraction(script);
            interactions.Add(productImpression);
        }

        public void TrackPurchaseOrder(string trackingNumber)
        {
            CustomerContext customerContext = CustomerContext.Current;
            if (customerContext != null && customerContext.CurrentContactId != Guid.Empty)
            {
                PurchaseOrder purchaseOrder = OrderContext.Current.GetPurchaseOrders(customerContext.CurrentContactId)
                                                                  .FirstOrDefault(po => po.TrackingNumber == trackingNumber);
                if (purchaseOrder != null)
                {
                    Transaction trans = purchaseOrder.ToEcommerceTransaction();

                    // TODO: Remove demo data
                    // For demo purposes, we need some sensible defaults
                    if (string.IsNullOrEmpty(trans.Country))
                    {
                        trans.Country = "Norway";
                    }
                    if (string.IsNullOrEmpty(trans.City))
                    {
                        trans.Country = "Oslo";
                    }

                    // Register tracking data with the Google Analytics add-on
                    // Note! This uses the _trackTrans method
                    // If we're using Enhanced Ecommerce, we 
                    // E
                    // Context.AddAnalyticsTransaction(trans);
                }
            }
        }

    }
}
