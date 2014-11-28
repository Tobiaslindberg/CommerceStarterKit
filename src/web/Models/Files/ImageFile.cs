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

        //public virtual Blob width370 { get; set; }
        //public virtual Blob width560 { get; set; }
        //public virtual Blob width750 { get; set; }
        //public virtual Blob width1130 { get; set; }
        //public virtual Blob box370 { get; set; }
        //public virtual Blob box560 { get; set; }
        //public virtual Blob box750 { get; set; }
        //public virtual Blob box1130 { get; set; }
        //public virtual Blob width580 { get; set; }
        //public virtual Blob width379 { get; set; }
        //public virtual Blob width279 { get; set; }
        //public virtual Blob width179 { get; set; }
        //public virtual Blob LargeThumbnail { get; set; }
        //public virtual Blob width110 { get; set; }
        //public virtual Blob width320 { get; set; }
        //public virtual Blob ListImage { get; set; }
        //public virtual Blob RelatedProduct { get; set; }
        //public virtual Blob SimilarProduct { get; set; }
        //public virtual Blob SliderImage { get; set; }

    }

}
