/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

namespace OxxCommerceStarterKit.Web.Models.ViewModels
{
	public class SocialBarViewModel
	{
		public SocialBarViewModel(string text)
		{
			Text = text;
		}

		public string Text { get; set; }
		public string Url { get; set; }
	}
}
