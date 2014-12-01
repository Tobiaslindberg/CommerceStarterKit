/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.ComponentModel.DataAnnotations;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;
using EPiServer.Framework.DataAnnotations;
using OxxCommerceStarterKit.Core;

namespace OxxCommerceStarterKit.Web.Models.Files
{
    [ContentType(GUID = "EE3BD195-7CB0-4756-AB5F-E5E223CD9820")]
    [MediaDescriptor(ExtensionString = "jpg,jpeg,jpe,ico,gif,bmp,png")]
    public class ImageFile : ImageData
    {
        public static class ImageWidths
        {
            public const string NARROW = "?preset=imagenarrow";
            public const string HALF = "?preset=imagehalf";
            public const string WIDE = "?preset=imagewide";
            public const string FULL = "?preset=imagefull";
        }

        public static class BoxSizes
        {
            public const string NARROW = "?preset=boxnarrow";
            public const string HALF = "?preset=boxhalf";
            public const string WIDE = "?preset=boxwide";
            public const string FULL = "?preset=boxfull";
        }

        public static class NewsletterWidths
        {
            public const string NARROW = "?preset=newsletternarrow";
            public const string HALF = "?preset=newsletterhalf";
            public const string WIDE = "?preset=newsletterwide";
            public const string FULL = "?preset=newsletterfull";
        }

        public static class Thumbnails
        {
            public const string Normal = "?preset=thumbnail";
            public const string Large = "?preset=largethumbnail";
        }

        public virtual String Description { get; set; }
        public virtual Url Link { get; set; }
        public virtual String Copyright { get; set; }
        public virtual String VideoUrl { get; set; }


        [Display(
            Name = "HotSpot configuration",
            Description = "HotSpot editor",
            GroupName = "HotSpots"
            )]
        [Searchable(false)]
        [UIHint(Constants.UIHint.HotSpotsEditor)]
        public virtual String HotSpotSettings { get; set; }

        /// <summary>
        /// Gets or sets the large thumbnail used by Commerce UI
        /// </summary>
        /// <remarks>
        /// You can also inherit from CommerceMedia
        /// </remarks>
        [Editable(false)]
        [ImageDescriptor(Width = 256, Height = 256)]
        public virtual Blob LargeThumbnail { get; set; }
    }

}
