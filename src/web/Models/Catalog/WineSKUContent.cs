/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Web.Routing;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Search;
using OxxCommerceStarterKit.Core.Extensions;
using OxxCommerceStarterKit.Web.EditorDescriptors.SelectionFactories;
using OxxCommerceStarterKit.Web.Models.Blocks.Contracts;
using OxxCommerceStarterKit.Web.Models.FindModels;
using OxxCommerceStarterKit.Web.Models.ViewModels;
using OxxCommerceStarterKit.Web.Services;

namespace OxxCommerceStarterKit.Web.Models.Catalog
{
    [CatalogContentType(GUID = "B96A3924-D07D-4ECA-BFA3-C0C3138CD138", MetaClassName = "WineSKU", 
        DisplayName = "Wine",
        Description = "A variation with information about wine.",
        GroupName = "Wine"
        )]
    public class WineSKUContent : VariationContent, IFacetBrand, IInfoModelNumber, IIndexableContent, IProductListViewModelInitializer
    {

        [Display(Name = "Recommendation Boost",
            Description = "An integer that can be used to boost the visibility of products in recommendation lists.",
            Order = 5)]
        public virtual int RecommendBoost { get; set; }

        [Display(Name = "Show in product list",
            Description = "Default is true, set to false to hide product from lists. The product can still be linked to and found through search.",
           Order = 7)]
        [DefaultValue(true)]
        public virtual bool ShowInList { get; set; }

        [Display(Name = "Model Number", Order = 9)]
        public virtual string Info_ModelNumber { get; set; }

        // Multi lang
        [Display(Name = "Description", Order = 10)]
        [CultureSpecific]
        [Searchable]
        public virtual XhtmlString Info_Description { get; set; }

        [Display(Name = "Color",
            Order = 12)]
        [Searchable]
        [SelectOne(SelectionFactoryType = typeof(WineColorSelectionFactory))]
        public virtual string Color { get; set; }

        // Same for all languages, this could give us
        // trouble later on, as names of countries are
        // translated. However for the demo, we use this
        // for facets.
        // TODO: Verify i18n
        [Display(Name = "Country",
            Order = 15)]
        [Searchable]
        public virtual string Country { get; set; }

        // Same for all languages
        [Display(Name = "Region",
            Order = 16)]
        [Searchable]
        public virtual string Region { get; set; }

        // Same for all languages
        [Display(Name = "Facet Brand",
            Order = 18)]
        public virtual string Facet_Brand { get; set; }

        // Same for all languages
        [Display(Name = "Vintage",
            Description = "Typically the year of the harvest",
            Order = 20)]
        [Searchable]
        [Required]
        public virtual string Vintage { get; set; }

        // Same for all languages
        [Display(Name = "Grape mix",
            Description = "Example: Chardonnay 40%, Pinot Noir 60%",
            Order = 25)]
        [Searchable]
        public virtual string GrapeMix { get; set; }

        // Same for all languages
        [Display(Name = "Alcohol",
            Description = "The alcoholic percentage",
            Order = 30)]
        [Searchable]
        public virtual decimal Alcohol { get; set; }

        [Display(Name = "Maturity",
            Description = "Maturity in a wine relates to its readiness for drinking. Example: Drink now and until 2022",
            Order = 35)]
        [CultureSpecific]
        [Searchable]
        public virtual string Maturity { get; set; }

        [Display(Name = "Taste",
            Description = "The use of wine tasting descriptors allows the taster to qualitatively relate the aromas and flavors that the taster experiences and can be used in assessing the overall quality of wine",
            Order = 40)]
        [CultureSpecific]
        [Searchable]
        public virtual string Taste { get; set; }

        [Display(Name = "Style",
            Description = "A broader style of categorization. See http://en.wikipedia.org/wiki/Category:Wine_styles for more information",
            Order = 45)]
        [CultureSpecific]
        [Searchable]
        public virtual string Style { get; set; }

        [Display(Name = "Closure",
            Description = "Closure is a term used in the wine industry to refer to a stopper, the object used to seal a bottle and avoid harmful contact between the wine and oxygen.[1]",
            Order = 50)]
        [Searchable]
        [SelectOne(SelectionFactoryType = typeof(WineClosureSelectionFactory))]
        public virtual string Closure { get; set; }

        [Display(Name = "Size",
            Description = "A descriptive version of the size",
           Order = 55)]
        [SelectOne(SelectionFactoryType = typeof(WineSizeSelectionFactory))]
        [DefaultValue("Standard (750ml)")]
        [UIHint("FloatingEditor")]
        public virtual string Size { get; set; }

        [Display(Name = "Size in ml", Order = 56)]
        [DefaultValue(750)]
        public virtual int SizeML { get; set; }

        public FindProduct GetFindProduct(IMarket market)
        {
            var language = (Language == null ? string.Empty : Language.Name);
            var findProduct = new WineFindProduct(this, language);

            findProduct.Sizes = new List<string>() { this.Size };
            findProduct.ShowInList = ShowInList;
            EPiServer.Commerce.SpecializedProperties.Price defaultPrice = this.GetDefaultPrice();
            findProduct.DefaultPrice = this.GetDisplayPrice(market);
            findProduct.DefaultPriceAmount = this.GetDefaultPriceAmount(market);
            findProduct.DiscountedPrice = this.GetDiscountDisplayPrice(defaultPrice, market);
            findProduct.CustomerClubPrice = this.GetCustomerClubDisplayPrice(market);
            findProduct.GrapeMixList = GetGrapeMixList();

            // Wine does not have variations. It is a variation

            return findProduct;
        }

        /// <summary>
        /// A typical property contains something like this: "Gruner 40%, Pinot Noir 60%"
        /// We parse it and remove numbers and percentages
        /// </summary>
        /// <returns>A list of grapes</returns>
        private List<string> GetGrapeMixList()
        {
            string value = GrapeMix;

            if (string.IsNullOrEmpty(value))
                return new List<string>();

            //remove percentage and numbers
            value = Regex.Replace(value, @"\d*%", "");

            return value.Split(',').Select(a => a.Trim()).ToList();
        }


        public bool ShouldIndex()
        {
            return !(StopPublish != null && StopPublish < DateTime.Now);
        }

        public ProductListViewModel Populate(IMarket market)
        {
            UrlResolver urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();

            ProductListViewModel productListViewModel = new ProductListViewModel
            {
                Code = this.Code,
                ContentLink = this.ContentLink,
                DisplayName = this.DisplayName,
                Description = Info_Description,
                ProductUrl = urlResolver.GetUrl(ContentLink),
                ImageUrl = this.GetDefaultImage(),
                PriceString = this.GetDisplayPrice(market),
                BrandName = Facet_Brand,
                Country = Country
            };
            ICurrentMarket currentMarket = ServiceLocator.Current.GetInstance<ICurrentMarket>();
            productListViewModel.PriceAmount = this.GetDefaultPriceAmount(currentMarket.GetCurrentMarket());
            return productListViewModel;
        }
    }


}
