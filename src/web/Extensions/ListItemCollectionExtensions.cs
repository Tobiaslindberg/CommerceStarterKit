/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using EPiServer;
using EPiServer.Core;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;
using EPiServer.Web;

namespace OxxCommerceStarterKit.Web.Extensions
{
	public static class LinkItemCollectionExtension
	{
		/// <summary>
		/// Prepares all links in a LinkItemCollection for output
		/// by filtering out inaccessible links and ensures all links are correct.
		/// </summary>
		/// <param name="linkItemCollection">The collection of links to prepare.</param>
		/// <param name="targetExternalLinksToNewWindow">True will set target to _blank if target is not specified for the LinkItem.</param>
		/// <returns>A prepared and filtered list of LinkItems</returns>
		public static IEnumerable<LinkItem> ToPreparedLinkItems(this LinkItemCollection linkItemCollection, bool targetExternalLinksToNewWindow)
		{
			var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();

			if (linkItemCollection != null)
			{
				foreach (var linkItem in linkItemCollection)
				{
					var url = new UrlBuilder(linkItem.Href);
					if (PermanentLinkMapStore.ToMapped(url))
					{
						var pr = PermanentLinkUtility.GetContentReference(url);
						if (!PageReference.IsNullOrEmpty(pr))
						{
							// page
							var page = contentLoader.Get<PageData>(pr);
							if (IsPageAccessible(page))
							{
								linkItem.Href = page.LinkURL;
								yield return linkItem;
							}
						}
						else
						{
							// document
							if (IsFileAccessible(linkItem.Href))
							{
								Global.UrlRewriteProvider.ConvertToExternal(url, null, System.Text.Encoding.UTF8);
								linkItem.Href = url.Path;
								yield return linkItem;
							}
						}
					}
					else if (!linkItem.Href.StartsWith("~"))
					{
						// external
						if (targetExternalLinksToNewWindow && string.IsNullOrEmpty(linkItem.Target))
						{
							linkItem.Target = "_blank";
						}
						if (linkItem.Href.StartsWith("mailto:") || linkItem.Target == "null")
						{
							linkItem.Target = string.Empty;
						}
						yield return linkItem;
					}
				}
			}
		}

		/// <summary>
		/// Prepares all links in a LinkItemCollection for output
		/// by filtering out inaccessible links and ensures all links are correct.
		/// </summary>
		/// <param name="linkItemCollection">The collection of links to prepare.</param>
		/// <returns>A prepared and filtered list of LinkItems</returns>
		public static IEnumerable<LinkItem> ToPreparedLinkItems(this LinkItemCollection linkItemCollection)
		{
			return linkItemCollection.ToPreparedLinkItems(false);
		}

		/// <summary>
		/// Converts a LinkItemCollection to typed pages. Any non-pages will be filtered out. (Not compatible with PageList - Use ToPageDataList)
		/// </summary>
		/// <typeparam name="T">PageType</typeparam>
		/// <param name="linkItemCollection">The collection of links to convert</param>
		/// <returns>An enumerable of typed PageData</returns>
		public static IEnumerable<T> ToPages<T>(this LinkItemCollection linkItemCollection) where T : PageData
		{
			var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();

			if (linkItemCollection != null)
			{
				foreach (var linkItem in linkItemCollection)
				{
					var url = new UrlBuilder(linkItem.Href);
					if (PermanentLinkMapStore.ToMapped(url))
					{
						var pr = PermanentLinkUtility.GetContentReference(url);
						if (!PageReference.IsNullOrEmpty(pr))
						{
							var page = contentLoader.Get<PageData>(pr);
							if (page is T && IsPageAccessible(page))
							{
								yield return (T)page;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Converts a LinkItemCollection to typed pages. Any non-pages will be filtered out. (Not compatible with PageList - Use ToPageDataList)
		/// </summary>
		/// <typeparam name="T">PageType</typeparam>
		/// <param name="linkItemCollection">The collection of links to convert</param>
		/// <returns>An enumerable of typed PageData</returns>
		public static IEnumerable<T> ToMedia<T>(this LinkItemCollection linkItemCollection) where T : MediaData
		{
			var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();

			if (linkItemCollection != null)
			{
				foreach (var linkItem in linkItemCollection)
				{
					var url = new UrlBuilder(linkItem.Href);
					if (PermanentLinkMapStore.ToMapped(url))
					{
						var pr = PermanentLinkUtility.GetContentReference(url);
						if (!PageReference.IsNullOrEmpty(pr))
						{
							var page = contentLoader.Get<MediaData>(pr);
							if (page is T)
							{
								yield return (T)page;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Converts a LinkItemCollection to typed pages. Any non-pages will be filtered out. (PageList compatible)
		/// </summary>
		/// <typeparam name="T">PageType</typeparam>
		/// <param name="linkItemCollection"></param>
		/// <returns>A list of typed PageData</returns>
		public static List<T> ToPageDataList<T>(this LinkItemCollection linkItemCollection) where T : PageData
		{
			return linkItemCollection.ToPages<T>().ToList();
		}

		/// <summary>
		/// Converts a LinkItemCollection to a list of pages. Any non-pages will be filtered out. (PageList compatible)
		/// </summary>
		/// <param name="linkItemCollection"></param>
		/// <returns>A list of typed PageData</returns>
		public static List<PageData> ToPageDataList(this LinkItemCollection linkItemCollection)
		{
			return linkItemCollection.ToPages<PageData>().ToList();
		}

		private static bool IsPageAccessible(PageData page)
		{
			return (page != null &&
				!page.IsDeleted &&
				page.Status == VersionStatus.Published &&
				page.PendingPublish == false &&
				page.StartPublish < DateTime.Now &&
				page.StopPublish > DateTime.Now &&
				page.ACL.QueryDistinctAccess(AccessLevel.Read));
		}

		// TODO: Possibly a better solution w/o try catch?
		private static bool IsFileAccessible(string filePath)
		{
			try
			{
				HostingEnvironment.VirtualPathProvider.GetFile(filePath);
				return true;
			}
			catch { }
			return false;
		}
	}
}
