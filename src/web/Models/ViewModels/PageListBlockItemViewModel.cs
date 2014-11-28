/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using OxxCommerceStarterKit.Web.Extensions;
using OxxCommerceStarterKit.Web.Models.PageTypes;

namespace OxxCommerceStarterKit.Web.Models.ViewModels
{
	public class PageListBlockItemViewModel
	{
		private string _imageUrl;
		public string ImageUrl
		{
			get
			{
				if (!string.IsNullOrEmpty(_imageUrl) && !string.IsNullOrEmpty(ImageExtraUrlParameters))
				{
					if (_imageUrl.Contains("?"))
					{
						var split = _imageUrl.Split('?');
						return split[0] + ImageExtraUrlParameters + "?" + split[1];
					}
					else
					{
						return _imageUrl + ImageExtraUrlParameters;
					}
				}

				return _imageUrl;
			}
		}
		public string Title { get; set; }
		public string Text { get; set; }
		public ContentReference ContentLink { get; set; }

		public string ImageExtraUrlParameters { get; set; }

		public PageListBlockItemViewModel(PageData page)
		{
			Title = page.Name;
			ContentLink = page.ContentLink;
			Text = GetPageListBlockItemText(page);
			_imageUrl = GetPageListBlockItemImageUrl(page);
		}

		private string GetPageListBlockItemText(PageData page)
		{
			if (page is BlogPage)
			{
				var blog = (BlogPage)page;
				return blog.ListViewText;
			}
			else if (page is ArticlePage)
			{
				var article = (ArticlePage)page;
				if (!string.IsNullOrEmpty(article.ListViewText))
				{
					return article.ListViewText;
				}
				else
				{
					if (article.Intro != null)
					{
						return article.Intro.ToHtmlString().StripHtml().StripPreviewText(255);
					}
				}
			}
			return string.Empty;
		}
		private string GetPageListBlockItemImageUrl(PageData page)
		{
			if (page is BlogPage)
			{
				var blog = (BlogPage)page;
				if (blog.ListViewImage != null)
				{
					return ServiceLocator.Current.GetInstance<UrlResolver>().GetUrl(blog.ListViewImage.ToString());
				}
			}
			else if (page is ArticlePage)
			{
				var article = (ArticlePage)page;
				if (article.ListViewImage != null)
				{
					return ServiceLocator.Current.GetInstance<UrlResolver>().GetUrl(article.ListViewImage.ToString());
				}
			}
			return "";
		}
	}
}
