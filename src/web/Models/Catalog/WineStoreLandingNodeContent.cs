/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.DataAnnotations;
using OxxCommerceStarterKit.Core.Models;

namespace OxxCommerceStarterKit.Web.Models.Catalog
{
    [CatalogContentType(GUID = "EB8059BC-AEFC-46A1-83FC-7C678D5E8258", MetaClassName = "WineStoreLandingNode")]
    [AvailableContentTypes(Include = new Type[] { typeof(WineSKUContent), typeof(NodeContent), typeof(BundleContent), typeof(PackageContent) })]
    public class WineMainLandingNodeContent : SiteCategoryContent
    {
    }
}
