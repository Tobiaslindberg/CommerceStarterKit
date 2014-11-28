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
		DisplayName = "Viddler Block",
		GUID = "b02db132-1475-4f06-abe1-7671f7c62882",
		Description = "Display Viddler video")]
	[SiteImageUrl]
	public class ViddlerBlock : SiteBlockData
	{

		/// <summary>
		/// Gets link of YouTube video
		/// </summary>        
		[Display(
			Name = "Viddler Link",
			Description = "URL link to viddler video",
			GroupName = SystemTabNames.Content,
			Order = 1)]
		[Required]
		[RegularExpression(@"^https?:\/\/(?:www\.)?viddler.com\/v\/(?=\w+)(?:\S+)?$",
			ErrorMessage = "The Url must be a valid Viddler video link")]
		[Searchable(false)]
		public virtual String ViddlerVideoLink { get; set; }

		[Display(
			Name = "CoverImage",
			GroupName = SystemTabNames.Content,
			Order = 10)]
		[UIHint(UIHint.Image)]
		[Searchable(false)]
		public virtual ContentReference CoverImage { get; set; }

		[ScaffoldColumn(false)]
		public virtual ViddlerUrl ViddlerVideo
		{
			get
			{
				string videoId = ViddlerVideoLink;
				if (!string.IsNullOrEmpty(videoId))
				{
					if (_viddlerUrl == null)
					{
						_viddlerUrl = new ViddlerUrl(videoId);
					}
					return _viddlerUrl;
				}
				return null;
			}
		}
		private ViddlerUrl _viddlerUrl;

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
			get { return !string.IsNullOrEmpty(ViddlerVideoLink); }
		}

		[ScaffoldColumn(false)]
		public bool HasCoverImage
		{
			get { return CoverImage != null; }
		}
	}

	public class ViddlerUrl
	{
		private const string UrlRegex = @"viddler\.com/v/([^?/&]+)(?:[?]secret=([^&]+))?";

		public string Id { get; set; }
		public string Secret { get; set; }


		public ViddlerUrl(string videoUrl)
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
				if (match.Groups.Count == 3)
				{
					Secret = match.Groups[2].Value;
				}
			}
		}

		public string GetIframeUrl(bool autoPlay)
		{
			bool hasSecret = !string.IsNullOrEmpty(Secret);
			return "//www.viddler.com/embed/" + Id + "/?f=1&offset=0&player=full&disablebranding=0" + 
				(hasSecret ? "&secret=" + Secret + "&view_secret=" + Secret : "") + 
				(autoPlay ? "&autoplay=1" : "") + "&wmode=opaque";
		}
	}
}

