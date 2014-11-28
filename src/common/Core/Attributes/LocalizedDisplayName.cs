/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.ComponentModel;
using System.Globalization;
using System.Threading;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace OxxCommerceStarterKit.Core.Attributes
{
    public class LocalizedDisplayName : DisplayNameAttribute
    {
        private static Injected<LocalizationService> LocalizationService { get; set; }
        private readonly string _resourceName;

        public LocalizedDisplayName(string resourceName)
        {
            _resourceName = resourceName;
        }

        public override string DisplayName
        {
            get { return GetMessageFromResource(_resourceName, Thread.CurrentThread.CurrentCulture); }
        }


        private static string GetMessageFromResource(string resourceId, CultureInfo currentCulture)
        {
            return Translate(resourceId, currentCulture);
        }

        private static string Translate(string resourceKey, CultureInfo currentCulture)
        {
            if (!resourceKey.StartsWith("/"))
            {
                resourceKey = "/" + resourceKey;
            }

            string value;

            if (!LocalizationService.Service.TryGetStringByCulture(resourceKey, currentCulture, out value))
            {
                value = resourceKey;
            }

            return value;
        }
    }
}
