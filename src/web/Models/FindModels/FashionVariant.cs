/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using EPiServer.Find;
using OxxCommerceStarterKit.Core.Models;

namespace OxxCommerceStarterKit.Web.Models.FindModels
{
    public class FashionVariant
    {
        [Id]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
		public string ColorCode { get; set; }
        public string Size { get; set; }
        public List<PriceAndMarket> Prices { get; set; }
		public string Code { get; set; }
		public decimal Stock { get; set; }
    }
}
