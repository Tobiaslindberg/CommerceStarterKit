/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.ComponentModel.DataAnnotations;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;
using OxxCommerceStarterKit.Core.Attributes;

namespace OxxCommerceStarterKit.Web.Models.PageTypes
{
	[ContentType(GUID = "41dfefca-e8c0-4ed4-b251-dfcdac0389c4",
	 DisplayName = "Blogg",
	 Description = "A dynamic blog template",
	 GroupName = "Pages",
	 Order = 100)]
	[SiteImageUrl]
	public class BlogPage : SitePage
	{
		[Display(
			Name = "List view image",
			GroupName = SystemTabNames.Content,
			Order = 1)]
		[UIHint(UIHint.Image)]
		public virtual Url ListViewImage { get; set; }

		[Display(
			Name = "List view text",
			GroupName = SystemTabNames.Content,
			Order = 10)]
		public virtual string ListViewText { get; set; }

		[Display(
			Name = "Content area top",
			GroupName = SystemTabNames.Content,
			Order = 15)]
		public virtual ContentArea PreBodyContent { get; set; }

		[Display(
			Name = "Page title",
			GroupName = SystemTabNames.Content,
			Order = 20)]
		[CultureSpecific]
		public virtual string PageTitle { get; set; }

		[Display(
			Name = "Sub page title",
			GroupName = SystemTabNames.Content,
			Order = 30)]
		[CultureSpecific]
		public virtual string SubPageTitle { get; set; }

		[Display(
		Name = "Content area bottom",
		GroupName = SystemTabNames.Content,
		Order = 40)]
		public virtual ContentArea BodyContent { get; set; }
	}
}
