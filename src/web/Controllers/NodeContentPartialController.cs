/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Find;
using EPiServer.Find.Framework;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Framework.Web;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;
using Mediachase.Commerce.Catalog;
using OxxCommerceStarterKit.Web.Models.Catalog;
using OxxCommerceStarterKit.Web.Models.FindModels;
using OxxCommerceStarterKit.Web.Models.ViewModels.Simple;

namespace OxxCommerceStarterKit.Web.Controllers
{
    [TemplateDescriptor(Inherited = true, TemplateTypeCategory = TemplateTypeCategories.MvcPartialController)]
    public class NodeContentPartialController : ContentController<NodeContent>
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
            SetLanguage();
            string language = Language;
            var client = SearchClient.Instance;

            try
            {
                var result = client.Search<FindProduct>()
                    .Filter(x => x.ParentCategoryId.Match(currentContent.ContentLink.ID))
                    .Filter(x => x.Language.Match(language))
                    .StaticallyCacheFor(TimeSpan.FromMinutes(1))
                    .GetResult();

                var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
                var referenceConverter = ServiceLocator.Current.GetInstance<ReferenceConverter>();


                return PartialView("Blocks/NodeContentPartial", result.Select(p => {
                
                    var productLink = referenceConverter.GetContentLink(p.Id, CatalogContentType.CatalogEntry, 0);
                
                    return new ProductViewModel(contentLoader.Get<FashionProductContent>(productLink));

                }));
            }

            catch (Exception)
            {
                return PartialView("Blocks/NodeContentPartial", null);
            }
        }
    }
}
