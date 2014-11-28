/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Framework.Web.Mvc;
using Mediachase.Commerce.Catalog;
using OxxCommerceStarterKit.Web.Models.Catalog;

namespace OxxCommerceStarterKit.Web.Controllers
{
    [TemplateDescriptor(Inherited = true)]
    [RequireClientResources]
    public class FashionStoreSubLandingNodeController : CommerceControllerBase<FashionStoreSubLandingNodeContent>
    {
         

        public FashionStoreSubLandingNodeController()         
        {
        }

        public ViewResult Index(CatalogContentBase currentContent, PageData currentPage)
        {
            var model = CreateFashionCatalogViewModel(currentContent);
            var virtualPath = String.Format("~/Views/{0}/Index.cshtml", currentContent.GetOriginalType().Name);
            if (!System.IO.File.Exists(Request.MapPath(virtualPath)))
            {
                virtualPath = String.Format("~/Views/{0}/Index.cshtml", Enum.GetName(typeof(CatalogContentType), currentContent.ContentType));
            }
            return View(virtualPath, model);
        }

       
    }
}
