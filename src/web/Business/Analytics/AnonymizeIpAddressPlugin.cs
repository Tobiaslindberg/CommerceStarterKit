/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Configuration;
using EPiServer.GoogleAnalytics.Web.Tracking;
using EPiServer.ServiceLocation;

namespace OxxCommerceStarterKit.Web.Business.Analytics
{
    /// <summary>
    /// Tells Google Anayltics to anonymize IP address if the
    /// appSetting GoogleAnalytics.AnonymizeIpAddress is set to
    /// true.
    /// </summary>
    
    // TODO: Enable when GA add-on supports more than one IPluginScript
    // [ServiceConfiguration(typeof(IPluginScript))]
    public class AnonymizeIpAddressPlugin : IPluginScript
    {
        public string GetScript()
        {
            string settingString = ConfigurationManager.AppSettings["GoogleAnalytics.AnonymizeIpAddress"];
            bool useAnonIp;
            if (bool.TryParse(settingString, out useAnonIp))
            {
                if (useAnonIp)
                {
                    return "ga('set', 'anonymizeIp', true);";
                }
            }
            return string.Empty;
        }
    }
}
