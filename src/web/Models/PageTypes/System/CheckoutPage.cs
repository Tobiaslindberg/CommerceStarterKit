/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace OxxCommerceStarterKit.Web.Models.PageTypes.System
{
    [ContentType(GUID = "3DC355CD-DD03-49B0-9F4D-A0DFF04DCB3A",
        DisplayName = "Checkout Page",
        GroupName = "Commerce System Pages",
        Order = 100,
		AvailableInEditMode = false,
        Description = "The page which shows single shipment checkout.")]
    public class CheckoutPage : CommerceSampleModulePage
    {


		[Display(Name = "Terms and conditions",
			Description = "Article for Terms and conditions modal window",
			GroupName = SystemTabNames.Content, 
			Order = 1)]
		[Searchable(false)]
		public virtual ContentReference TermsArticle { get; set; }

    }
}
