/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.SpecializedProperties;

namespace OxxCommerceStarterKit.Web.Models.ViewModels
{
    public class PageViewModel<T> : IPageViewModel<T>
        where T : PageData
    {
        public PageViewModel(T currentPage)
        {
            CurrentPage = currentPage;
            Language = currentPage.LanguageBranch;
        }

        protected PageViewModel()
        {            
        }

        public T CurrentPage
        {
            get;
            set;
        }

		public LinkItemCollection TopLeftMenu { get; set; }
		public LinkItemCollection TopRightMenu { get; set; }
		public IEnumerable<PageData> FooterMenu { get; set; }
		public ContentArea SocialMediaIcons { get; set; }
		public ContentArea FooterButtons { get; set; }
		public IContent Section { get; set; }
        public ContentReference LoginPage { get; set; }
        public ContentReference AccountPage { get; set; }
        public ContentReference CheckoutPage { get; set; }       
        public string Language { get; set; }
        public string SearchUrl { get; set; }
        public string GoogleAnalyticsTrackingCode { get; set; }
    }
}
