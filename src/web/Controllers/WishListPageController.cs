/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Web.Mvc;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Website.Helpers;
using OxxCommerceStarterKit.Web.Models.PageTypes;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Controllers
{
    [TemplateDescriptor(Inherited = true)]
    public class WishListPageController : PageController<WishListPage>
    {
        public ViewResult Index(WishListPage currentPage)
        {
            return View(new WishListModel(currentPage));
        }

        [HttpPost]
        public ActionResult AddToWishList(WishListPage currentPage, string code, decimal quantity)
        {
            Entry entry = CatalogContext.Current.GetCatalogEntry(code, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo));

            CartHelper ch = new CartHelper(CartHelper.WishListName);
            ch.AddEntry(entry, quantity, false);
            ch.Cart.AcceptChanges();

            return  RedirectToAction("Index");
        }

        public ActionResult ClearWishList(WishListPage currentPage)
        {
            CartHelper ch = new CartHelper(CartHelper.WishListName);
            ch.Delete();
            ch.Cart.AcceptChanges();

            return RedirectToAction("Index");
        }
    }
}
