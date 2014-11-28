/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Web;
using OxxCommerceStarterKit.Core.Attributes;
using OxxCommerceStarterKit.Web.EditorDescriptors;
using OxxCommerceStarterKit.Web.Models.CustomProperties;

namespace OxxCommerceStarterKit.Web.Models.PageTypes
{
	[ContentType(GUID = "d7cdf1da-83a0-4f9e-b602-55017ad333ee",
	 DisplayName = "Article Page",
	 Description = "A dynamic article template",
	 GroupName = "Pages",
	 Order = 100)]
	[SiteImageUrl]
	public class ArticlePage : SitePage
	{
		[Display(
			Name = "List view image",
			GroupName = SystemTabNames.Content,
			Order = 1)]
		[UIHint(UIHint.Image)]
		public virtual Url ListViewImage { get; set; }

		[Display(
			Name = "List view text",
			Description = "If empty, then the intro will be used instead",
			GroupName = SystemTabNames.Content,
			Order = 10)]
        [CultureSpecific]
		public virtual string ListViewText { get; set; }

		[Display(
			Name = "Content area top",
			GroupName = SystemTabNames.Content,
			Order = 15)]
        [CultureSpecific]
		public virtual ContentArea PreBodyContent { get; set; }

		[Display(
			Name = "Page title",
			GroupName= SystemTabNames.Content,
			Order = 20)]
        [CultureSpecific]
		public virtual string PageTitle { get; set; }

		[Display(
			Name = "Intro",
			GroupName = SystemTabNames.Content,
			Order = 30)]
        [CultureSpecific]
		public virtual XhtmlString Intro { get; set; }

		[Display(
			Name = "Body",
			GroupName = SystemTabNames.Content,
			Order = 40)]
		[CultureSpecific]
		public virtual XhtmlString BodyText { get; set; }

		[Display(
		Name = "Content area bottom",
		GroupName = SystemTabNames.Content,
		Order = 50)]
        [CultureSpecific]
		public virtual ContentArea BodyContent { get; set; }

	}
}
