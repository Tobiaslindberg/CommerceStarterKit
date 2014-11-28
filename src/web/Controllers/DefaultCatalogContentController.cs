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
using OxxCommerceStarterKit.Web.Business;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Controllers
{
    [TemplateDescriptor(Inherited = true)]
    [RequireClientResources]
    public class DefaultCatalogContentController : CommerceControllerBase<CatalogContentBase>
    {
        public DefaultCatalogContentController()
        {
            
        }

        public ViewResult Index(CatalogContentBase currentContent, PageData currentPage)
        {
            var virtualPath = String.Format("~/Views/{0}/Index.cshtml", currentContent.GetOriginalType().Name);
            if (!System.IO.File.Exists(Request.MapPath(virtualPath)))
            {
                virtualPath = String.Format("~/Views/{0}/Index.cshtml", Enum.GetName(typeof(CatalogContentType), currentContent.ContentType));
            }
            return View(virtualPath, CreateCatalogViewModel(currentContent, currentPage));
        }


        /// <summary>
        /// Creates a catalog view model instance.
        /// </summary>
        /// <param name="catalogContent">The catalog content.</param>
        /// <param name="pageData">The page data.</param>
        /// <returns>A catalog content view model instance.</returns>
        public ICatalogViewModel<CatalogContentBase> CreateCatalogViewModel(CatalogContentBase catalogContent, PageData pageData)
        {
            var activator = new Activator<ICatalogViewModel<CatalogContentBase>>();
            var model = activator.Activate(typeof(CatalogContentViewModel<>), catalogContent);
            InitializeCatalogViewModel(model);
            return model;
        }

       
       
    }
}
