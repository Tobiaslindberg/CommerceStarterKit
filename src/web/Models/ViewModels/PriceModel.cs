/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Linq;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Website.Helpers;

namespace OxxCommerceStarterKit.Web.Models.ViewModels
{
	public class PriceModel
	{
		public Price Price { get; set; }
        public string DiscountDisplayPrice { get; set; }
        public string CustomerClubDisplayPrice { get; set; }

		public PriceModel()
		{

		}

		public PriceModel(Price price)
			: this()
		{
			if (price != null)
			{
				Price = price;
			}
			else
			{
				Price = default(Price);
			}
		}

	}
}
