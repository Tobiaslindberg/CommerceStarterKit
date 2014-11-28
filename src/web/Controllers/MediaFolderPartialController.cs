/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Globalization;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Controllers
{
    [TemplateDescriptor(Inherited = true)]
    public class MediaFolderPartialController : PartialContentController<ContentFolder>
    {
        private readonly UrlResolver _urlResolver;
        private readonly IContentRepository _contentRepository;

        public MediaFolderPartialController(UrlResolver urlResolver, IContentRepository contentRepository)
        {
            _urlResolver = urlResolver;
            _contentRepository = contentRepository;
        }

        public string Language
        {
            get
            {
                string language = null;
                if (ControllerContext.RouteData.Values["language"] != null)
                {
                    language = ControllerContext.RouteData.Values["language"].ToString();
                }

                if (string.IsNullOrEmpty(language))
                {
                    language = ContentLanguage.PreferredCulture.Name;
                }

                return language;
            }
        }


        /// <summary>
		/// The index action for the image file. Creates the view model and renders the view.
		/// </summary>
		/// <param name="currentContent">The current image file.</param>
        public override ActionResult Index(ContentFolder currentContent)
        {            
            ContentFolderViewModel viewModel = new ContentFolderViewModel();
            string language = Language;
            var children = _contentRepository.GetChildren<MediaData>(currentContent.ContentLink);
            foreach (MediaData mediaData in children)
            {
                ContentFolderItemViewModel itemModel = new ContentFolderItemViewModel();
                itemModel.Content = mediaData;
                itemModel.Url = _urlResolver.GetUrl(mediaData.ContentLink);
                itemModel.ThumbnailUrl = _urlResolver.GetUrl(
                    mediaData.ContentLink,
                    language,
                    new VirtualPathArguments() {ContextMode = ContextMode.Default}) +
                                         "?preset=thumbnail";
                viewModel.Items.Add(itemModel);
            }

            return PartialView("Index", viewModel);
		}

    }
}
