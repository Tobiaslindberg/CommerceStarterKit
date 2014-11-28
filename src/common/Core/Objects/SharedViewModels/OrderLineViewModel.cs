/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using OxxCommerceStarterKit.Core.Extensions;

namespace OxxCommerceStarterKit.Core.Objects.SharedViewModels
{
	public class OrderLineViewModel
	{
        public string Code { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Color { get; set; }
		public string Size { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
		public decimal Discount { get; set; }
		public string ArticleNumber { get; set; }

		public OrderLineViewModel() { }
		public OrderLineViewModel(Mediachase.Commerce.Orders.LineItem item)
		{
		    Code = item.CatalogEntryId;
			Name = item.DisplayName;
			Description = item.GetStringValue(Constants.Metadata.LineItem.Description);
			Price = item.ExtendedPrice;
			Discount = item.LineItemDiscountAmount + item.OrderLevelDiscountAmount;
			Quantity = (int)item.Quantity;
			Size = item.GetStringValue(Constants.Metadata.LineItem.Size);
			Color = item.GetStringValue(Constants.Metadata.LineItem.Color);
			ArticleNumber = item.GetStringValue(Constants.Metadata.LineItem.ArticleNumber);
		}
	}
}
