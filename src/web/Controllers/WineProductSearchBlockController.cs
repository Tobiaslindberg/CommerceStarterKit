/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Globalization;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using OxxCommerceStarterKit.Core.Extensions;
using OxxCommerceStarterKit.Web.Business.Analytics;
using OxxCommerceStarterKit.Web.Models.Blocks;
using OxxCommerceStarterKit.Web.Models.Blocks.Contracts;
using OxxCommerceStarterKit.Web.Models.Catalog;
using OxxCommerceStarterKit.Web.Models.ViewModels;
using OxxCommerceStarterKit.Web.Services;

namespace OxxCommerceStarterKit.Web.Controllers
{
    public class WineProductSearchBlockController : BlockController<WineProductSearchBlock>
    {

        private readonly IContentLoader _contentLoader;
        private readonly ProductService _productService;

        public class WineProductSearchResult
        {
            public string Heading { get; set; }
            public List<ProductListViewModel> Products { get; set; }
        }


        public WineProductSearchBlockController(IContentLoader contentLoader, ProductService productService)
        {
            _contentLoader = contentLoader;
            _productService = productService;

        }

        // GET: RelatedProductsBlock
        public override ActionResult Index(WineProductSearchBlock currentContent)
        {
            try
            {
                // We need to know which language the page we're hosted on is
                string language = ControllerContext.RequestContext.GetLanguage();

                List<ProductListViewModel> productListViewModels = currentContent.GetSearchResults(language);

                if (productListViewModels == null)
                {
                    // No hits, but we could still have manually added products
                    productListViewModels = new List<ProductListViewModel>();
                }

                // Override result with priority products
                if (currentContent.Products != null)
                {
                    foreach (var contentAreaItem in currentContent.Products.Items)
                    {
                        var item = contentAreaItem.GetContent();
                        if (item != null)
                        {
                            WineSKUContent wineSkuContent = _contentLoader.Get<WineSKUContent>(item.ContentLink);
                            if (wineSkuContent != null)
                            {
                                // Remove priority products from list
                                productListViewModels.RemoveAll(
                                    x => x.ContentLink.CompareToIgnoreWorkID(wineSkuContent.ContentLink));
                                // Add to beginning
                                ProductListViewModel priorityProduct =
                                    _productService.GetProductListViewModel(wineSkuContent);
                                productListViewModels.Insert(0, priorityProduct);
                            }
                        }
                    }
                }

                if (productListViewModels.Count > currentContent.ResultsPerPage)
                {
                    productListViewModels = productListViewModels.Take(currentContent.ResultsPerPage).ToList();
                }

                WineProductSearchResult wineProductSearchResult = new WineProductSearchResult
                {
                    Heading = currentContent.Heading,
                    Products = productListViewModels
                };


                // Track impressions
                TrackProductImpressions(wineProductSearchResult);


                return View(wineProductSearchResult);
            }
            catch (ServiceException)
            {
                return View("FindError");
            }
        }

        private void TrackProductImpressions(WineProductSearchResult wineProductSearchResult)
        {
            foreach (var product in wineProductSearchResult.Products)
            {
                GoogleAnalyticsTracking tracker = new GoogleAnalyticsTracking(ControllerContext.HttpContext);
                tracker.ProductImpression(
                    product.Code,
                    product.DisplayName,
                    null,
                    product.BrandName,
                    null,
                    "Wine Product Search Block");

            }
        }
    }
}
