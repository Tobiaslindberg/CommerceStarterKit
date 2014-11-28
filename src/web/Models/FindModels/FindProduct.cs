/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using OxxCommerceStarterKit.Core.Extensions;
using OxxCommerceStarterKit.Core.Models;
using OxxCommerceStarterKit.Web.Models.Catalog;

namespace OxxCommerceStarterKit.Web.Models.FindModels
{
    public class FindProduct
    {
        private static Injected<UrlResolver> urlResolverInjected;

        public FindProduct()
        {
            
        }

        public FindProduct(EntryContentBase entryContentBase, string language)
        {
            IndexId = entryContentBase.ContentLink.ID + "_" + language;
            Id = entryContentBase.ContentLink.ID;
            Name = entryContentBase.Name;
            DisplayName = entryContentBase.DisplayName;
            Language = language;
            Description = Description ?? null;
            ParentCategoryId = entryContentBase.GetProductCategoryIds(language);
            ParentCategoryName = entryContentBase.GetParentCategoryNames(language);
            MainCategoryName = entryContentBase.GetMainCategory(language);
            CategoryName = entryContentBase.GetCategoryName(language);
            ProductUrl = urlResolverInjected.Service.GetUrl(entryContentBase.ContentLink, language);
            DefaultImageUrl = entryContentBase.GetDefaultImage();
         
        }

        [Id]
        public string IndexId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Language { get; set; }
        public XhtmlString Description { get; set; }
        public List<string> Color { get; set; }
        public string DescriptiveColor { get; set; }
        public List<string> Sizes { get; set; }
        public string SizeUnit { get; set; }
		public string SizeType { get; set; }
        public string Fit { get; set; }
        public List<string> SizesList { get; set; } 
        public List<string> ParentCategoryName { get; set; }
        public List<int> ParentCategoryId { get; set; }
        public string MainCategoryName { get; set; }
        public string CategoryName { get; set; }
        public string DefaultImageUrl { get; set; }
        // List of images - wrap in ImageInfo (url, type of image, etc.)
        public string ProductUrl { get; set; }
        public bool ShowInList { get; set; }
        public string DefaultPrice { get; set; }
        /// <summary>
        /// Used for filtering on range, is rounded to int
        /// </summary>
        public int DefaultPriceAmount { get; set; }
        public string DiscountedPrice { get; set; }
        public decimal DiscountedPriceAmount { get; set; }
        public List<FashionVariant> Variants { get; set; }
		public string NewItemText { get; set; }
		public int SalesCounter { get; set; }
		public string CustomerClubPrice { get; set; }
        public string Brand { get; set; }
        //Wine
        public string Varieties { get; set; }
        public string Vintage { get; set; }
        public string Taste { get; set; }
        public string Style { get; set; }
        public string Region { get; set; }
        public string Maturity { get; set; }
        public decimal Alcohol { get; set; }
        public string Closure { get; set; }
        public List<string> GrapeMixList { get; set; }
        public string Country { get; set; }
     
    }

    public class WineFindProduct : FindProduct
    {
        public WineFindProduct()
        {
            
        }

        public WineFindProduct(EntryContentBase entryContentBase, string language) : base(entryContentBase, language)
        {
            WineSKUContent wineSkuContent = entryContentBase as WineSKUContent;
            if (wineSkuContent != null)
            {
                Varieties = wineSkuContent.GrapeMix;
                Vintage = wineSkuContent.Vintage;
                Taste = wineSkuContent.Taste;
                Style = wineSkuContent.Style;
                Country = wineSkuContent.Country;
                Region = wineSkuContent.Region;
                Maturity = wineSkuContent.Maturity;
                Alcohol = wineSkuContent.Alcohol;
                Closure = wineSkuContent.Closure;
                Brand = wineSkuContent.Facet_Brand;

            }
        }

       
    }

}
