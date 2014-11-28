/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using EPiServer.Core;

namespace OxxCommerceStarterKit.Core.Extensions
{
	public static class LinkItemCollectionExtensions
	{
		public static List<PageData> ToPages(this EPiServer.SpecializedProperties.LinkItemCollection linkItemCollection)
		{
			List<PageData> pages = new List<PageData>();
			var contentLoader = EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<EPiServer.IContentLoader>();

			foreach (EPiServer.SpecializedProperties.LinkItem linkItem in linkItemCollection)
			{
				string linkUrl;
				if (!EPiServer.Web.PermanentLinkMapStore.TryToMapped(linkItem.Href, out linkUrl))
					continue;

				if (string.IsNullOrEmpty(linkUrl))
					continue;

				PageReference pageReference = PageReference.ParseUrl(linkUrl);

				if (PageReference.IsNullOrEmpty(pageReference))
					continue;

				pages.Add(contentLoader.Get<PageData>(pageReference));
			}

			return pages;
		}
	}
}
