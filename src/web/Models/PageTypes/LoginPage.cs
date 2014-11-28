/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using OxxCommerceStarterKit.Core.Attributes;

namespace OxxCommerceStarterKit.Web.Models.PageTypes
{
	[ContentType(GUID = "ECC455C7-FD07-4480-970E-3FB8D0F85BDE",
		DisplayName = "Login Page",
		Description = "Login Page",
		GroupName = "Commerce System Pages",
		AvailableInEditMode = false,
		Order = 100)]
	[SiteImageUrl]
	public class LoginPage : CommerceSampleModulePage
	{
		[Display(Name = "Login page text", Order = 10)]
		public virtual XhtmlString LoginContent { get; set; }

		[Display(Name = "Forgot password page text", Order = 20)]
		public virtual XhtmlString ResetPasswordContent { get; set; }

		[Display(Name = "Forgot password email sent text", Order = 30)]
		public virtual XhtmlString SentPasswordTokenContent { get; set; }

		[Display(Name = "Reset password page text", Order = 40)]
		public virtual XhtmlString RequestPasswordContent { get; set; }

		[Display(Name = "Forgot password email subject", Order = 50)]
		public virtual string RequestPasswordSubject { get; set; }

		[Display(Name = "Forgot password email text", Order = 60)]
		public virtual XhtmlString RequestPasswordExisting { get; set; }

		[Display(Name = "Register user page", Order = 70)]
		public virtual ContentReference RegisterPage { get; set; }


		[Display(Name = "Page after login", Order = 80)]
		public virtual ContentReference LoggedInPage { get; set; }
	}
}
