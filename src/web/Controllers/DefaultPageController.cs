/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.GoogleAnalytics.Helpers;
using Mediachase.Commerce.Customers;
using OxxCommerceStarterKit.Web.Business.Analytics;
using OxxCommerceStarterKit.Web.Models.PageTypes;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Controllers
{
    [TemplateDescriptor(Inherited = true)]
    public class DefaultPageController : PageControllerBase<PageData>
    {
		private readonly IContentLoader _contentLoader;        

        public DefaultPageController(IContentLoader contentLoader )
        {            
			_contentLoader = contentLoader;		    
        }

        public ViewResult Index(PageData currentPage)
        {
            var virtualPath = String.Format("~/Views/{0}/Index.cshtml", currentPage.GetOriginalType().Name);
            if (!System.IO.File.Exists(Request.MapPath(virtualPath)))
            {
                virtualPath = "Index";
            }

            return View(virtualPath, CreatePageViewModel(currentPage));
        }

		[HttpPost]
		public ActionResult Get(int reference, HomePage currentPage)
		{
			currentPage = currentPage ?? _contentLoader.Get<HomePage>(ContentReference.StartPage);

			var page = _contentLoader.Get<IContent>(new ContentReference(reference));
			if (page is ArticlePage)
			{
				var model = new PageViewModel<ArticlePage>((ArticlePage)page);
				return View("~/Views/ArticlePage/Index.cshtml", model);
			}
			return View();
		}
    }
}
