/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.Framework.Web;
using EPiServer.ServiceLocation;

namespace OxxCommerceStarterKit.Web.Business
{
	public static class ContentExtensions
	{
		/// <summary>
		/// Filters content which should not be visible to the user. 
		/// </summary>
		public static IEnumerable<T> FilterForDisplay<T>(this IEnumerable<T> contents, bool requirePageTemplate = false, bool requireVisibleInMenu = false)
			where T : IContent
		{
			var accessFilter = new FilterAccess();
			var publishedFilter = new FilterPublished();
			contents = contents.Where(x => !publishedFilter.ShouldFilter(x) && !accessFilter.ShouldFilter(x));
			if (requirePageTemplate)
			{
				var templateFilter = ServiceLocator.Current.GetInstance<FilterTemplate>();
				templateFilter.TemplateTypeCategories = TemplateTypeCategories.Page;
				contents = contents.Where(x => !templateFilter.ShouldFilter(x));
			}
			if (requireVisibleInMenu)
			{
				contents = contents.Where(x => VisibleInMenu(x));
			}
			return contents;
		}

		private static bool VisibleInMenu(IContent content)
		{
			var page = content as PageData;
			if (page == null)
			{
				return true;
			}
			return page.VisibleInMenu;
		}
	}
}
