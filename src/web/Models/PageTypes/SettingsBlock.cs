/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.ComponentModel.DataAnnotations;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Shell.ObjectEditing;
using EPiServer.SpecializedProperties;
using EPiServer.Web;
using OxxCommerceStarterKit.Web.Models.Blocks;

namespace OxxCommerceStarterKit.Web.Models.PageTypes
{
	[ContentType(
		GUID = "75EA193A-DDBF-4590-8E22-BF74ABE56906",
		AvailableInEditMode = false,        // Just for settings, not able to add from edit mode
		DisplayName = "Settings Data",
		Description = "Contains global settings data for this site.",
		GroupName = "Commerce System",
		Order = 1)]
	public class SettingsBlock : BlockData
	{
		[Searchable(false)]
		[Display(
			Name = "CartPage",
			Description = "The page that displays the shopping cart.",
			GroupName = SystemTabNames.Settings,
			Order = 10)]
		[CultureSpecific]
		public virtual PageReference CartPage { get; set; }

		[Searchable(false)]
		[Display(
			Name = "WishList Page",
			Description = "The page that displays the wish list.",
			GroupName = SystemTabNames.Settings,
			Order = 20)]
		[CultureSpecific]
		public virtual PageReference WishListPage { get; set; }

		[Searchable(false)]
		[Display(
			Name = "Checkout Page",
			Description = "The checkout page to complete your order.",
			GroupName = SystemTabNames.Settings,
			Order = 30)]
		[CultureSpecific]
		public virtual PageReference CheckoutPage { get; set; }

		[Searchable(false)]
		[Display(
			Name = "Account Page",
			Description = "The page that displays your account information.",
			GroupName = SystemTabNames.Settings,
			Order = 35)]
		[CultureSpecific]
		public virtual PageReference AccountPage { get; set; }

		[Searchable(false)]
		[Display(
			Name = "Your Orders Page",
			Description = "The page that displays the order history.",
			GroupName = SystemTabNames.Settings,
			Order = 40)]
		public virtual PageReference YourOrdersPage { get; set; }

		[Searchable(false)]
		[Display(
			Name = "Receipt Page",
			Description = "The page you see after your order is complete",
			GroupName = SystemTabNames.Settings,
			Order = 45)]
		[CultureSpecific]
		public virtual PageReference ReceiptPage { get; set; }

		[Searchable(false)]
		[Display(
			Name = "Newsletter unsubscribe page",
			Description = "The page where the user can unsubscribe from the newsletter",
			GroupName = SystemTabNames.Settings,
			Order = 47)]
		public virtual PageReference NewsletterUnsubscribePage { get; set; }

		[Searchable(false)]
		[Display(
			Name = "TopLeftMenu",
			Description = "The menu at top left",
			GroupName = SystemTabNames.Content,
			Order = 50)]
		[CultureSpecific]
		public virtual LinkItemCollection TopLeftMenu { get; set; }

		[Searchable(false)]
		[Display(
			Name = "TopRightMenu",
			Description = "The menu at top right",
			GroupName = SystemTabNames.Content,
			Order = 60)]
		[CultureSpecific]
		public virtual LinkItemCollection TopRightMenu { get; set; }


		[Searchable(false)]
		[Display(
			Name = "Footer buttons",
			GroupName = SystemTabNames.Content,
			Order = 65)]
		[AllowedTypes(new Type[] { typeof(ButtonWithHelpLinkBlock) })]
		[CultureSpecific]
		public virtual ContentArea FooterButtons { get; set; }


		[Searchable(false)]
		[Display(
			Name = "Footer menu root folder",
			Description = "The folder who's children will be in the footer menu",
			GroupName = SystemTabNames.Content,
			Order = 70)]
		public virtual PageReference FooterMenuFolder { get; set; }

		[Searchable(false)]
		[Display(
			Name = "Social Media",
			Description = "Social Media Links",
			GroupName = SystemTabNames.Content,
			Order = 80)]
		[AllowedTypes(new [] { typeof(SocialMediaLinkBlock) })]
		[CultureSpecific]
		public virtual ContentArea SocialMediaIcons { get; set; }

		[Searchable(false)]
		[CultureSpecific]
		[Display(Name = "Delivery And Returns",
			GroupName=SystemTabNames.Content,
			Order = 90)]
		public virtual XhtmlString DeliveryAndReturns { get; set; }

		[Searchable(false)]
		[Display(Name = "Login Page",
			GroupName = SystemTabNames.Settings,
			Order=90)]
		public virtual ContentReference LoginPage { get; set; }

		[Searchable(false)]
		[Display(Name = "DIBS Payment Page",
			GroupName = SystemTabNames.Settings,
			Order = 100)]
		[CultureSpecific]
		public virtual ContentReference DibsPaymentPage { get; set; }

		[Searchable(false)]
		[Display(Name = "Search Page",
			GroupName = SystemTabNames.Settings,
			Order = 110)]
		public virtual ContentReference SearchPage { get; set; }

		[Searchable(false)]
		[Display(Name = "Show Prices For Related Products",
			GroupName = SystemTabNames.Settings,
			Order = 120)]
		public virtual bool ShowRelatedProductPrices { get; set; }

		[Searchable(false)]
		[Display(Name = "Error 500 title",
			GroupName = SystemTabNames.Settings,
			Order = 130)]
		// [CultureSpecific] - NOTE! You do not know what language caused an error
		public virtual string ErrorPageTitle { get; set; }

		[Searchable(false)]
		[Display(Name = "Error 500 text",
			GroupName = SystemTabNames.Settings,
			Order = 140)]
		// [CultureSpecific] - NOTE! You do not know what language caused an error
		public virtual XhtmlString ErrorPageText { get; set; }

		[Searchable(false)]
		[Display(Name = "Error 404 title",
			GroupName = SystemTabNames.Settings,
			Order = 150)]
		// [CultureSpecific] - NOTE! You do not know what language caused an error
		public virtual string FileNotFoundPageTitle { get; set; }

		[Searchable(false)]
		[Display(Name = "Error 404 text",
			GroupName = SystemTabNames.Settings,
			Order = 160)]
		// [CultureSpecific] - NOTE! You do not know what language caused an error
		public virtual XhtmlString FileNotFoundPageText { get; set; }

		[Searchable(false)]
		[Display(Name = "Import catalog last success time",
			GroupName = SystemTabNames.Settings,
			Order = 170)]
		public virtual DateTime ImportCatalogLastSuccessTime { get; set; }

		[Searchable(false)]
		[Display(Name = "The Logo to use on the site",
			GroupName = SystemTabNames.Settings,
			Order = 180)]
		[CultureSpecific]
		[UIHint(UIHint.Image)]
		public virtual ContentReference LogoImage { get; set; }

		[Searchable(false)]
        [CultureSpecific]
		[Display(Name = "Javascripts here will be added to header tag",
			GroupName = SystemTabNames.Settings,
			Order = 200)]
        [UIHint(UIHint.Textarea)]
		public virtual string HeaderScripts { get; set; }

		
	}
}
