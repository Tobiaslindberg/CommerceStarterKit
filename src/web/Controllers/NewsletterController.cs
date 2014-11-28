/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using OxxCommerceStarterKit.Web.Models.PageTypes;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Controllers
{
	[TemplateDescriptor()]
	public class NewsletterController : PageController<NewsletterPage>
    {
        //
        // GET: /Newsletter/
        public ActionResult Index(NewsletterPage currentPage, HomePage homePage)
        {
			var model = new NewsletterViewModel(currentPage);

			var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();

			var homepage = contentLoader.Get<HomePage>(ContentReference.StartPage);
			if (homepage != null && homepage.Settings.NewsletterUnsubscribePage != null)
			{
				model.UnsubscribeUrl = ServiceLocator.Current.GetInstance<UrlResolver>().GetUrl(homepage.Settings.NewsletterUnsubscribePage);

				if (model.UnsubscribeUrl.Contains("?"))
				{
					model.UnsubscribeUrl += "&email=%recipient%";
				}
				else
				{
					model.UnsubscribeUrl += "?email=%recipient%";
				}
			}


            return View("Newsletter1", model);
			//return View("Index", currentPage");
        }


	}
}
