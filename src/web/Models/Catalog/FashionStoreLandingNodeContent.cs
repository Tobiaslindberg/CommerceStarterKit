/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.ComponentModel.DataAnnotations;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.DataAnnotations;
using EPiServer.Shell.ObjectEditing;
using OxxCommerceStarterKit.Core.Models;
using OxxCommerceStarterKit.Web.Models.PageTypes;

namespace OxxCommerceStarterKit.Web.Models.Catalog
{
    [CatalogContentType(GUID = "35A29D99-3531-4E4B-A40E-EF262E9DB8B5", MetaClassName = "FashionStoreLandingNode")]
    [AvailableContentTypes(Include = new Type[] { typeof(FashionProductContent), typeof(FashionItemContent), typeof(NodeContent) })]
    public class FashionStoreLandingNodeContent : SiteCategoryContent
    {


		[Display(Name = "Size guide", Order = 10)]
		[AllowedTypes(new Type[] { typeof(ArticlePage), typeof(BlogPage) })]
		public virtual Url SizeGuide { get; set; }
    }
}
