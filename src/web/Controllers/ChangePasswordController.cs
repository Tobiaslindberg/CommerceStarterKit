/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Web.Mvc;
using Mediachase.Commerce.Customers;
using OxxCommerceStarterKit.Core.Objects;
using OxxCommerceStarterKit.Web.Models.PageTypes;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Controllers
{
	public class ChangePasswordController : PageControllerBase<ChangePasswordPage>
    {

		public ActionResult Index(ChangePasswordPage currentPage)
		{
			var model = new ChangePasswordViewModel(currentPage);

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Index(ChangePasswordPage currentPage, ChangePasswordForm changePasswordForm)
		{
			var model = new ChangePasswordViewModel(currentPage);
			model.ChangePasswordForm = changePasswordForm;

		    if (ModelState.IsValid)
		    {
		        var myUser = CustomerContext.Current.GetUserForContact(CustomerContext.Current.CurrentContact);
		        bool isChangeSuccess = myUser.ChangePassword(
		            changePasswordForm.OldPassword,
		            changePasswordForm.NewPassword.Trim());

                if(!isChangeSuccess)
                    ModelState.AddModelError("WrongPassword","Wrong password");
                else
                {
                    return View("PasswordChanged",model);
                }
		    }

		    return View(model);
		}
    }
}
