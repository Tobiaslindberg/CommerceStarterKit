/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using EPiServer;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Web;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using OxxCommerceStarterKit.Web.Models.Blocks;

namespace OxxCommerceStarterKit.Web.Business.Rendering
{
    [ServiceConfiguration(typeof(IViewTemplateModelRegistrator))]
    public class TemplateCoordinator : IViewTemplateModelRegistrator
    {
        public const string BlockFolder = "~/Views/Shared/Blocks/";
        public const string PagePartialsFolder = "~/Views/Shared/PagePartials/";

		public static void OnTemplateResolved(object sender, TemplateResolverEventArgs args)
		{
			//Disable DefaultPageController for page types that shouldn't have any renderer as pages
			//if (args.ItemToRender is IContainerPage && args.SelectedTemplate != null && args.SelectedTemplate.TemplateType == typeof(DefaultPageController))
			//{
			//	args.SelectedTemplate = null;
			//}
		}

        /// <summary>
        /// Registers renderers/templates which are not automatically discovered, 
        /// i.e. partial views whose names does not match a content type's name.
        /// </summary>
        /// <remarks>
        /// Using only partial views instead of controllers for blocks and page partials
        /// has performance benefits as they will only require calls to RenderPartial instead of
        /// RenderAction for controllers.
        /// Registering partial views as templates this way also enables specifying tags and 
        /// that a template supports all types inheriting from the content type/model type.
        /// </remarks>
		public void Register(TemplateModelCollection viewTemplateModelRegistrator)
		{

            // We want a special renderer for the HtmlBlock when rendered in full view
            viewTemplateModelRegistrator.Add(typeof (HtmlBlock), new TemplateModel()
            {
                Name = "Html Block rendered in Full view",
                Inherit = true,
                Tags = new[] { WebGlobal.ContentAreaTags.FullWidth },
                Path = BlockPath("HtmlBlock.Full.cshtml"),
                TemplateTypeCategory = TemplateTypeCategories.MvcPartialView,
                AvailableWithoutTag = false,
                Default = false
            });

	
			//viewTemplateModelRegistrator.Add(typeof(ProductTeaserBlock), new TemplateModel
			//{
			//	Name = "ProductTeaserEditBlock",
			//	Inherited = true,
			//	Tags = new [] { RenderingTags.Edit, RenderingTags.Preview },
			//	Path = BlockPath("ProductTeaserEditBlock.cshtml"),
			//	TemplateTypeCategory = TemplateTypeCategories.MvcPartial
			//});

        }

        public static string BlockPath(string fileName)
        {
            return string.Format("{0}{1}", BlockFolder, fileName);
        }

		public static string PagePartialPath(string fileName)
        {
            return string.Format("{0}{1}", PagePartialsFolder, fileName);
        }
    }
}
