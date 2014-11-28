/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EPiServer;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Mvc.Html;
using EPiServer.Web.Routing;

namespace OxxCommerceStarterKit.Web.Helpers
{
    public static class UrlHelpers
    {
        /// <summary>
        /// Replace "_" with "-" to support HTML-5 data-* attributes
        /// </summary>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static RouteValueDictionary AnonymousObjectToHtmlAttributes(object htmlAttributes)
        {
            RouteValueDictionary result = new RouteValueDictionary();
            if (htmlAttributes != null)
            {
                foreach (System.ComponentModel.PropertyDescriptor property in System.ComponentModel.TypeDescriptor.GetProperties(htmlAttributes))
                {
                    result.Add(property.Name.Replace('_', '-'), property.GetValue(htmlAttributes));
                }
            }
            return result;
        }

		/// <summary>
		/// Get a page reference from the url
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static int GetReferenceFromUrl(Url url)
		{
			if (url != null)
			{
				var reference = PermanentLinkUtility.GetContentReference(new UrlBuilder(url.ToString()));
				if (reference != null && reference.ID > 0)
				{
					return reference.ID;
				}
			}
			return -1;
		}

        /// <summary>
        /// Return url 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        public static Uri GetFullUrl(this HttpRequestBase request, string relativeUrl)
        {
            string hostHeader = request.Headers["host"];
            return new Uri(string.Format("{0}://{1}{2}",
               request.Url == null ? "http" : request.Url.Scheme,
               hostHeader,
               relativeUrl));
        }

        public static RouteValueDictionary GetPageRoute(this RequestContext requestContext, ContentReference pageLink)
        {
            var values = new RouteValueDictionary();
            values[RoutingConstants.NodeKey] = pageLink;
            values[RoutingConstants.LanguageKey] = ContentLanguage.PreferredCulture.Name;
            return values;
        }

		public static string ContentUrl(this UrlHelper urlHelper, EPiServer.SpecializedProperties.LinkItem item)
		{
			var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
            IContent content = urlResolver.Route(new UrlBuilder(item.Href));
            if (content == null)
            {
                return item.Href.Replace("~/", "/");
            }
			return urlHelper.ContentUrl(content.ContentLink);
		}


		public static string BaseUrl(this UrlHelper url)
		{
			return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority).TrimEnd('/');
		}
    }
}
