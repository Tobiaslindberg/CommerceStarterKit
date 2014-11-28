/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

namespace OxxCommerceStarterKit.Web.Services.Email.Models
{
	public class ResetPassword : EmailBase
	{
		public string Token { get; set; }
		public string Body { get; set; }
		public string ResetUrl { get; set; }
	}
}
