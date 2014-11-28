/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.ComponentModel.DataAnnotations;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;
using OxxCommerceStarterKit.Core.Attributes;
using OxxCommerceStarterKit.Web.Business.Rendering;

namespace OxxCommerceStarterKit.Web.Models.Blocks
{
	[ContentType(GUID = "c1a6d014-fa3f-4786-b35d-fa78940a1fdd",
        DisplayName = "Carousel Block",
        Description = "Slides images into view."
        )]
	[SiteImageUrl]
    public class SliderBlock : SiteBlockData, IDefaultDisplayOption
	{

		[Display(
			GroupName = SystemTabNames.Content,
			Order = 10)]
		[Searchable(false)]
		public virtual LinkItemCollection Images { get; set; }

		[ScaffoldColumn(false)]
	    public string Tag {
	        get { return string.Empty; }
	    }
	}
}
