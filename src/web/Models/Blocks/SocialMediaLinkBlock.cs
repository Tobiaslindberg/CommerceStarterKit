/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.ComponentModel.DataAnnotations;
using EPiServer;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace OxxCommerceStarterKit.Web.Models.Blocks
{
	/// <summary>
	/// Used to insert a link which is styled as a button
	/// </summary>
	[ContentType(GUID = "9F50587C-C09E-4B9D-8011-54BABF4AFB24")]
	public class SocialMediaLinkBlock : SiteBlockData
	{
		[Display(Order = 10, GroupName = SystemTabNames.Content)]
		[Searchable(false)]
		public virtual string Title { get; set; }

		[Display(Order = 20, GroupName = SystemTabNames.Content, Name = "Lenke til sosialt media")]
		[Searchable(false)]
		public virtual Url Link { get; set; }

		[Display(Order = 30, GroupName = SystemTabNames.Content, Name = "CSS klasse for ikon")]
		[Searchable(false)]
		public virtual string CssClass { get; set; }
	}
}
