/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using OxxCommerceStarterKit.Core.Attributes;

namespace OxxCommerceStarterKit.Web.Models.PageTypes
{
    [ContentType( GUID = "760AEE27-E362-49EE-BFAC-4FE32F2C4EFB",
                  DisplayName = "Default Page",
                  GroupName = "CommerceSample",
				  AvailableInEditMode = false,
				  Order = 100,
                  Description = "Marked for DELETION")]
    [SiteImageUrl]
    public class DefaultPage : SitePage
    {
        [Required]
        [Searchable(false)]
        [CultureSpecific]
        [Display(  Name = "Page Title",
                   Description = "Title",
                   GroupName = SystemTabNames.Content,
                   Order = 1)]
        public virtual string PageTitle { get; set; }

        [Searchable(false)]
        [Display(  Name = "Sub Header",
                   Description = "Secondary Header",
                   GroupName = SystemTabNames.Content,
                   Order = 2)]
        public virtual XhtmlString PageSubHeader { get; set; } 
         
        [Searchable(false)]
        [Display(  Name = "Main Content",
                   Description = "Main Content",
                   GroupName = SystemTabNames.Content,
                   Order = 3)]
        public virtual XhtmlString BodyMarkup { get; set; } 
    }
}
