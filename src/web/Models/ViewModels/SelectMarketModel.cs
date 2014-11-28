/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Markets;

namespace OxxCommerceStarterKit.Web.Models.ViewModels
{
    public class SelectMarketModel
    {
        private readonly Injected<IMarketService> _marketService;
        private readonly Injected<ICurrentMarket> _currentMarket;

        public IEnumerable<IMarket> Markets
        {
            get { return _marketService.Service.GetAllMarkets(); }
        }

        public bool IsCurrentMarket(IMarket market)
        {
            return market.MarketId == _currentMarket.Service.GetCurrentMarket().MarketId;
        }
    }
}
