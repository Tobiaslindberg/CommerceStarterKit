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

namespace OxxCommerceStarterKit.Web.Models.PageTypes
{
    public class CommerceSampleModulePage : SitePage
    {
        [Display(
            Name = "PageSubHeader",
            Description = "PageSubHeader",
            GroupName = SystemTabNames.Content,
            Order = 2)]
        [Searchable(false)]
        [CultureSpecific]
        public virtual XhtmlString PageSubHeader { get; set; }
    }
}
