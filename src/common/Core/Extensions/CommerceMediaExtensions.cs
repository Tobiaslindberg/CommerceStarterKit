/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.Core;
using EPiServer.Web;

namespace OxxCommerceStarterKit.Core.Extensions
{
    public static class CommerceMediaExtensions
    {
        //public static Guid AssetId(this CommerceMedia media)
        //{
        //    return new Guid(media.AssetKey);
        //}

        public static ContentReference AssetContentLink(this CommerceMedia media, IPermanentLinkMapper permanentLinkMapper)
        {
            return media.AssetLink;
            // return PermanentLinkUtility.FindContentReference(media.AssetLink, permanentLinkMapper);
        }
    }
}
