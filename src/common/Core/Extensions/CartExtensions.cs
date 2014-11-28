/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using Mediachase.Commerce.Orders;

namespace OxxCommerceStarterKit.Core.Extensions
{
    public static class CartExtensions
    {
        public static string GeneratePredictableOrderNumber(this Cart cart)
        {
            return "PO" + cart.OrderGroupId;
        }



    }
}
