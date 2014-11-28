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
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using OxxCommerceStarterKit.Web.Models.PageTypes;

namespace OxxCommerceStarterKit.Web.Business
{
    public class SiteConfiguration
    {
        private SiteConfiguration()
        {
        }

        public SettingsBlock Settings { get; set; }

        public static SiteConfiguration Current()
        {
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            // This can actually be 0 if we have a problem with our language settings
            if (ContentReference.StartPage != null && ContentReference.StartPage.ID > 0)
            {
                var startPage = contentLoader.Get<HomePage>(ContentReference.StartPage);
                SiteConfiguration config = new SiteConfiguration
                {
                    Settings = startPage.Settings
                };

                return config;
            }
            return new SiteConfiguration();
        }
    }
}
