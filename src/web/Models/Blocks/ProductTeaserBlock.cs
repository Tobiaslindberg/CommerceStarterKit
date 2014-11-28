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
using EPiServer.Web;
using OxxCommerceStarterKit.Core.Attributes;

namespace OxxCommerceStarterKit.Web.Models.Blocks
{
	[ContentType(GUID = "eb436f04-d5e7-4f0d-95f6-173474db1742")]
	[SiteImageUrl]
	public class ProductTeaserBlock : SiteBlockData
	{
		[Display(
			GroupName = SystemTabNames.Content,
			Order = 10)]
		[CultureSpecific]
		[UIHint(UIHint.Image)]
		public virtual ContentReference Image { get; set; }

		[Display(
			GroupName = SystemTabNames.Content,
			Order = 20)]
		[CultureSpecific]
		//[AllowedTypes(typeof(ProductBase))]
		public virtual ContentReference Product { get; set; }

		[Display(
			GroupName = SystemTabNames.Content,
			Order = 30)]
		[CultureSpecific]
		public virtual string ImageTitle { get; set; }

		[Display(
			GroupName = SystemTabNames.Content,
			Order = 40)]
		[CultureSpecific]
		public virtual string ImageText { get; set; }

		[Display(
			GroupName = SystemTabNames.Content,
			Order = 50)]
		[CultureSpecific]
		[RegularExpression("#([0-9a-f]{3}|[0-9a-f]{6})")]
		public virtual string ImageTextColor { get; set; }

		[Display(
			GroupName = SystemTabNames.Content,
			Order = 60)]
		[CultureSpecific]
		[RegularExpression("#([0-9a-f]{3}|[0-9a-f]{6})")]
		public virtual string ImageTextBackgroundColor { get; set; }

		[Display(
			GroupName = SystemTabNames.Content,
			Order = 70)]
		[Range(0, 100)]
		[CultureSpecific]
		public virtual int ImageTextBackgroundTransparency { get; set; }

	}
}
