/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.ServiceLocation;
using log4net.Util;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Engine;
using Mediachase.Commerce.Pricing;
using Mediachase.Commerce.Website.Helpers;
using OxxCommerceStarterKit.Core.Models;
using Price = EPiServer.Commerce.SpecializedProperties.Price;

namespace OxxCommerceStarterKit.Core.Extensions
{
    public static class PriceExtensions
    {

        public static int GetDefaultPriceAmount(this VariationContent variation)
        {
            ICurrentMarket currentMarket = ServiceLocator.Current.GetInstance<ICurrentMarket>();
            return GetDefaultPriceAmount(variation, currentMarket.GetCurrentMarket());
        }

        public static int GetDefaultPriceAmount(this VariationContent variation, IMarket market = null)
        {
            Price price = variation.GetDefautPriceMoney(market);
            return price != null ? decimal.ToInt32(price.UnitPrice.Amount) : 0;
        }

        public static int GetDefaultPriceAmount(this List<VariationContent> variations, IMarket market = null)
        {
            Price price = variations.GetDefautPriceMoney(market);
            return price != null ? decimal.ToInt32(price.UnitPrice.Amount) : 0;
        }

        public static Price GetDefaultPrice(this VariationContent variation, IMarket market = null)
        {
            return variation.GetDefautPriceMoney(market);
        }

        public static Price GetDefaultPrice(this List<VariationContent> variations, IMarket market = null)
        {
            if (variations.Any())
            {
                return variations.FirstOrDefault().GetDefaultPrice(market);
            }
            return null;
        }

 
        public static string GetDisplayPrice(this VariationContent variation, IMarket market = null)
        {
            Price price = variation.GetDefautPriceMoney(market);
            return price != null ? price.UnitPrice.ToString() : string.Empty;
        }

        public static string GetDisplayPrice(this List<VariationContent> variations, IMarket market = null)
        {
            if (variations.Any())
            {
                return variations.FirstOrDefault().GetDisplayPrice(market);
            }
            return null;
        }

        public static Price GetDefautPriceMoney(this VariationContent variation, IMarket market = null)
        {
            ReadOnlyPricingLoader pricingLoader = ServiceLocator.Current.GetInstance<ReadOnlyPricingLoader>();

            ItemCollection<Price> prices;

            prices = variation.GetPrices();
          
            if (prices != null)
            {
                if(market != null)
                    return prices.FirstOrDefault(x => x.MarketId.Equals(market.MarketId) && x.UnitPrice.Currency == market.DefaultCurrency && x.CustomerPricing.PriceTypeId == CustomerPricing.PriceType.AllCustomers);
                return prices.FirstOrDefault(x => x.CustomerPricing.PriceTypeId == CustomerPricing.PriceType.AllCustomers);
            }
            return null;

        }

        public static Price GetDefautPriceMoney(this List<VariationContent> variations, IMarket market = null)
        {

            if (variations.Any())
            {
                List<Price> prices = variations.Select(variant => GetDefautPriceMoney(variant, market)).Where(x => x != null).ToList();

                if (prices.Any())
                {
                    return prices.FirstOrDefault();
                }
            }
            return null;

        }

