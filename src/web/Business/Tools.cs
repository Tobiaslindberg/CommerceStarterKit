/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Configuration;

namespace OxxCommerceStarterKit.Web.Business
{
	public static class Tools
	{
		public static string GetAppSetting(string key)
		{
			if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]))
			{
				return ConfigurationManager.AppSettings[key];
			}
			return string.Empty;
		}

		public static bool IsAppSettingTrue(string key)
		{
			var setting = GetAppSetting(key);
			if (setting != "" && setting.ToLower() != "false" && setting.ToLower() != "no") return true;
			return false;
		}
	}
}
