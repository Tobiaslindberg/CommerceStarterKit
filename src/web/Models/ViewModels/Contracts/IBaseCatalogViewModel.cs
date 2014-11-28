/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using EPiServer.Commerce.Catalog.ContentTypes;

namespace OxxCommerceStarterKit.Web.Models.ViewModels
{
    public interface IBaseCatalogViewModel<out T>
        where T : CatalogContentBase        
    {
        T CatalogContent { get; }
    }
}
