/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.WebPages;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Editor;
using EPiServer.ServiceLocation;
using EPiServer.UI.Report;
using EPiServer.Web;
using EPiServer.Web.Routing;
using OxxCommerceStarterKit.Web.Models.PageTypes;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Business
{
    public class PageContextActionFilter : IResultFilter
    {
        private readonly IContentLoader _contentLoader;
        private readonly UrlResolver _urlResolver;
        private readonly ILanguageBranchRepository _languageBranchRepository;
        // private readonly ViewModelFactory _modelFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageContextActionFilter"/> class.
        /// </summary>
        public PageContextActionFilter(IContentLoader contentLoader, UrlResolver urlResolver, ILanguageBranchRepository languageBranchRepository)
        {
            _contentLoader = contentLoader;
            _urlResolver = urlResolver;
            _languageBranchRepository = languageBranchRepository;
        }

        /// <summary>
        /// Called before an action result executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            SettingsBlock settings = SiteConfiguration.Current().Settings;

            // This can actually be null if we have a problem with our language settings
            if (settings != null)
			{
				var chrome = new Chrome();
				chrome.TopLeftMenu = settings.TopLeftMenu;
				chrome.TopRightMenu = settings.TopRightMenu;
				chrome.FooterMenu = GetFooterMenuContent(settings);
				chrome.SocialMediaIcons = settings.SocialMediaIcons;
				chrome.FooterButtons = settings.FooterButtons;
				chrome.LoginPage = settings.LoginPage;
				chrome.AccountPage = settings.AccountPage;
				
				chrome.CheckoutPage = settings.CheckoutPage;
				chrome.SearchPage = settings.SearchPage;
				if (settings.LogoImage != null)
				{
					chrome.LogoImageUrl = _urlResolver.GetUrl(settings.LogoImage);
				}
				else
				{
					chrome.LogoImageUrl = new Url("/Content/Images/commerce-shop-logo.png");
				}

                // Set up languages for Chrome
                var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
                var startPage = contentLoader.Get<HomePage>(ContentReference.StartPage);
                chrome.Language = startPage.LanguageBranch;
			    chrome.Languages = GetLanguageInfo(startPage);

				filterContext.Controller.ViewBag.Chrome = chrome;
			}
        }

        public IEnumerable<ChromeLanguageInfo> GetLanguageInfo(PageData page)
        {
            List<ChromeLanguageInfo> languages = new List<ChromeLanguageInfo>();
            ReadOnlyStringList pageLanguages = page.PageLanguages;
            string currentLanguage = page.LanguageBranch;

            foreach (string language in pageLanguages)
            {
                LanguageBranch languageBranch = _languageBranchRepository.ListEnabled().FirstOrDefault(l => l.LanguageID.Equals(language, StringComparison.InvariantCultureIgnoreCase));
                if (languageBranch != null)
                {
                    languages.Add(new ChromeLanguageInfo()
                    {
                        DisplayName = languageBranch.Name,
                        IconUrl = languageBranch.ResolvedIconPath, //"/Content/Images/flags/" + language + ".png",
                        // We use this to enable language switching inside edit mode too
                        Url = languageBranch.CurrentUrlSegment,
                        EditUrl = PageEditing.GetEditUrlForLanguage(page.ContentLink, languageBranch.LanguageID),
                        Selected = string.Compare(language, currentLanguage, StringComparison.InvariantCultureIgnoreCase) == 0
                    });
                }
            }

            return languages;
        }



        private IEnumerable<PageData> GetFooterMenuContent(SettingsBlock settings)
        {
            if (settings.FooterMenuFolder != null)
            {
                return _contentLoader.GetChildren<PageData>(settings.FooterMenuFolder).FilterForDisplay<PageData>(true, true);
            }
            else
            {
                return new List<PageData>();
            }
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }
    }
}
