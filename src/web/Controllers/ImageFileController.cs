/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Web.Mvc;
using EPiServer.Globalization;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using OxxCommerceStarterKit.Web.Models.Blocks;
using OxxCommerceStarterKit.Web.Models.Files;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Controllers
{
    /// <summary>
    /// This controller is used when viewing an image in a content area. Mainly used in articles.
    /// </summary>
    public class ImageFileController : PartialContentController<ImageFile>
    {
        private readonly UrlResolver _urlResolver;

        public ImageFileController(UrlResolver urlResolver)
        {
            _urlResolver = urlResolver;
        }

		/// <summary>
		/// The index action for the image file. Creates the view model and renders the view.
		/// </summary>
		/// <param name="currentContent">The current image file.</param>
		public override ActionResult Index(ImageFile currentContent)
		{
			return Image(currentContent, string.Empty);
		}

        /// <summary>
        /// The index action for the image file. Creates the view model and renders the view.
        /// </summary>
        /// <param name="currentContent">The current image file.</param>
		/// <param name="extraImageUrlParameters">Text added to the url to the image</param>
        public ActionResult Image(ImageFile currentContent, string extraImageUrlParameters = null)
        {
			string extra = string.IsNullOrEmpty(extraImageUrlParameters) ? "" : extraImageUrlParameters;

			if (!string.IsNullOrEmpty(currentContent.VideoUrl))
			{
				string guessSource = currentContent.VideoUrl.ToLower();
				if (guessSource.Contains("viddler.com"))
				{
					var viddlerVideo = new ViddlerUrl(currentContent.VideoUrl);
					var viddlerModel = new VideoViewModel()
					{
						CoverImageUrl = _urlResolver.GetUrl(currentContent.ContentLink) + extra,
						HasCoverImage = true,
						IframeUrl = viddlerVideo.GetIframeUrl(true),
						IframeId = "viddler-" + viddlerVideo.Id
					};
					return PartialView("_Video", viddlerModel);
				}
				else if (guessSource.Contains("youtube"))
				{
					var youTubeVideo = new YouTubeUrl(currentContent.VideoUrl);
					var youTubeModel = new VideoViewModel()
					{
						CoverImageUrl = _urlResolver.GetUrl(currentContent.ContentLink) + extra,
						HasCoverImage = true,
						IframeUrl = youTubeVideo.GetIframeUrl(true)
					};
					return PartialView("_Video", youTubeModel);
				}
				else if (guessSource.Contains("vimeo.com"))
				{
					var vimeoVideo = new VimeoUrl(currentContent.VideoUrl);
					var vimeoModel = new VideoViewModel()
					{
						CoverImageUrl = _urlResolver.GetUrl(currentContent.ContentLink) + extra,
						HasCoverImage = true,
						IframeUrl = vimeoVideo.GetIframeUrl(true)
					};
					return PartialView("_Video", vimeoModel);
				}
			}

			var model = new ImageViewModel(currentContent, Language)
			{
				IsMobile = this.Request.Browser.IsMobileDevice,
				Tag = ControllerContext.ParentActionViewContext.ViewData["Tag"] as string
			};
            // Add additonal data to it
            model.Url += extra;

			return PartialView("~/Views/ImageFile/Index.cshtml", model);
        }

        public string Language
		{
			get
			{
			    string language = ControllerContext.RequestContext.GetLanguage();
				if (string.IsNullOrEmpty(language))
				{
					language = ContentLanguage.PreferredCulture.Name;
				}

				return language;
			}
		}
    }
}
