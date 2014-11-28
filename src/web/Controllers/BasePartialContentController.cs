/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Framework.Web;
using EPiServer.Web.Mvc;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Controllers
{
    [TemplateDescriptor(TemplateTypeCategory = TemplateTypeCategories.MvcPartialController, Inherited = true, AvailableWithoutTag = false)]
    public class BasePartialContentController<T> : PartialContentController<T> where T : IContentData
    {
        /// <summary>
        /// Gets the partial name of the the view to use when tags are used on content
        /// in content areas.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        protected string GetViewForDisplayOption(string tag)
        {
            string viewSize = string.Empty;
            switch (tag)
            {
                case WebGlobal.ContentAreaTags.FullWidth:
                    viewSize = "Full";
                    break;
                case WebGlobal.ContentAreaTags.TwoThirdsWidth:
                    viewSize = "TwoThird";
                    break;
                case WebGlobal.ContentAreaTags.HalfWidth:
                    viewSize = "Half";
                    break;
                case WebGlobal.ContentAreaTags.OneThirdWidth:
                    viewSize = "OneThird";
                    break;



            }
            return viewSize;
        }

        protected bool ViewExists(string name)
        {
            ViewEngineResult result = ViewEngines.Engines.FindView(ControllerContext, name, null);
            return (result.View != null);
        }

        protected virtual ActionResult GetPartialViewForTag(string baseViewName, ProductListViewModel productListViewModel)
        {
            if (ControllerContext.ParentActionViewContext.ViewData["tag"] != null)
            {
                string tag = ControllerContext.ParentActionViewContext.ViewData["tag"].ToString();
                if (string.IsNullOrEmpty(tag) == false)
                {
                    string viewSize = GetViewForDisplayOption(tag);

                    if (ViewExists(baseViewName + viewSize))
                        return PartialView(baseViewName + viewSize, productListViewModel);
                }
            }
            return null;
        }


    } 
}
