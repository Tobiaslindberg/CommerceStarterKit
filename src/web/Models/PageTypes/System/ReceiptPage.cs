/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using OxxCommerceStarterKit.Core.Attributes;

namespace OxxCommerceStarterKit.Web.Models.PageTypes.System
{
	[ContentType(GUID = "42faeb0c-a28a-430a-81d0-feb8da887bcf",
		DisplayName = "Receipt Page",
		GroupName = "Commerce System Pages",
		Order = 100,
		AvailableInEditMode = false,
		Description = "The page which shows the receipt after a payment.")]
	[SiteImageUrl]
	public class ReceiptPage : CommerceSampleModulePage
	{

		[Searchable(false)]
		[CultureSpecific]
		[Display(Name = "Thanks for shopping title",
			Order = 10)]
		public virtual string ThankYouTitle { get; set; }

		[Searchable(false)]
		[CultureSpecific]
		[Display(Name = "Thanks for shopping text",
			Order = 20)]
		public virtual XhtmlString ThankYouText { get; set; }

	}
}
