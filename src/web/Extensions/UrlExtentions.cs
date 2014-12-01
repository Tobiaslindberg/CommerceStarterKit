using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.Core;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace OxxCommerceStarterKit.Web.Extensions
{
    public static class UrlExtentions
    {
        public static string GetDefaultModeUrl(this UrlResolver urlResolver, ContentReference contentReference)
        {
            return urlResolver.GetUrl(contentReference, null, new VirtualPathArguments() { ContextMode = ContextMode.Default });
        }
    }
}