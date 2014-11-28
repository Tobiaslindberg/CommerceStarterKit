/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;

namespace OxxCommerceStarterKit.Web.Models.Catalog
{
    [CatalogContentType(GUID = "7AB5BB63-9B81-4D40-91FA-5551F985004D", MetaClassName = "DepartmentNodeWithProducts")]
    public class DepartmentNodeWithProductsContent : NodeContent
    {
    }
}
