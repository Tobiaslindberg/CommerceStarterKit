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
    [ContentType(GUID = "3B579852-D4AD-41D5-B4BA-50FF4CC55A6A",
        DisplayName = "Home Page",
        Description = "The start page of the site",
        GroupName = "Pages",
		AvailableInEditMode = false,
		Order = 100)]
    [SiteImageUrl]
    public class HomePage : SitePage
    {

        [Searchable(false)]
        [Display(
            Name = "Body Content",
            Description = "BodyContent",
            GroupName = SystemTabNames.Content,
            Order = 0)]
        [CultureSpecific]
        public virtual ContentArea BodyContent { get; set; }

        [Display(
            Name = "Site Settings",
            Description = "Global settings for this site.",
            GroupName = "Site Settings",
            Order = 0)]
        public virtual SettingsBlock Settings { get; set; }
    }
}
