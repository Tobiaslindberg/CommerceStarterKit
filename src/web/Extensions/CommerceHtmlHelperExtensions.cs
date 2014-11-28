/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Editor;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Mediachase.Commerce;
using OxxCommerceStarterKit.Core.Extensions;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Extensions
{
	public static class CommerceHtmlHelperExtensions
	{
		public static MvcHtmlString InStockRange(this HtmlHelper html, LazyVariationViewModelCollection variationViewModels)
		{
			var variationViewModelsList = variationViewModels.Value.ToList();
			if (variationViewModelsList.Any())
			{
				var minInStock =
					variationViewModelsList.Min(x => x.Inventory.Value != null ? x.Inventory.Value.InStockQuantity : 0);
				var maxInStock =
					variationViewModelsList.Max(x => x.Inventory.Value != null ? x.Inventory.Value.InStockQuantity : 0);

				if (maxInStock.Equals(minInStock))
				{
					return new MvcHtmlString(GetInStockString(maxInStock));
				}

				return
					new MvcHtmlString(String.Concat(GetInStockString(minInStock), " - ", GetInStockString(maxInStock),
						" pending on variant."));
			}
			return new MvcHtmlString("Cannot find info about stock");
		}

		public static MvcHtmlString UnitPriceRange(this HtmlHelper html, LazyVariationViewModelCollection variationViewModels)
		{
			var variationViewModelsList = variationViewModels.Value.ToList();
			if (variationViewModelsList.Any(x => x.Price != null))
			{
				try
				{
					var minPrice =
						variationViewModelsList.Min(x => x.Price != null ? x.Price.UnitPrice : default(Money));
					var maxPrice =
						variationViewModelsList.Max(x => x.Price != null ? x.Price.UnitPrice : default(Money));

					if (maxPrice.Equals(minPrice))
					{
						return new MvcHtmlString(GetPriceString(maxPrice));
					}

					return
						new MvcHtmlString(String.Concat(GetPriceString(minPrice), " - ", GetPriceString(maxPrice),
							" pending on variant."));
				}
				catch (Exception ex)
				{
					return new MvcHtmlString(ex.Message);
				}
			}
			return new MvcHtmlString("Cannot find a price");
		}

		private static string GetInStockString(decimal inStock)
		{
			return inStock.ToString(CultureInfo.CurrentCulture);
		}

		private static string GetPriceString(Money price)
		{
			return price.ToString(CultureInfo.CurrentCulture);
		}


		public static MvcHtmlString AssetImage(this HtmlHelper html, EntryContentBase entry)
		{
			return AssetImage(html, entry, ServiceLocator.Current.GetInstance<IPermanentLinkMapper>());
		}

		public static MvcHtmlString AssetImage(this HtmlHelper html, EntryContentBase entry, IPermanentLinkMapper permanentLinkMapper)
		{
			var commerceMedia = entry.GetCommerceMedia();
			if (commerceMedia == null)
			{
				return MvcHtmlString.Empty;
			}

			var contentLink = commerceMedia.AssetContentLink(permanentLinkMapper);

			return html.Partial(UIHint.Image, contentLink);
		}

		public static MvcHtmlString AssetUrl(this HtmlHelper html, EntryContentBase entry)
		{
			return AssetUrl(html, entry, ServiceLocator.Current.GetInstance<UrlResolver>(), ServiceLocator.Current.GetInstance<IPermanentLinkMapper>());
		}

		public static MvcHtmlString AssetUrl(this HtmlHelper html, EntryContentBase entry, UrlResolver urlResolver, IPermanentLinkMapper permanentLinkMapper)
		{
			var commerceMedia = entry.GetCommerceMedia();
			if (commerceMedia == null)
			{
				return new MvcHtmlString(null);
			}

			return AssetUrl(commerceMedia.AssetContentLink(permanentLinkMapper), urlResolver);
		}

		public static MvcHtmlString AssetUrl(this HtmlHelper html, EntryContentBase entry, int index)
		{
			return AssetUrl(html, entry, index, ServiceLocator.Current.GetInstance<UrlResolver>(), ServiceLocator.Current.GetInstance<IPermanentLinkMapper>());
		}

		public static MvcHtmlString AssetUrl(this HtmlHelper html, EntryContentBase entry, int index, UrlResolver urlResolver, IPermanentLinkMapper permanentLinkMapper)
		{

			var commerceMedia = entry.GetCommerceMedia(index);
			if (commerceMedia == null)
			{
				return new MvcHtmlString(null);
			}


			return AssetUrl(commerceMedia.AssetContentLink(permanentLinkMapper), urlResolver);
		}

		public static List<string> AssetImageUrls(this HtmlHelper html, EntryContentBase entry)
		{
			var output = new List<string>();
			var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();

			if (entry != null)
			{
				foreach (var imageLink in html.AssetImageReferences(entry))
				{
					output.Add(urlResolver.GetUrl(imageLink));
				}
			}

			return output;
		}

		public static List<ContentReference> AssetImageReferences(this HtmlHelper html, EntryContentBase entry)
		{
			return entry.AssetImageUrls();
		}

		public static string AssetSwatch(this HtmlHelper html, EntryContentBase entry)
		{
			if (entry != null)
			{
				return entry.AssetSwatchUrl();
			}
			return "";
		}

		private static MvcHtmlString AssetUrl(ContentReference mediaReference, UrlResolver urlResolver)
		{
			string url = urlResolver.GetUrl(mediaReference);
			if (PageEditing.PageIsInEditMode && url.ToLower().EndsWith("epieditmode=false"))
			{
				url = System.Text.RegularExpressions.Regex.Replace(url, @",,\d+[?]epieditmode=false$", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			}
			return new MvcHtmlString(url);
		}


	}
}
