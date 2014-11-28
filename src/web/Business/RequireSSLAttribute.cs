/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Web;
using System.Web.Mvc;

namespace OxxCommerceStarterKit.Web.Business
{
	public class RequireSSLAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			HttpRequestBase req = filterContext.HttpContext.Request;
			HttpResponseBase res = filterContext.HttpContext.Response;

			//Check if we're secure or not and if we're on the local box
			if (!req.IsSecureConnection && !req.IsLocal && Tools.IsAppSettingTrue("RedirectToHttps"))
			{
				string url = req.Url.ToString().ToLower().Replace("http:", "https:");
				res.Redirect(url);
			}
			base.OnActionExecuting(filterContext);
		}
	}
}
