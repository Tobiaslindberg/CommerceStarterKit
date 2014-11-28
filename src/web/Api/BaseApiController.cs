/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Globalization;
using System.Threading;
using System.Web.Http;
using EPiServer.Find;
using EPiServer.Globalization;

namespace OxxCommerceStarterKit.Web.Api
{
    public class BaseApiController : ApiController
    {
        protected string _language = null;

        public string Language
        {
            get
            {
                if (_language == null)
                {
                    if (ControllerContext.RouteData.Values["language"] != null)
                    {
                        _language = ControllerContext.RouteData.Values["language"].ToString();
                    }

                    if (string.IsNullOrEmpty(_language))
                    {
                        _language = ContentLanguage.PreferredCulture.Name;

                    }
                }

                return _language;
            }
            set
            {
                _language = value;
            }
        }

        public void SetLanguage()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Language);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Language);
            EPiServer.BaseLibrary.Context.Current["EPiServer:ContentLanguage"] = new CultureInfo(Language);
        }

        public static Language GetLanguage(string languageCode)
        {
            switch (languageCode)
            {
                case "en":
                    return EPiServer.Find.Language.English;
                case "no":
                    return EPiServer.Find.Language.Norwegian;
                default:
                    return EPiServer.Find.Language.English;
            }
        }

    }
}
