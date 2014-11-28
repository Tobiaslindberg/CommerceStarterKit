/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.ComponentModel.DataAnnotations;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace OxxCommerceStarterKit.Web.Models.PageTypes
{
    [ContentType(GUID = "7b6d05bd-d941-4c12-aad1-e5d30b6e12eb",
        DisplayName = "Search Page",
        GroupName = "Commerce System Pages",
        Order = 100,
		AvailableInEditMode = false, 
		Description = "A page which is used to show search result.")]
    public class SearchPage : SitePage
    {
        [Searchable(false)]
        [Display(Name = "Number of items to show", Description = "If not set, 9 is default", GroupName = SystemTabNames.Content, Order = 20)]
        public virtual int NumberOfProductsToShow { get; set; }

        [Searchable(false)]
        [Display(Name = "Number of articles to show", Description = "If not set, 5 is default", GroupName = SystemTabNames.Content, Order = 30)]
        public virtual int NumberOfArticlesToShow { get; set; }
    }
}
