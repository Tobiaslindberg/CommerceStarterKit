/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

namespace OxxCommerceStarterKit.Web.Services.Email.Models
{
	public interface INotificationSettings
	{
		string From { get; set; }
		string Header { get; set; }
		string Footer { get; set; }
	}
}
