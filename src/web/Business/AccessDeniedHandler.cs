/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Web;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using OxxCommerceStarterKit.Web.Models.PageTypes;

namespace OxxCommerceStarterKit.Web.Business
{
    /// <summary>
    /// Default implementation of <see cref="T:EPiServer.AccessDeniedDelegate"/>
    /// </summary>
    public static class AccessDeniedHandler
    {
        /// <summary>
        /// Creates the access denied delegate respect to site authentication mode.
        ///             If the site configured with Forms then creates FormLogonAccessDenied otherwise BrowserLogonAccessDenied
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns/>
        public static AccessDeniedDelegate CreateAccessDeniedDelegate(AuthorizationContext filterContext)
        {
            return new AccessDeniedDelegate(d => AccessDeniedHandler.AccessDenied(filterContext));
        }

        public static void AccessDenied(object sender)
        {
            if (FormsSettings.IsFormsAuthentication)
                AccessDeniedHandler.FormLogonAccessDenied((AuthorizationContext)sender);
            else
                AccessDeniedHandler.BrowserLogonAccessDenied();
        }


        /// Sends an access denied message when using bowser-based authentication (Basic Auth or NTLM).
        /// 
        /// </summary>
        private static void BrowserLogonAccessDenied()
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Status = "401 Unauthorized";
            HttpContext.Current.Response.Write("Access denied.");
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// Handles access denied for forms authentication scenarios by redirecting to the logon form.
        /// 
        /// </summary>
        /// <param name="filter"></param>
        private static void FormLogonAccessDenied(AuthorizationContext filter)
        {
            IContentRepository contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            UrlResolver urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();

            string str = HttpContext.Current.Request.Url.PathAndQuery;
            var loginUrl =
                urlResolver.GetUrl(SiteConfiguration.Current().Settings.LoginPage);

            filter.Result = new RedirectResult(loginUrl + "?ReturnUrl=" + HttpContext.Current.Server.UrlEncode(str));
        }
    }
}
