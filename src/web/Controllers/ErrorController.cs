/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BVNetwork.Bvn.FileNotFound.Logging;
using BVNetwork.Bvn.FileNotFound.Upgrade;
using BVNetwork.FileNotFound.Configuration;
using BVNetwork.FileNotFound.Redirects;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using OxxCommerceStarterKit.Web.Models.PageTypes;
using OxxCommerceStarterKit.Web.Models.ViewModels;
using OxxCommerceStarterKit.Web.Services.Email;

namespace OxxCommerceStarterKit.Web.Controllers
{
    public class ErrorController : PageControllerBase<PageData>
    {


        // GET: Error404
        public ActionResult Error404()
        {
			ErrorPageViewModel model = GetViewModel();

			model.Referer = HttpContext.Request.UrlReferrer;
			model.NotFoundUrl = GetUrlNotFound(HttpContext);

			HandleOnLoad(HttpContext, model.NotFoundUrl, model.Referer);

			return View("Error404", model);
        }

		private static ErrorPageViewModel GetViewModel()
		{
			ErrorPageViewModel model;
			try
			{
				var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();				

				model = CreateErrorPageViewModel(contentLoader.Get<HomePage>(ContentReference.StartPage));
				model.HasDatabase = true;
			}
			catch (Exception)
			{
				model = new ErrorPageViewModel();
			}
			return model;
		}

        private static ErrorPageViewModel CreateErrorPageViewModel(HomePage pageData)
        {
            var model = new ErrorPageViewModel();
            model.CurrentPage = pageData;            
            return (ErrorPageViewModel)model;
        }

		public ActionResult Error500(Exception exception = null)
		{
			ErrorPageViewModel model = GetViewModel();

			if (exception != null && !(exception is OperationCanceledException || exception is TaskCanceledException) && (
				HttpContext == null || (HttpContext != null && !HttpContext.Request.Url.ToString().EndsWith("/find_v2/"))))
			{
				_log.Error(exception);

				NotifyDeveloper("", exception);
			}



			return View("Error500", model);
		}


		public void NotifyDeveloper(string error, Exception exception)
		{
			try
			{
				string message = "", server = "", domain = "";
				if (HttpContext != null)
				{
					domain = HttpContext.Request.Url.Host;
					server = HttpContext.Server.MachineName;
					message = ""
						+ "Server: " + server + "<br />"
						+ "Domain: " + domain + "<br />"
						+ "Url: " + HttpContext.Request.Url + "<br />";
				}
				else
				{
					server = Environment.MachineName;
					domain = "-";
				}
				message += " Exception: <b>" + exception.Message + "</b><br />"
					+ "Message: <b>" + error + "</b><br />"
					+ "Stack Trace: " + exception.StackTrace + "<br />";

				string to = "eros@oxx.no";
				string subject = server + " - " + domain + " - Notify developer";

				if (server == "PC-EMMU-1") to = "emmu@oxx.no";
				if (server == "PC-EROS-1" || server == "PC-EROS-2") to = "eros@oxx.no";

				var emailService = ServiceLocator.Current.GetInstance<IEmailService>();
				emailService.SendWelcomeEmail(to, subject, message);
			}
			catch (Exception ex)
			{
				_log.Error("Unable to notify deloper about 500 error", ex);
			}
		}

		/// <summary>
		/// Copied from BVNetwork.FileNotFound.NotFoundPageUtil.HandleOnLoad(Page page, Uri urlNotFound, string referer)
		/// </summary>
		/// <param name="context"></param>
		/// <param name="urlNotFound"></param>
		/// <param name="referer"></param>
		protected void HandleOnLoad(HttpContextBase context, Uri urlNotFound, Uri referer)
		{
			if (_log.IsDebugEnabled)
			{
				_log.DebugFormat("Trying to handle 404 for \"{0}\" (Referrer: \"{1}\")", urlNotFound, referer);
			}
			CustomRedirectHandler current = CustomRedirectHandler.Current;
			CustomRedirect customRedirect = current.CustomRedirects.Find(HttpUtility.HtmlEncode(urlNotFound.AbsoluteUri));
			string str = HttpUtility.HtmlEncode(urlNotFound.PathAndQuery);
			if (customRedirect == null)
			{
				customRedirect = current.CustomRedirects.Find(str);
			}
			if (customRedirect != null)
			{
				if (customRedirect.State.Equals(0) && string.Compare(customRedirect.NewUrl, str, StringComparison.InvariantCultureIgnoreCase) != 0)
				{
					_log.Info(string.Format("404 Custom Redirect: To: '{0}' (from: '{1}')", customRedirect.NewUrl, str));
					context.Response.Clear();
					context.Response.StatusCode = 301;
					context.Response.StatusDescription = "Moved Permanently";
					context.Response.RedirectLocation = customRedirect.NewUrl;
					context.Response.End();
					return;
				}
			}
			else if (Configuration.Logging == LoggerMode.On && Upgrader.Valid)
			{
				Logger.LogRequest(str, referer == null ? string.Empty : referer.ToString());
			}
			context.Response.TrySkipIisCustomErrors = true;
			context.Response.StatusCode = 404;
			context.Response.Status = "404 File not found";
		}


		public static Uri GetUrlNotFound(HttpContextBase context)
		{
			Uri uri = null;
			string item = context.Request.ServerVariables["QUERY_STRING"];
			if (item != null && item.StartsWith("404;"))
			{
				char[] chrArray = new char[] { ';' };
				uri = new Uri(item.Split(chrArray)[1]);
			}
			if (uri == null && item.StartsWith("aspxerrorpath="))
			{
				string[] strArrays = item.Split(new char[] { '=' });
				uri = new Uri(string.Concat(context.Request.Url.GetLeftPart(UriPartial.Authority), HttpUtility.UrlDecode(strArrays[1])));
			}
			return uri;
		}
    }
}
