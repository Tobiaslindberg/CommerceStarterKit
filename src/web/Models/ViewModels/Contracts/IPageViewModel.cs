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
    public interface IPageViewModel<out T> 
        where T : PageData
    {
        T CurrentPage { get; }

		LinkItemCollection TopLeftMenu { get; set; }
		LinkItemCollection TopRightMenu { get; set; }
		IEnumerable<PageData> FooterMenu { get; set; }
		ContentArea SocialMediaIcons { get; set; }
		ContentArea FooterButtons { get; set; }
		IContent Section { get; set; }
        ContentReference LoginPage { get; set; }
        ContentReference AccountPage { get; set; }
        ContentReference CheckoutPage { get; set; }        
        string Language { get; set; }
        string SearchUrl { get; set; }
        string GoogleAnalyticsTrackingCode { get; set; }
    }
}
