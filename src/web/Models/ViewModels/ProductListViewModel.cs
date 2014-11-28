/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.Core;

namespace OxxCommerceStarterKit.Web.Models.ViewModels
{
    public class ProductListViewModel
    {
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public string NewItemText { get; set; }
        public XhtmlString Description { get; set; }
        public ContentReference ContentLink { get; set; }
        public string PriceString { get; set; }
        public decimal PriceAmount { get; set; }
        public string BrandName { get; set; }
        public Dictionary<string, ContentReference> Images { get; set; }
        public Dictionary<string, string> Variants { get; set; }
        public string Country { get; set; }

        public string ProductUrl { get; set; }

        public string ImageUrl { get; set; }
    }
}
