/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using EPiServer.Core;

namespace OxxCommerceStarterKit.Web.Models.ViewModels
{
    public class ContentFolderViewModel
    {
        public ContentFolderViewModel()
        {
            Items = new List<ContentFolderItemViewModel>();
        }

        public List<ContentFolderItemViewModel> Items { get; set; }
    }

    public class ContentFolderItemViewModel
    {
        public MediaData Content { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }

    }
}
