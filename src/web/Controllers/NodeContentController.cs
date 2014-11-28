/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Globalization;
using EPiServer.Web.Mvc;

namespace OxxCommerceStarterKit.Web.Controllers
{
    [TemplateDescriptor(Inherited = true)]
    public class NodeContentController : ContentController<NodeContent>
    {
        public string Language
        {
            get
            {
                string language = null;
                if (ControllerContext.RouteData.Values["language"] != null)
                {
                    language = ControllerContext.RouteData.Values["language"].ToString();
                }

                if (string.IsNullOrEmpty(language))
                {
                    language = ContentLanguage.PreferredCulture.Name;
                }

                return language;
            }
        }

        public void SetLanguage()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Language);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Language);
            EPiServer.BaseLibrary.Context.Current["EPiServer:ContentLanguage"] = new CultureInfo(Language);
        }

        public ActionResult Index(NodeContent currentContent)
        {
            ViewBag.Language = Language;

            return View(currentContent);
        }
    }
}
