/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Website.Helpers;
using OxxCommerceStarterKit.Web.Models.PageTypes;

namespace OxxCommerceStarterKit.Web.Models.ViewModels
{
    public class WishListModel : PageViewModel<WishListPage>
    {
        public WishListModel(WishListPage currentPage)
            : base(currentPage)
        {
        }

        public IEnumerable<LineItem> LineItems
        {
            get
            {
                var wishListHelper = new CartHelper(CartHelper.WishListName);
                bool isEmpty = wishListHelper.IsEmpty;

                // Make sure to check that prices has not changed
                if (!isEmpty)
                {

                    //wishListHelper.Reset();
                    //wishListHelper.Cart.ProviderId = "FrontEnd";
                   // wishListHelper.RunWorkflow("CartValidate"); //Wishlists should not be validated.

                    // If cart is empty, remove it from the database
                    isEmpty = wishListHelper.IsEmpty;
                    if (isEmpty)
                    {
                        wishListHelper.Delete();
                    }
                    wishListHelper.Cart.AcceptChanges();
                }

                return wishListHelper.LineItems;
            }
        }
    }
}
