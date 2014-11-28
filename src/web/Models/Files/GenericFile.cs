/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace OxxCommerceStarterKit.Web.Models.Files
{
    [ContentType(GUID = "427BB926-1038-4B42-A9F9-790D2D504D48")]
    public class GenericFile : MediaData
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public virtual String Description { get; set; }
    }
}
