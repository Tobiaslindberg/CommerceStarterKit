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
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using log4net;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Inventory;
using Mediachase.Commerce.Pricing;
using OxxCommerceStarterKit.Core;
using OxxCommerceStarterKit.Core.Extensions;
using OxxCommerceStarterKit.Core.Models;
using OxxCommerceStarterKit.Web.Models.Catalog;
using OxxCommerceStarterKit.Web.Models.FindModels;

namespace OxxCommerceStarterKit.Web.Helpers
{
	[ServiceConfiguration()]
	public class FindHelper
	{
		private const string NoImagePath = "/siteassets/system/no-image.png";
		private readonly IContentLoader _contentLoader = null;
		private readonly ILinksRepository _linksRepository = null;
		private ReadOnlyPricingLoader _priceLoader = null;
		private UrlResolver _urlResolver = null;
		private IPermanentLinkMapper _permanentLinkMapper = null;
		private readonly ReferenceConverter _referenceConverter = null;
		private IPriceService _priceService = null;
		private IWarehouseInventoryService _inventoryService = null;
		private ILog _log = LogManager.GetLogger(typeof(FindHelper));

		public FindHelper(ReadOnlyPricingLoader priceLoader, ILinksRepository linksRepository, IContentLoader contentLoader, UrlResolver urlResolver, IPermanentLinkMapper permanentLinkMapper, ReferenceConverter referenceConverter, IPriceService priceService, IWarehouseInventoryService inventoryService)
		{
			_priceLoader = priceLoader;
			_linksRepository = linksRepository;
			_contentLoader = contentLoader;
			_urlResolver = urlResolver;
			_permanentLinkMapper = permanentLinkMapper;
			_referenceConverter = referenceConverter;
			_priceService = priceService;
			_inventoryService = inventoryService;
		}




  
        private List<CatalogContentBase> GetProductCategories(CatalogContentBase productContent, string language)
		{
			var allRelations = _linksRepository.GetRelationsBySource(productContent.ContentLink);
			var categories = allRelations.OfType<NodeRelation>().ToList();
			List<CatalogContentBase> parentCategories = new List<CatalogContentBase>();
			try
			{
				if (categories.Any())
				{
					foreach (var nodeRelation in categories)
					{
						if (nodeRelation.Target != _referenceConverter.GetRootLink())
						{
							CatalogContentBase parentCategory =
								_contentLoader.Get<CatalogContentBase>(nodeRelation.Target,
									new LanguageSelector(language));
							if (parentCategory != null)
							{
								parentCategories.Add(parentCategory);
							}
						}
					}
				}
				else if (productContent.ParentLink != null && productContent.ParentLink != _referenceConverter.GetRootLink())
				{
					CatalogContentBase parentCategory =
					  _contentLoader.Get<CatalogContentBase>(productContent.ParentLink,
					  new LanguageSelector(language));
					parentCategories.Add(parentCategory);
				}
			}
			catch (Exception ex)
			{
				_log.Debug(string.Format("Failed to get categories from product {0}, Code: {1}.", productContent.Name, productContent.ContentLink), ex);
			}
			return parentCategories;
		}



		private bool IsInStock(IEnumerable<FashionVariant> variations)
		{
			return variations.Any(x => x.Stock > 0);
		}


		private string GetDefaultImage(FashionProductContent productContent, UrlResolver urlResolver, IPermanentLinkMapper permanentLinkMapper)
		{
			var commerceMedia = productContent.CommerceMediaCollection.OrderBy(m => m.SortOrder).FirstOrDefault(z => z.GroupName != null && z.GroupName.ToLower() != "swatch");
			if (commerceMedia != null)
			{
			    var contentReference = commerceMedia.AssetContentLink(permanentLinkMapper);
				return urlResolver.GetUrl(contentReference);
			}
			return NoImagePath;
		}

		private string GetDefaultImage(IEnumerable<FindProduct> productList)
		{

			FindProduct firstProduct = productList.FirstOrDefault(x => !string.IsNullOrEmpty(x.DefaultImageUrl));
			if (firstProduct != null)
				return firstProduct.DefaultImageUrl;
			return NoImagePath;
		}
	}
}
