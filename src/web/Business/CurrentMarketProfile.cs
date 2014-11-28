/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Web;
using System.Web.Profile;
using Mediachase.Commerce;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Markets;

namespace OxxCommerceStarterKit.Web.Business
{
    /// <summary>
    /// Implementation of current market selection that stores information in user profile.
    /// </summary>
    public class CurrentMarketProfile : ICurrentMarket
    {
        private const string _marketIdKey = "MarketId";
        private readonly IMarketService _marketService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentMarketProfile"/> class.
        /// </summary>
        public CurrentMarketProfile()
            : this(EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<IMarketService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentMarketProfile"/> class.
        /// </summary>
        /// <param name="marketService">The market service.</param>
        public CurrentMarketProfile(IMarketService marketService)
        {
            _marketService = marketService;
        }

        /// <summary>
        /// Gets the current market.
        /// </summary>
        /// <returns>current Market</returns>
        public IMarket GetCurrentMarket()
        {
            var currentMarketId = LoadMarketFromProfile();
            if (string.IsNullOrWhiteSpace(currentMarketId))
            {
                currentMarketId = MarketId.Default.Value;
                SetCurrentMarket(currentMarketId);
            }

            // In case we fail to get the current market, fall back to default market
            return GetMarket(new MarketId(currentMarketId)) ?? GetMarket(MarketId.Default);
        }

        /// <summary>
        /// Sets the current market.
        /// </summary>
        /// <param name="marketId">The market id.</param>
        /// <remarks>This will set the current currency for the ECF context too</remarks>
        /// <returns>return the successful set MarketId</returns>
        public void SetCurrentMarket(MarketId marketId)
        {
            var market = GetMarket(marketId);
            if (market == null)
            {
                return;
            }
            SaveMarketInProfile(marketId);
            SiteContext.Current.Currency = market.DefaultCurrency;
            EPiServer.Globalization.ContentLanguage.PreferredCulture = market.DefaultLanguage;
        }

        private IMarket GetMarket(MarketId marketId)
        {
            return _marketService.GetMarket(marketId);
        }

        private string LoadMarketFromProfile()
        {
            if (ProfileStorage == null)
            {
                return null;
            }
            return ProfileStorage[_marketIdKey] as string;
        }

        private void SaveMarketInProfile(MarketId marketId)
        {
            if (ProfileStorage != null)
            {
                ProfileStorage[_marketIdKey] = marketId.Value;
                ProfileStorage.Save();
            }
        }

        private ProfileBase ProfileStorage
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return null;
                }
                return HttpContext.Current.Profile;
            }
        }
    }
}
