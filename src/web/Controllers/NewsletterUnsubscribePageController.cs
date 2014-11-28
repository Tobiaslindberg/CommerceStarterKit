/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Web.Mvc;
using System.Web.Security;
using EPiServer.Framework.Localization;
using Mediachase.Commerce.Customers;
using OxxCommerceStarterKit.Web.Models.PageTypes;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Controllers
{
	public class NewsletterUnsubscribePageController : PageControllerBase<NewsletterUnsubscribePage>
    {
		private readonly LocalizationService _localizationService;


		public NewsletterUnsubscribePageController(LocalizationService localizationService)
		{
			_localizationService = localizationService;

		}


        
		[HttpGet]
        public ActionResult Index(NewsletterUnsubscribePage currentPage, string email = null)
        {
			var model = new NewsletterUnsubscribeViewModel(currentPage);
			model.Email = email;
            return View(model);
        }


		[HttpPost]
		public ActionResult Index(NewsletterUnsubscribeViewModel model, NewsletterUnsubscribePage currentPage)
		{
			if (model.CurrentPage == null)
			{
				model.CurrentPage = currentPage;
			}
			if (ModelState.IsValid) {

				var user = Membership.GetUser(model.Email);
				if (user == null)
				{
					ModelState.AddModelError("ErrorResponse", _localizationService.GetString("/common/newsletter_unsubscribe/not_registered"));
				}
				else
				{
					var customer = CustomerContext.Current.GetContactForUser(user);
					if (customer == null)
					{
						ModelState.AddModelError("ErrorResponse", _localizationService.GetString("/common/newsletter_unsubscribe/not_registered"));
					}
					else
					{
						customer.CustomerGroup = null;
						customer.SaveChanges();
						ViewBag.SuccessResponse = true;
					}
				}
			}
			
			return View(model);
		}
    }
}
