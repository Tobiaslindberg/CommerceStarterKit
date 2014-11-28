/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Configuration;
using EPiServer.GoogleAnalytics.Web.Tracking;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;

namespace OxxCommerceStarterKit.Web.Business.Analytics
{
    /// <summary>
    /// Tells Google Anayltics to inject the Enhanced Ecommerce script
    /// and use the currency for the current market 
    /// </summary>
    // TODO: Enable if GA add-on will support more than one IPluginScript
    // For now, we register it using an IInitializableModule
    // [ServiceConfiguration(typeof(IPluginScript))]
    public class RequireEnhancedCommercePlugin : IPluginScript
    {
        public string GetScript()
        {
            ICurrentMarket currentMarket = ServiceLocator.Current.GetInstance<ICurrentMarket>();
            IMarket market = currentMarket.GetCurrentMarket();


            string script = "ga('require', 'ec');\n";
            script = script + string.Format("ga('set', '&cu', '{0}');", market.DefaultCurrency);
            return script;

        }
    }
}
