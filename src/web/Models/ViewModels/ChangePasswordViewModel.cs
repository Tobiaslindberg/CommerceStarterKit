/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using OxxCommerceStarterKit.Core.Objects;
using OxxCommerceStarterKit.Core.Repositories;
using OxxCommerceStarterKit.Web.Models.PageTypes;

namespace OxxCommerceStarterKit.Web.Models.ViewModels
{
	public class ChangePasswordViewModel : PageViewModel<ChangePasswordPage>
	{
		public ChangePasswordForm ChangePasswordForm { get; set; }
		public string Firstname { get; set; }

		public ChangePasswordViewModel(ChangePasswordPage currentPage)
			: base(currentPage)
		{
			ChangePasswordForm = new ChangePasswordForm();

			var contactRepository = new ContactRepository();
			var contactInformation = contactRepository.Get();
			Firstname = contactInformation.FirstName;

		}
	}
}
