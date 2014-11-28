/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;

namespace OxxCommerceStarterKit.Core.Objects
{
    public class ShoppingCart
    {
        public IEnumerable<LineItem> Items { get; set; }
        public IEnumerable<Mediachase.Commerce.Orders.LineItem> Items2 { get; set; }
    }
}
