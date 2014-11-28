/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;
using EPiServer.Web.Mvc;
using OxxCommerceStarterKit.Web.Models.PageTypes;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Controllers
{
	public class ShoppingCategoryController : PageController<ShoppingCategoryPage>
	{
		IContentLoader contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
		public ActionResult Index(ShoppingCategoryPage currentPage)
		{
			var model = new ShoppingCategoryViewModel(currentPage);
			model.Language = currentPage.Language.Name;
			
		    if (currentPage.CatalogNodes != null)
		    {
		        model.CommerceCategoryIds = GetCommerceNodeIds(currentPage);
		    }
			model.NumberOfProductsToShow = currentPage.NumberOfProductsToShow > 0 ? currentPage.NumberOfProductsToShow : 9;

			if (currentPage.ParentLink != null)
			{
				var languageSelector = new LanguageSelector(model.Language);
				var parent = contentLoader.Get<IContent>(currentPage.ParentLink, languageSelector);
				ShoppingCategoryPage topNode = null;
				while (!(parent is HomePage))
				{
					topNode = parent as ShoppingCategoryPage;
					parent = contentLoader.Get<IContent>(parent.ParentLink, languageSelector);
				}

				if (topNode == null && parent is HomePage)
				{
					topNode = currentPage;
					model.CommerceCategoryIds = string.Empty;
				}
				if (topNode != null)
				{
					model.ParentName = topNode.Name;
					model.CategoryPages = contentLoader.GetChildren<ShoppingCategoryPage>(topNode.ContentLink);
					model.CommerceRootCategoryName = GetCommerceNodeNames(topNode);
				}
			}

			return View(model);
		}
		private string GetMainCategoryFromParentPage(ContentReference parentReference, CultureInfo languageInfo)
		{
			var parentPage = contentLoader.Get<PageData>(parentReference, new LanguageSelector(languageInfo.Name)) as ShoppingCategoryPage;
		    if (parentPage != null)
		    {
				return GetCommerceNodeNames(parentPage);
		    }
		    return string.Empty;
		}

		private string GetCommerceNodeNames(ShoppingCategoryPage pageData)
		{
			if (pageData.CatalogNodes != null)
			{
				return string.Join(",", pageData.CatalogNodes.Select(x => x.Text).ToArray());
			}
			return string.Empty;
		}

		private string GetCommerceNodeIds(ShoppingCategoryPage pageData)
	    {
            if (pageData.CatalogNodes != null)
            {
                string commerceCategories = string.Empty;
                foreach (LinkItem catalogNodeLinkItem in pageData.CatalogNodes)
                {
					string linkUrl;
					if (!EPiServer.Web.PermanentLinkMapStore.TryToMapped(catalogNodeLinkItem.Href, out linkUrl))
						continue;

					if (string.IsNullOrEmpty(linkUrl))
						continue;

					PageReference pageReference = PageReference.ParseUrl(linkUrl);

					string id = pageReference.ID.ToString();

					if (string.IsNullOrEmpty(commerceCategories))
					{
						commerceCategories = id;
					}
					else
					{
						commerceCategories += "," + id;
					}

                }
                return commerceCategories;
            }
            return string.Empty;
	    }
	}
}
