/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.ComponentModel.DataAnnotations;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.DataAnnotations;

namespace OxxCommerceStarterKit.Web.Models.Catalog
{
    [CatalogContentType(DisplayName = "Fashion Variant",
        GUID = "BE40A3E0-49BC-48DD-9C1D-819C2661C9BD", 
        MetaClassName = "Fashion_Item_Class")]
    public class FashionItemContent : VariationContent
    {
        [Display(Name = "Size")]
        // ReSharper disable once InconsistentNaming
        public virtual string Facet_Size { get; set; }

        [CultureSpecific]
        [Display(Name = "Color")]
        public virtual string FacetColor { get; set; }

    }
}
