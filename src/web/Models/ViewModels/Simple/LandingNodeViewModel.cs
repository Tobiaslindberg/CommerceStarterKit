/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Web;
using EPiServer.Find;
using EPiServer.Find.Framework;
using OxxCommerceStarterKit.Core.Extensions;
using OxxCommerceStarterKit.Web.Api;
using OxxCommerceStarterKit.Web.Models.Catalog;
using OxxCommerceStarterKit.Web.Models.FindModels;

namespace OxxCommerceStarterKit.Web.Models.ViewModels.Simple
{
	public class LandingNodeViewModel
	{
		public int MaxItems { get; set; }
		
		private ITypeSearch<FindProduct> _query;
		public SearchResults<FindProduct> SearchResults
		{
			get
			{
                return _query.Take(MaxItems).StaticallyCacheFor(TimeSpan.FromMinutes(1)).GetResult();
			}

		}

		public LandingNodeViewModel(string language)
		{
			_query = SearchClient.Instance.Search<FindProduct>(ShoppingController.GetLanguage(language))
				.Filter(x => x.Language.Match(language))
				.Filter(x => x.ShowInList.Match(true))
				.OrderByDescending(x => x.SalesCounter);
		}

		public LandingNodeViewModel(FashionStoreSubLandingNodeContent category, HttpRequestBase httpRequest)
			: this(category, GetLanguage(httpRequest)) { }
		public LandingNodeViewModel(FashionStoreSubLandingNodeContent category, string language)
			: this(language)
		{
			// main category
			string mainCategory = "";
			var mainCategoryNode = category.GetParent();
			if (mainCategoryNode != null)
			{
				mainCategory = mainCategoryNode.Name;
			}

			if (!string.IsNullOrEmpty(mainCategory))
			{
				_query = _query.Filter(x => x.MainCategoryName.Match(mainCategory));
			}

			_query = _query.Filter(x => x.ParentCategoryName.Match(category.Name));


		}

		public LandingNodeViewModel(FashionStoreLandingNodeContent mainCategory, HttpRequestBase httpRequest)
			: this(mainCategory, GetLanguage(httpRequest)) { }
		public LandingNodeViewModel(FashionStoreLandingNodeContent mainCategory, string language)
			: this(language)
		{
			_query = _query.Filter(x => x.MainCategoryName.Match(mainCategory.Name));
		}

		private static string GetLanguage(HttpRequestBase httpRequest)
		{
			string language = null;
			if (httpRequest.RequestContext.RouteData.Values["language"] != null)
			{
				language = httpRequest.RequestContext.RouteData.Values["language"].ToString();
			}
			if (string.IsNullOrEmpty(language))
			{
				language = EPiServer.Globalization.ContentLanguage.PreferredCulture.Name;
			}
			return language;
		}
	}
}
