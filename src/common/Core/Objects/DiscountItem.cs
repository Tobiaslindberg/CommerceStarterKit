/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using Mediachase.Commerce.Orders;

namespace OxxCommerceStarterKit.Core.Objects
{
	public class DiscountItem
	{
		public string Code { get; set; }
		public decimal Amount { get; set; }

		public DiscountItem(Discount discount)
		{
			Code = discount.DiscountCode;
			Amount = discount.DiscountValue;
		}
	}
}
