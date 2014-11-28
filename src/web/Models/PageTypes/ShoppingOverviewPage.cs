/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.ComponentModel.DataAnnotations;
using EPiServer.Commerce;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using OxxCommerceStarterKit.Core.Attributes;

namespace OxxCommerceStarterKit.Web.Models.PageTypes
{
    [ContentType(GUID = "A92C8A05-54A1-4356-BB1E-35B8AFF283A5",
                  DisplayName = "Shopping Page",
                  GroupName = "Commerce System Pages",
                  Order = 100,
				  AvailableInEditMode = false,
                  Description = "The shopping page.")]
    [SiteImageUrl]
    public class ShoppingOverviewPage : SitePage
    {
        [Required]
        [Searchable(false)]
        [BackingType(typeof(PropertyString))]
        [Display(  Name = "PageTitle",
                   Description = "PageTitle",
                   GroupName = SystemTabNames.Content,
                   Order = 1)]
        public virtual string PageTitle { get; set; }

        [Searchable(false)]
        [Display(  Name = "PageSubHeader",
                   Description = "PageSubHeader",
                   GroupName = SystemTabNames.Content,
                   Order = 2)]
        public virtual XhtmlString PageSubHeader { get; set; } 
         
        [Searchable(false)]
        [Display(  Name = "BodyMarkup",
                   Description = "BodyMarkup",
                   GroupName = SystemTabNames.Content,
                   Order = 3)]
        public virtual XhtmlString BodyMarkup { get; set; }

        [Searchable(false)]
        [Display(Name = "CatalogEntryPoint", Description = "Where to start browsing the catalog", GroupName = SystemTabNames.Content, Order = 10)]
        [UIHint(UIHint.CatalogContent)]
        public virtual ContentReference CatalogEntryPoint { get; set; }
    }
}
