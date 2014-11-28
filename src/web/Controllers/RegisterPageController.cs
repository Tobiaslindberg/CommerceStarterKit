/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using EPiServer.Core;
using EPiServer.Editor;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Mediachase.BusinessFoundation.Data;
using Mediachase.Commerce;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Security;
using OxxCommerceStarterKit.Core;
using OxxCommerceStarterKit.Core.Extensions;
using OxxCommerceStarterKit.Core.Objects;
using OxxCommerceStarterKit.Core.Repositories.Interfaces;
using OxxCommerceStarterKit.Web.Models.PageTypes;
using OxxCommerceStarterKit.Web.Models.ViewModels;
using OxxCommerceStarterKit.Web.Services.Email;

namespace OxxCommerceStarterKit.Web.Controllers
{
	public class RegisterPageController : PageControllerBase<RegisterPage>
	{
		private readonly UrlResolver _urlResolver;
		private readonly LocalizationService _localizationService;
		private readonly ICurrentMarket _currentMarket;

		public RegisterPageController(UrlResolver urlResolver, LocalizationService localizationService, ICurrentMarket currentMarket)
		{
			_urlResolver = urlResolver;
			_localizationService = localizationService;
			_currentMarket = currentMarket;
		}

		public ActionResult Index(RegisterPage currentPage)
		{
			RegisterPageViewModel model = new RegisterPageViewModel(currentPage);
			model.RegisterForm.AvailableCategories = GetAvailableCategories();

			if (PageEditing.PageIsInEditMode)
			{
				return View("Edit", model);
			}

			if (Request.IsAjaxRequest())
			{
				return PartialView(model);
			}

			return View("Index", model);
		}

		private Dictionary<string, string> GetAvailableCategories()
		{
			var output = new Dictionary<string, string>();

			// get the Category type from the BusinessFoundation
			var category = DataContext.Current.GetMetaFieldType(Constants.Metadata.Customer.Category);
			if (category != null)
			{
				if (category.EnumItems != null)
				{
					foreach (var item in category.EnumItems.OrderBy(x => x.OrderId))
					{
						output.Add(item.Handle.ToString(), item.Name);
					}
				}
			}
			return output;
		}


		[HttpPost]
		public ActionResult Register(RegisterPage currentPage, RegisterPageViewModel model, RegisterForm registerForm, int[] SelectedCategories)
		{
			model.RegisterForm.AvailableCategories = GetAvailableCategories();
			model.RegisterForm.SelectedCategories = SelectedCategories;
			if (registerForm.Password != registerForm.PasswordConfirm)
			{
				ModelState.AddModelError("RegisterForm.ValidationMessage", _localizationService.GetString("/common/validation/compare_passwords"));
			}

			if (!ModelState.IsValid)
			{
				return View("Index", model);
			}

			string emailAddress = registerForm.UserName.Trim();
			string password = registerForm.Password;

			// Account
			MembershipUser user = null;
			MembershipCreateStatus createStatus;
			user = Membership.CreateUser(emailAddress, password, emailAddress, null, null, true, out createStatus);

			bool existingUserWithoutPassword = false;

			if (createStatus == MembershipCreateStatus.DuplicateUserName)
			{
				user = Membership.GetUser(emailAddress);
				var customer1 = CustomerContext.Current.GetContactForUser(user);
				if (customer1.GetHasPassword())
				{
					ModelState.AddModelError("RegisterForm.ValidationMessage", _localizationService.GetString("/common/account/register_error_unique_username"));
				}
				else
				{
					existingUserWithoutPassword = true;
				}
			}
			else if (user == null)
			{
				ModelState.AddModelError("RegisterForm.ValidationMessage", _localizationService.GetString("/common/account/register_error"));
			}

			if (!ModelState.IsValid)
			{
				return View("Index", model);
			}

			if (!existingUserWithoutPassword)
			{
				SecurityContext.Current.AssignUserToGlobalRole(user, AppRoles.EveryoneRole);
				SecurityContext.Current.AssignUserToGlobalRole(user, AppRoles.RegisteredRole);
			}
			else
			{
				// set new password
				var pass = user.ResetPassword();
				user.ChangePassword(pass, password);
			}

			var customer = CustomerContext.Current.GetContactForUser(user);
			customer.FirstName = registerForm.Address.FirstName;
			customer.LastName = registerForm.Address.LastName;
			customer.SetPhoneNumber(registerForm.Phone);
			customer.FullName = string.Format("{0} {1}", customer.FirstName, customer.LastName);
			customer.SetHasPassword(true);

			// member club
			if (registerForm.MemberClub)
			{
				customer.CustomerGroup = Constants.CustomerGroup.CustomerClub;
			}

			// categories
			customer.SetCategories(SelectedCategories);

			customer.SaveChanges();

			var CustomerAddressRepository = ServiceLocator.Current.GetInstance<ICustomerAddressRepository>();
			CustomerAddressRepository.SetCustomer(customer);

			// copy address fields to shipping address
			registerForm.Address.CheckAndSetCountryCode();

			var ShippingAddress = (Address)registerForm.Address.Clone();
			ShippingAddress.IsPreferredShippingAddress = true;
			CustomerAddressRepository.Save(ShippingAddress);

			registerForm.Address.IsPreferredBillingAddress = true;
			CustomerAddressRepository.Save(registerForm.Address);

			LoginController.CreateAuthenticationCookie(ControllerContext.HttpContext, emailAddress, AppContext.Current.ApplicationName, false);

			bool mail_sent = SendWelcomeEmail(registerForm.UserName, currentPage);

			return Redirect(_urlResolver.GetUrl(ContentReference.StartPage));
		}


		public static bool SendWelcomeEmail(string email, RegisterPage currentPage = null)
		{
			var emailService = ServiceLocator.Current.GetInstance<IEmailService>();

			if (currentPage == null)
			{
				var contentLoader = ServiceLocator.Current.GetInstance<EPiServer.IContentLoader>();
				var homePage = contentLoader.Get<HomePage>(ContentReference.StartPage);
				if (homePage != null && homePage.Settings != null && homePage.Settings.LoginPage != null)
				{
					var loginPage = contentLoader.Get<LoginPage>(homePage.Settings.LoginPage);
					if (loginPage != null && loginPage.RegisterPage != null)
					{
						currentPage = contentLoader.Get<RegisterPage>(loginPage.RegisterPage);
					}
				}
			}
			if (currentPage != null)
			{
				return emailService.SendWelcomeEmail(email, currentPage.EmailSubject, currentPage.EmailBody.ToString());
				
			}
			return false;
		}
	}
}
