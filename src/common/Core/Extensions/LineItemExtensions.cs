/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Orders;

namespace OxxCommerceStarterKit.Core.Extensions
{
    public static class LineItemExtensions
    {
		public static Injected<UrlResolver> UrlResolver { get; set; }
		public static Injected<ReferenceConverter> ReferenceConverter { get; set; }
		public static Injected<ICurrentMarket> CurrentMarket { get; set; }
		public static Injected<IContentLoader> ContentLoader { get; set; }
		

		/// <summary>
		/// Gets the entry link.
		/// </summary>
		/// <param name="lineItem">The line item.</param>
		/// <returns>The url link to the entry related to the line item.</returns>
		public static string GetEntryLink(this LineItem lineItem, string language)
		{
			if (lineItem == null)
			{
				return string.Empty;
			}
			var entry = GetEntryContent<EntryContentBase>(lineItem);
			var parent = entry.GetParent();
			if (string.IsNullOrEmpty(language))
			{
				language = CurrentMarket.Service.GetCurrentMarket().DefaultLanguage.Name;
			}

		    return UrlResolver.Service.GetUrl(parent == null ? entry.ContentLink : parent.ContentLink, language);
		}


		/// <summary>
		/// Gets the content of the entry.
		/// </summary>
		/// <param name="lineItem">The line item.</param>
		/// <returns></returns>
		public static T GetEntryContent<T>(this LineItem lineItem) where T : EntryContentBase
		{
			return GetEntryContent<T>(lineItem.CatalogEntryId);
		}


		/// <summary>
		/// Gets the content of the entry by code.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="code">The code.</param>
		/// <returns></returns>
		private static T GetEntryContent<T>(string code) where T : EntryContentBase
		{
			return ContentLoader.Service.Get<T>(ReferenceConverter.Service.GetContentLink(code, CatalogContentType.CatalogEntry));
		}

    }
}
