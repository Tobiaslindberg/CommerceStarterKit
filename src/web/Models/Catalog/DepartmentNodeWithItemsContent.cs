/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.DataAnnotations;

namespace OxxCommerceStarterKit.Web.Models.Catalog
{
    [CatalogContentType(GUID = "C8E2094B-E969-4ACA-8174-D95BC0C68F48", MetaClassName = "DepartmentNodeWithItems")]
    [AvailableContentTypes(Include = new[] { typeof(VariationContent), typeof(NodeContent) })]
    public class DepartmentNodeWithItemsContent : NodeContent
    {
    }
}
