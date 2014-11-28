/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Linq;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;

namespace OxxCommerceStarterKit.Web.Models.ViewModels.Simple
{
    public class ProductVariantViewModel
    {
        readonly VariationContent _variationContent;
        private Price _price;
        private bool _priceIsSet;
        private bool _isAvailable = false;

        public Price Price
        {
            get
            {
                if (_priceIsSet)
                    return _price;

                _priceIsSet = true;
                
                var currentMarketId = ServiceLocator.Current.GetInstance<ICurrentMarket>().GetCurrentMarket().MarketId;

                // Get the first price for the current market with no price code
                ReadOnlyPricingLoader readOnlyPricingLoader = ServiceLocator.Current.GetInstance<ReadOnlyPricingLoader>();
                return _price = _variationContent.GetPrices(readOnlyPricingLoader).FirstOrDefault(p => p.MarketId == currentMarketId && p.CustomerPricing.PriceCode == string.Empty);
            }
        }

        public bool IsAvailableInCurrentMarket
        {
            get { return _variationContent.IsAvailableInCurrentMarket(); ; }
        }

        public bool IsAvailable
        {
            get { return _isAvailable; }
        }

        public ProductVariantViewModel(VariationContent variationContent)
        {
            this._variationContent = variationContent;
            _isAvailable = _variationContent.IsPendingPublish == false && 
                        _variationContent.IsDeleted == false &&
                        _variationContent.IsAvailableInCurrentMarket();
        }
    }
}
