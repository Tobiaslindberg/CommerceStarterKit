/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace OxxCommerceStarterKit.Core.Attributes
{
    public class PlaceholderAttribute : MetadataAttribute
    {
        //http://weblogs.asp.net/seanmcalinden/archive/2010/06/12/asp-net-mvc-2-auto-complete-textbox-custom-view-model-attribute-amp-editortemplate.aspx

        private static Injected<LocalizationService> LocalizationService { get; set; }
        private readonly string _resourceName;

        public PlaceholderAttribute(string resourceName)
        {
            _resourceName = resourceName;
        }

        public override void Process(ModelMetadata modelMetaData)
        {
            modelMetaData.AdditionalValues.Add(Constants.ViewData.PlaceholderData, GetMessageFromResource(_resourceName, Thread.CurrentThread.CurrentCulture));
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
