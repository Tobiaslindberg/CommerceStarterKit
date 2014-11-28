/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;
using OxxCommerceStarterKit.Core.Attributes;

namespace OxxCommerceStarterKit.Web.Models.Blocks
{
	/// <summary>
	/// Based on the YouTube block from Jeff Wallace:
	/// http://world.episerver.com/Blogs/Jeff-Wallace/Dates/2013/3/YouTube-Block/
	/// </summary>
	[ContentType(
		DisplayName = "Vimeo Block",
		GUID = "a8172c33-e087-4e68-980e-a79b0e093675",
		Description = "Display Vimeo video")]
	[SiteImageUrl]
	public class VimeoBlock : SiteBlockData
	{

		/// <summary>
		/// Gets link of YouTube video
		/// </summary>        
		[Display(
			Name = "Vimeo Link",
			Description = "URL link to Vimeo video",
			GroupName = SystemTabNames.Content,
			Order = 1)]
		[Required]
		[RegularExpression(@"^https?:\/\/(?:www\.)?vimeo.com\/?(?=\w+)(?:\S+)?$",
			ErrorMessage = "The Url must be a valid Vimeo video link")]
		[Searchable(false)]
		public virtual String VimeoVideoLink { get; set; }

		[Display(
			Name = "CoverImage",
			GroupName = SystemTabNames.Content,
			Order = 10)]
		[UIHint(UIHint.Image)]
		[Searchable(false)]
		public virtual ContentReference CoverImage { get; set; }

		[ScaffoldColumn(false)]
		public virtual VimeoUrl VimeoVideo
		{
			get
			{
				string videoId = VimeoVideoLink;

				if (!string.IsNullOrEmpty(videoId))
				{
					if (_vimeoUrl == null)
					{
						_vimeoUrl = new VimeoUrl(videoId);
					}
					return _vimeoUrl;
				}
				return null;
			}
		}
		private VimeoUrl _vimeoUrl;

		/// <summary>
		/// Heading for the video
		/// </summary>        
		[Display(
			Name = "Heading",
			Description = "Heading for the video",
			GroupName = SystemTabNames.Content,
			Order = 20)]
		[CultureSpecific]
		public virtual String Heading { get; set; }

		/// <summary>
		/// Descriptive text for the video
		/// </summary>        
		[Display(
			Name = "Video Text",
			Description = "Descriptive text for the video",
			GroupName = SystemTabNames.Content,
			Order = 30)]
		[CultureSpecific]
		public virtual XhtmlString VideoText { get; set; }

		[ScaffoldColumn(false)]
		public bool HasVideo
		{
			get { return !string.IsNullOrEmpty(VimeoVideoLink); }
		}

		[ScaffoldColumn(false)]
		public bool HasCoverImage
		{
			get { return CoverImage != null; }
		}

	}

	public class VimeoUrl
	{
		private const string UrlRegex = @"vimeo\.com/(\d+)";

		public string Id { get; set; }

		public VimeoUrl(string videoUrl)
		{
			GetVideoId(videoUrl);
		}

		private void GetVideoId(string videoUrl)
		{
			var regex = new Regex(UrlRegex);

			var match = regex.Match(videoUrl);

			if (match.Success)
			{
				Id = match.Groups[1].Value;
			}
		}

		public string GetIframeUrl(bool autoPlay)
		{
			return "//player.vimeo.com/video/" + Id + "?title=0&byline=0&portrait=0" + (autoPlay ? "&autoplay=1" : "");
		}
	}
}
