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
using OxxCommerceStarterKit.Core.Attributes;

namespace OxxCommerceStarterKit.Web.Models.PageTypes
{
	[ContentType(DisplayName = "Newsletter unsubscribe page", 
		GUID = "5c7fd942-3bf3-46fa-a309-59a94466d103", 
		Description = "A page to allow a user to unsubscribe from a newsletter",
		GroupName = "Pages")]
	[SiteImageUrl]
	public class NewsletterUnsubscribePage : SitePage
	{
		[CultureSpecific]
		[Display(Name = "Title",
			Description = "The title of the unsubscribe page",
			GroupName = SystemTabNames.Content,
			Order = 10)]
		public virtual string Title { get; set; }


		[CultureSpecific]
		[Display(Name = "Main body",
			Description = "The main body of the unsubscribe page",
			GroupName = SystemTabNames.Content,
			Order = 20)]
		public virtual XhtmlString MainBody { get; set; }

	}
}
