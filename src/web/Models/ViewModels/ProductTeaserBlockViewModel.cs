/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using OxxCommerceStarterKit.Web.Models.Blocks;
using OxxCommerceStarterKit.Web.Models.Files;
using OxxCommerceStarterKit.Web.Models.PageTypes;

namespace OxxCommerceStarterKit.Web.Models.ViewModels
{
	public class ProductTeaserBlockViewModel : PageViewModel<SitePage>
	{
		public ProductTeaserBlockViewModel(ProductTeaserBlock model)
		{
		    IContentLoader contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            UrlResolver urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
			BlockContent = model;
			CustomColor = CalculateBackgroundColor(model);

		    if (ContentReference.IsNullOrEmpty(model.Image) == false)
		    {
                ImageContent = contentLoader.Get<MediaData>(model.Image);
                // Use Default view, not the one for edit mode which
                // we can't easliy scale
                ImageUrl = urlResolver.GetUrl(model.Image, this.Language, new VirtualPathArguments(){ContextMode = ContextMode.Default});
		    }
		}

		private string CalculateBackgroundColor(ProductTeaserBlock model)
		{
			string customColor = "";
			if (!string.IsNullOrEmpty(model.ImageTextBackgroundColor))
			{
				if (model.ImageTextBackgroundTransparency == 0)
				{
					customColor = string.Format("background-color:{0};", model.ImageTextBackgroundColor);
				}
				else
				{
					string sRed = null, sGreen = null, sBlue = null;
					int red = 0, green = 0, blue = 0;
					if (model.ImageTextBackgroundColor.Length == 4)
					{
						sRed = model.ImageTextBackgroundColor.Substring(1, 1);
						sGreen = model.ImageTextBackgroundColor.Substring(2, 1);
						sBlue = model.ImageTextBackgroundColor.Substring(3, 1);
						sRed += sRed;
						sGreen += sGreen;
						sBlue += sBlue;
					}
					else if (model.ImageTextBackgroundColor.Length == 7)
					{
						sRed = model.ImageTextBackgroundColor.Substring(1, 2);
						sGreen = model.ImageTextBackgroundColor.Substring(3, 2);
						sBlue = model.ImageTextBackgroundColor.Substring(5, 2);
					}
					if (sRed != null && sGreen != null && sBlue != null)
					{
						red = Convert.ToInt32(sRed, 16);
						green = Convert.ToInt32(sGreen, 16);
						blue = Convert.ToInt32(sBlue, 16);
						string alpha = (((double)model.ImageTextBackgroundTransparency) / 100.0).ToString().Replace(",", ".");
						customColor = string.Format("background-color:rgba({0},{1},{2},{3});", red, green, blue, alpha);
					}
				}
				if (!string.IsNullOrEmpty(model.ImageTextColor))
				{
					customColor = string.Format("{0}color:{1};", customColor, model.ImageTextColor);
				}
			}
			return customColor;
		}

		public ProductTeaserBlock BlockContent { get; set; }
		public string CustomColor { get; set; }
		public string Tag { get; set; }
        public string ImageUrl { get; set; }
        public MediaData ImageContent { get; set; }

	    public bool HasImage
	    {
	        get { return ImageUrl != null; }
	    }

	}
}
