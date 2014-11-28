/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Linq;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Website.Helpers;
using OxxCommerceStarterKit.Core.Objects;
using LineItem = OxxCommerceStarterKit.Core.Objects.LineItem;

namespace OxxCommerceStarterKit.Web.Api
{
    public class CheckoutController : BaseApiController
    {
        public ShoppingCart GetCart()
        {
            CartHelper ch = new CartHelper(Cart.DefaultName);


            var items = ch.LineItems.Select(lineItem => new LineItem() {Name = lineItem.DisplayName}).ToList();

            return new ShoppingCart()
            {
                Items = items,
                Items2 = ch.LineItems
            };
        }
    }
}
