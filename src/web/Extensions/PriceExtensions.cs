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
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Markets;
using OxxCommerceStarterKit.Core.Extensions;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Extensions
{
    public static class PriceExtensions
    {
        private static Injected<ReadOnlyPricingLoader> injectedPriceLoader; 
        private static Injected<ICurrentMarket> injectedMarketService; 

        public static PriceModel GetPriceModel(this VariationContent currentContent)
        {
            PriceModel priceModel = new PriceModel();
            priceModel.Price = GetPrice(currentContent);
            priceModel.DiscountDisplayPrice = currentContent.GetDiscountDisplayPrice(currentContent.GetDefaultPrice());
            priceModel.CustomerClubDisplayPrice = currentContent.GetCustomerClubDisplayPrice();
            return priceModel;
        }

        public static Price GetPrice(IPricing pricing)
        {            
            return pricing.GetPrices(injectedPriceLoader.Service).FirstOrDefault(x => x.MarketId == injectedMarketService.Service.GetCurrentMarket().MarketId);
        }

    }
}
