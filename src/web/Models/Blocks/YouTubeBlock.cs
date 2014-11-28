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
using OxxCommerceStarterKit.Web.Business.Rendering;

namespace OxxCommerceStarterKit.Web.Models.Blocks
{
    /// <summary>
    /// Based on the YouTube block from Jeff Wallace:
    /// http://world.episerver.com/Blogs/Jeff-Wallace/Dates/2013/3/YouTube-Block/
    /// </summary>
    [ContentType(
        DisplayName = "YouTube Block",
        GUID = "67429E0D-9365-407C-8A49-69489382BBDC",
        Description = "Display YouTube video")]
	[SiteImageUrl]
    public class YouTubeBlock : SiteBlockData, IDefaultDisplayOption
    {
        /// <summary>
        /// Gets link of YouTube video
        /// </summary>        
        [Display(
            Name = "YouTube Link",
            Description = "URL link to YouTube video",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        [Required]
        [RegularExpression(@"^https?:\/\/(?:www\.)?youtube.com\/watch\?(?=.*v=\w+)(?:\S+)?$",
            ErrorMessage = "The Url must be a valid YouTube video link")]
		[Searchable(false)]
        public virtual String YouTubeLink { get; set; }


		[Display(
			Name = "CoverImage",
			GroupName = SystemTabNames.Content,
			Order = 10)]
		[UIHint(UIHint.Image)]
		[Searchable(false)]
		public virtual ContentReference CoverImage { get; set; }

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
		public virtual YouTubeUrl YouTubeVideo
		{
			get
			{
				string videoId = YouTubeLink;
				if (!string.IsNullOrEmpty(videoId))
				{
					if (_youTubeUrl == null)
					{
						_youTubeUrl = new YouTubeUrl(videoId);
					}
					return _youTubeUrl;
				}
				return null;
			}
		}
		private YouTubeUrl _youTubeUrl;

		[ScaffoldColumn(false)]
        public bool HasVideo
        {
            get { return !string.IsNullOrEmpty(YouTubeLink); }
        }

		[ScaffoldColumn(false)]
		public bool HasCoverImage
		{
			get { return CoverImage != null; }
		}

		[ScaffoldColumn(false)]
        public string Tag
        {
            get { return WebGlobal.ContentAreaTags.HalfWidth; }
        }

    }

	public class YouTubeUrl
	{
		private const string UrlRegex = @"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)";

		public string Id { get; set; }

		public YouTubeUrl(string videoUrl)
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
			return "//www.youtube.com/embed/" + Id + "?feature=player_embedded&wmode=opaque" + (autoPlay ? "&autoplay=1" : "");
		}
	}
}
