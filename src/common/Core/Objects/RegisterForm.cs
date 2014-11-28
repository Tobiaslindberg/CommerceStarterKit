/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OxxCommerceStarterKit.Core.Attributes;

namespace OxxCommerceStarterKit.Core.Objects
{
	public class RegisterForm
	{
		[LocalizedDisplayName("/common/account/username")]
		[DataType(DataType.Text)]
		[Required]
		public string UserName { get; set; }

		[LocalizedDisplayName("/common/account/password")]
		[DataType(DataType.Password)]
		[Required]
		public string Password { get; set; }

		[LocalizedDisplayName("/common/account/password")]
		[DataType(DataType.Password)]
		[Required]
		public string PasswordConfirm { get; set; }

		// used by reset password form
		public bool PasswordChanged { get; set; }
		public string Token { get; set; }


		public string ValidationMessage { get; set; }


		[LocalizedDisplayName("/common/accountpages/phone_label")]
		[DataType(DataType.Text)]
		public string Phone { get; set; }

		public Address Address { get; set; }

		public bool MemberClub { get; set; }

		public Dictionary<string, string> AvailableCategories { get; set; }
		public int[] SelectedCategories { get; set; }
	}
}
