/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using System.Web.Routing;
using EPiServer.Shell.Navigation;

namespace OxxCommerceStarterKit.Web.Plugins.CommerceTools
{
    [MenuProvider]
    public class CommerceToolsMenuProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems()
        {
               
            UrlMenuItem urlMenuItem = new UrlMenuItem("Price Update", "/global/commerce/priceupdate",
                "/plugins/commercetools/prices/updateprices.aspx");
            urlMenuItem.IsAvailable = ((RequestContext request) => true);
            urlMenuItem.SortIndex = 100;

            return new MenuItem[]
			{				
				urlMenuItem                
			};
        }
    }   
}
