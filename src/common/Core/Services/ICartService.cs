/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using OxxCommerceStarterKit.Core.Objects;

namespace OxxCommerceStarterKit.Core.Services
{
    public interface ICartService
    {
        CartActionResult AddToCart(LineItem lineItem);
		CartActionResult AddToWishList(LineItem lineItem);
        List<LineItem> GetItems(string cart, string language);
		CartActionResult UpdateCart(string name, LineItem product);
        decimal GetTotal(string name);
        decimal GetTotalAmount(string name);
		decimal GetTotalLineItemsAmount(string name);
		decimal GetTotalDiscount(string name);
		decimal GetTax(string name);
		decimal GetShipping(string name);
		List<DiscountItem> GetAllDiscountCodes(string name);

		CartActionResult RemoveFromCart(string name, LineItem product);
		CartActionResult MoveBetweenCarts(string fromName, string toName, LineItem product);
		CartActionResult EmptyCart(string name);
		CartActionResult ValidateCart(string name);
		CartActionResult AddDiscountCode(string name, string code);
        void UpdateShipping(string name);
    }
}
