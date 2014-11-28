/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using EPiServer.DataAnnotations;
using OxxCommerceStarterKit.Core.Attributes;

namespace OxxCommerceStarterKit.Web.Models.PageTypes
{
    [ContentType(GUID = "3FE3BD9F-7101-49ED-B99D-AACF40459374",
         DisplayName = "WishList Page",
         Description = "The page shows user's wish list.",
         GroupName = "Commerce System Pages",
		AvailableInEditMode = false, 
		Order = 100)]
    [SiteImageUrl]
    public class WishListPage : CommerceSampleModulePage
    {
    }
}
