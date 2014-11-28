/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using EPiServer.DataAnnotations;

namespace OxxCommerceStarterKit.Web.Models.PageTypes
{
	[ContentType(GUID = "3FE3BD9F-7101-49ED-B99D-AACF40459D56",
		DisplayName = "Orders Page",
		Description = "The page shows orders.",
		GroupName = "Commerce System Pages",
		AvailableInEditMode = false,
		Order = 100)]
	public class OrdersPage : CommerceSampleModulePage
	{
	}
}
