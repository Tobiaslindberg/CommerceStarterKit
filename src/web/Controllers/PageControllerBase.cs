/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;
using log4net;
using OxxCommerceStarterKit.Web.Business;
using OxxCommerceStarterKit.Web.Models.PageTypes;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Controllers
{
	public class PageControllerBase<T> : PageController<T> where T : PageData
	{
		private static Injected<IContentLoader> _contentLoaderService ;
        protected static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected IContentLoader ContentLoader
		{
			get { return _contentLoaderService.Service; }
		}
 
		protected T CurrentPage
		{
			get
			{
				return PageContext.Page as T;
			}
		}

		protected override void OnAuthorization(AuthorizationContext filterContext)
		{
			CheckAccess(filterContext);
			base.OnAuthorization(filterContext);
		}

		private void CheckAccess(AuthorizationContext filterContext)
		{
			if (CurrentPage.QueryAccess().HasFlag(AccessLevel.Read))
				return;
			ServeAccessDenied(filterContext);
		}

		private void ServeAccessDenied(AuthorizationContext filterContext)
		{
			_log.Info(
				"AccessDenied",
				new AccessDeniedException(CurrentPage.ContentLink));

			AccessDeniedDelegate accessDenied
				= AccessDeniedHandler.CreateAccessDeniedDelegate(filterContext);
			accessDenied(filterContext);
		}

		public IPageViewModel<PageData> CreatePageViewModel(PageData pageData)
		{
			var activator = new Activator<IPageViewModel<PageData>>();
			var model = activator.Activate(typeof(PageViewModel<>), pageData);
			InitializePageViewModel(model);
			return model;
		}

		public void InitializePageViewModel<TViewModel>(TViewModel model) where TViewModel : IPageViewModel<PageData>
		{
			var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
			if (ContentReference.IsNullOrEmpty(ContentReference.StartPage) == false)
			{
				HomePage startPage = contentLoader.Get<HomePage>(ContentReference.StartPage);

				model.TopLeftMenu = model.TopLeftMenu ?? startPage.Settings.TopLeftMenu;
				model.TopRightMenu = model.TopRightMenu ?? startPage.Settings.TopRightMenu;
				model.SocialMediaIcons = model.SocialMediaIcons ?? startPage.Settings.SocialMediaIcons;
				model.FooterButtons = model.FooterButtons ?? startPage.Settings.FooterButtons;
				if (model.CurrentPage != null)
				{
					model.Section = model.Section ?? GetSection(model.CurrentPage.ContentLink);
				}
				else
				{
					model.Section = model.Section ?? GetSection(startPage.ContentLink);
				}
				model.LoginPage = model.LoginPage ?? startPage.Settings.LoginPage;
				model.AccountPage = model.AccountPage ?? startPage.Settings.AccountPage;
			    model.Language = string.IsNullOrEmpty(model.Language) == false ? model.Language : startPage.LanguageBranch;
				model.CheckoutPage = model.CheckoutPage ?? startPage.Settings.CheckoutPage;
			}
		}



        /// <summary>
        /// Returns the closest parent to the start page of the given page.
        /// </summary>
        /// <remarks>
        /// Start Page
        ///     - About Us (This is the section)
        ///         - News
        ///             News 1 (= contentLink parameter)
        /// </remarks>
        /// <param name="contentLink">The content you want to find the section for</param>
        /// <returns>The parent page closes to the start page, or the page referenced by the contentLink itself</returns>
		private IContent GetSection(ContentReference contentLink)
		{
			var currentContent = ContentLoader.Get<IContent>(contentLink);
			if (currentContent.ParentLink != null && currentContent.ParentLink.CompareToIgnoreWorkID(ContentReference.StartPage))
			{
				return currentContent;
			}

            // Loop upwards until the parent is start page or root
			return ContentLoader.GetAncestors(contentLink)
				.OfType<PageData>()
				.SkipWhile(x => x.ParentLink == null || !x.ParentLink.CompareToIgnoreWorkID(ContentReference.StartPage))
				.FirstOrDefault();
		}

	}
}
