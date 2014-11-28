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
    public class VariationViewModel<TVariationContent> : IVariationViewModel<TVariationContent>
        where TVariationContent : VariationContent        
    {
        public VariationViewModel(TVariationContent currentContent)            
        {
            CatalogContent = currentContent;
        }

        public TVariationContent CatalogContent { get; private set; }

        public Lazy<Inventory> Inventory { get; set; }
        public Price Price { get; set; }
        public EntryContentBase ParentEntry { get; set; }
        public EntryContentBase ContentWithAssets { get; set; }
        public PriceModel PriceViewModel { get; set; }
    }
}
