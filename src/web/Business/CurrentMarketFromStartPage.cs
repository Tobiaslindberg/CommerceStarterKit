/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Linq;
using System.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Markets;

namespace OxxCommerceStarterKit.Web.Business
{
    /// <summary>
    /// This class gets the current market based on the language of the start page. It has 
    /// smart default handling, so we don't have to do it.
    /// </summary>
    /// <remarks>
    /// If you need to be able to switch market based on other parameters than the site language
    /// you need to implement your own ICurrentMarket (See the <see cref="CurrentMarketProfile"/>
    /// for an example.
    /// </remarks>
    public class CurrentMarketFromStartPage : ICurrentMarket
    {
        private readonly IMarketService _marketService;
        private readonly IContentLoader _contentLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentMarketFromStartPage"/> class.
        /// </summary>
        public CurrentMarketFromStartPage()
            : this(
                ServiceLocator.Current.GetInstance<IMarketService>(),
                ServiceLocator.Current.GetInstance<IContentLoader>()
                )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentMarketFromStartPage"/> class.
        /// </summary>
        /// <param name="marketService">The market service.</param>
        /// <param name="contentLoader">The content loader</param>
        public CurrentMarketFromStartPage(IMarketService marketService, IContentLoader contentLoader)
        {
            _marketService = marketService;
            _contentLoader = contentLoader;
        }

        public IMarket GetCurrentMarket()
        {
            IMarket market = GetCachedMarket();
            if (market != null)
                return market;
			if (ContentReference.StartPage != null && ContentReference.StartPage.ID > 0)
			{
				var startPage = _contentLoader.Get<PageData>(ContentReference.StartPage);
				// startPage.Language
				var allMarkets = _marketService.GetAllMarkets();
				market = allMarkets.FirstOrDefault(m => m.DefaultLanguage.Equals(startPage.Language));
			}
			
            if(market == null)
            {
                // In case we fail to get the current market, fall back to default market
                return GetMarket(MarketId.Default);
            }

            SetCachedMarket(market);

            return market;

        }

        protected IMarket GetCachedMarket()
        {
            if(HttpContext.Current != null && HttpContext.Current.Items["CurrentMarketCache"] != null)
            {
                return HttpContext.Current.Items["CurrentMarketCache"] as IMarket;
            }

            return null;
        }

        protected void SetCachedMarket(IMarket market)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items["CurrentMarketCache"] = market;
            }
        }


        protected IMarket GetMarket(MarketId marketId)
        {
            return _marketService.GetMarket(marketId);
        }


        public void SetCurrentMarket(MarketId marketId)
        {
            // Nope - no can do - we read it from the current context
            throw new NotImplementedException("Cannot set current market, it is resolved from the start page.");
        }
    }
}
