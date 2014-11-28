/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.SpecializedProperties;

namespace OxxCommerceStarterKit.Web.Models.ViewModels
{
    public interface IVariationViewModel<out TVariationContent> : IBaseCatalogViewModel<TVariationContent>
        where TVariationContent : VariationContent        
    {
        Lazy<Inventory> Inventory { get; set; }
        Price Price { get; set; }
        EntryContentBase ParentEntry { get; set; }
        EntryContentBase ContentWithAssets { get; set; }


        PriceModel PriceViewModel { get; set; }
    }
}
