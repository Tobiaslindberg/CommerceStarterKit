/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Filters;
using OxxCommerceStarterKit.Core.Attributes;

namespace OxxCommerceStarterKit.Web.Models.Blocks
{
	[ContentType(GUID = "771f1e97-fd06-4349-a2b8-2f4dee314751", 
		DisplayName = "Page list block")]
	[SiteImageUrl]
	public class PageListBlock : SiteBlockData
	{
		[Display(
			GroupName = SystemTabNames.Content,
			Name = "Antall",
			Order = 10)]
		[DefaultValue(3)]
		[Required]
		[Searchable(false)]
		public virtual int Count { get; set; }

		[Display(
			GroupName = SystemTabNames.Content,
			Name = "Sortering",
			Order = 20)]
		[DefaultValue(FilterSortOrder.PublishedDescending)]
		[UIHint("SortOrder")]
		[BackingType(typeof(PropertyNumber))]
		[Searchable(false)]
		public virtual FilterSortOrder SortOrder { get; set; }

		[Display(
			GroupName = SystemTabNames.Content,
			Name = "Hent fra mappe",
			Order = 30)]
		[Required]
		[Searchable(false)]
		public virtual PageReference Root { get; set; }

		[Display(
			GroupName = SystemTabNames.Content,
			Name = "Filtrer på type",
			Order = 40)]
		[Searchable(false)]
		public virtual PageType PageTypeFilter { get; set; }

		[Display(
			GroupName = SystemTabNames.Content,
			Name = "Filtrer på kategori",
			Order = 50)]
		[Searchable(false)]
		public virtual CategoryList CategoryFilter { get; set; }

		[Display(
			GroupName = SystemTabNames.Content,
			Name = "Søk i undermapper",
			Order = 60)]
		[Searchable(false)]
		public virtual bool Recursive { get; set; }

		#region IInitializableContent

		/// <summary>
		/// Sets the default property values on the content data.
		/// </summary>
		/// <param name="contentType">Type of the content.</param>
		public override void SetDefaultValues(ContentType contentType)
		{
			base.SetDefaultValues(contentType);

			Count = 3;
			SortOrder = FilterSortOrder.PublishedDescending;
		}

		#endregion


	}
}
