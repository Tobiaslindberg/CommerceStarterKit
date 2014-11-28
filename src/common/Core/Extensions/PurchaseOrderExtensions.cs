/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Linq;
using Mediachase.Commerce.Orders;

namespace OxxCommerceStarterKit.Core.Extensions
{
    public static class PurchaseOrderExtensions
    {
        public static string GetBillingEmail(this PurchaseOrder order)
        {
            var address = order.OrderAddresses.FirstOrDefault(a => a.Name == Constants.Order.BillingAddressName);

            if (address == null)
            {
                throw new ApplicationException("Order does not have billing address");
            }

            return address.Email ?? string.Empty;
        }

		public static string GetBillingPhone(this PurchaseOrder order)
		{
			var address = order.OrderAddresses.FirstOrDefault(a => a.Name == Constants.Order.BillingAddressName);

			if (address == null)
			{
				throw new ApplicationException("Order does not have billing address");
			}

			return address.DaytimePhoneNumber ?? string.Empty;
		}
    }
}
