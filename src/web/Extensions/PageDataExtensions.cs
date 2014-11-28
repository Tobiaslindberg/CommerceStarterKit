/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using OxxCommerceStarterKit.Web.Business;

namespace OxxCommerceStarterKit.Web.Extensions
{
	public static class PageDataExtensions
	{

		public static IEnumerable<T> GetChildren<T>(this PageData page) where T : PageData
		{
			var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
			return contentLoader.GetChildren<T>(page.ContentLink).FilterForDisplay<T>(true, true);
		}


	}
}