        /// <summary>
        /// Gets the discount price, if no discount is set, returns string.Empty
        /// </summary>
        /// <param name="variation">The variation.</param>
        /// <param name="defaultPrice">The price to compare against</param>
        /// <param name="market">The market.</param>
        /// <returns></returns>
        public static string GetDiscountDisplayPrice(this VariationContent variation, Price defaultPrice, IMarket market = null)
        {

            if (market == null)
            {
                ICurrentMarket currentMarket = ServiceLocator.Current.GetInstance<ICurrentMarket>();
                market = currentMarket.GetCurrentMarket();
            }
            Func<PriceAndMarket, bool> priceFilter = d => d.PriceCode != string.Empty &&
                    !(d.PriceTypeId == Mediachase.Commerce.Pricing.CustomerPricing.PriceType.PriceGroup.ToString() &&
                        d.PriceCode == Constants.CustomerGroup.CustomerClub);


            var prices = variation.GetPricesWithMarket(market).FirstOrDefault(priceFilter);


            if (prices != null) return prices.Price;
            return string.Empty;


            
            //if (defaultPrice == null)
            //    return string.Empty;
            //Price price;
            //if(market == null)
            //    price = StoreHelper.GetDiscountPrice(variation.LoadEntry(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
            //else
            //    price = StoreHelper.GetDiscountPrice(variation.LoadEntry(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull),string.Empty,string.Empty,market);
            
            //if(price == null)
            //{
            //    return string.Empty;
            //}

            //if (price.Money == defaultPrice.Money)
            //    return string.Empty;

            //return price.Money.Amount.ToString();
        }

        /// <summary>
        /// Gets the discount price, if no discount is set, returns string.Empty
        /// </summary>
        /// <param name="variations">All variants for a product</param>
        /// <param name="defaultPrice">The price to compare against</param>
        /// <param name="market">The market.</param>
        /// <returns></returns>
        public static string GetDiscountDisplayPrice(this List<VariationContent> variations, Price defaultPrice, IMarket market = null)
        {
            if (variations.Any())
            {
                VariationContent variationContent = variations.FirstOrDefault(x => x.GetPricesWithMarket(market) != null);
                return variationContent.GetDiscountDisplayPrice(defaultPrice, market);
            }
            return string.Empty;
        }


        //// TODO: MOVE TO EXTENSION METHODS FOR RE-USE
        //public static string GetDiscountedPrice(this List<VariationContent> variations, Price defaultPrice, IMarket market = null)
        //{
        //    // TODO: GetDiscountedPrice in find index: improvement point?? this gets the members club price in the search result as "the first variation that has a price with a pricecode"
        //    if (variations.Any())
        //    {
        //        // Has price code, but is not for customer club
        //        Func<PriceAndMarket, bool> priceFilter = d => d.PriceCode != string.Empty &&
        //                !(d.PriceTypeId == Mediachase.Commerce.Pricing.CustomerPricing.PriceType.PriceGroup.ToString() &&
        //                  d.PriceCode == Constants.CustomerGroup.CustomerClub);

        //        List<VariationContent> variationsWithPrices = variations.Where(x => x.GetPricesWithMarket(market) != null).ToList();

        //        if (variationsWithPrices.Any())
        //        {
        //            VariationContent variation = variationsWithPrices.FirstOrDefault();
        //            var price = variation.GetPricesWithMarket(market).FirstOrDefault(priceFilter);
        //            if (price != null) return price.Price;
        //            return string.Empty;
        //        }

        //    }
        //    return string.Empty;
        //}

        public static string GetCustomerClubDisplayPrice(this VariationContent variation, IMarket market = null)
        {
            if (market == null)
            {
                ICurrentMarket currentMarket = ServiceLocator.Current.GetInstance<ICurrentMarket>();
                market = currentMarket.GetCurrentMarket();
            }
            Func<PriceAndMarket, bool> priceFilter = d => d.PriceCode != string.Empty &&
                    (d.PriceTypeId == CustomerPricing.PriceType.PriceGroup.ToString() &&
                        d.PriceCode == Constants.CustomerGroup.CustomerClub);

           
            var prices = variation.GetPricesWithMarket(market).FirstOrDefault(priceFilter);


            if (prices != null) return prices.Price;
            return string.Empty;
        }

        // TODO: MOVE TO EXTENSION METHODS FOR RE-USE
        public static string GetCustomerClubDisplayPrice(this List<VariationContent> variations, IMarket market = null)
        {
            if (variations.Any())
            {
                VariationContent variationContent = variations.FirstOrDefault(x => x.GetPricesWithMarket(market) != null);
                return variationContent.GetCustomerClubDisplayPrice(market);
            }
            return string.Empty;
        }
    }

}
