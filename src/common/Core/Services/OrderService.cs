/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using Mediachase.Commerce.Orders;
using OxxCommerceStarterKit.Core.Extensions;
using OxxCommerceStarterKit.Core.Repositories;
using LineItem = OxxCommerceStarterKit.Core.Objects.LineItem;

namespace OxxCommerceStarterKit.Core.Services
{
    public class OrderService : IOrderService
    {
        private PurchaseOrder _purchaseOrder;

        public OrderService(string trackingNumber)
        {
            OrderRepository orderRepository = new OrderRepository();

            _purchaseOrder = orderRepository.GetOrderByTrackingNumber(trackingNumber);
        }

        public OrderService(int orderGroupId)
        {
            OrderRepository orderRepository = new OrderRepository();

            _purchaseOrder = orderRepository.GetOrderById(orderGroupId);
        }

        public List<LineItem> GetItems()
        {            
            return MapCartItems(_purchaseOrder.OrderForms[0].LineItems.ToArray(), "no");
        }

        private List<LineItem> MapCartItems(IEnumerable<Mediachase.Commerce.Orders.LineItem> lineItems, string language)
        {

            List<LineItem> items = new List<LineItem>();


            if (lineItems != null)
                foreach (Mediachase.Commerce.Orders.LineItem lineItem in lineItems)
                {
                    items.Add(new LineItem
                    {
                        Code = lineItem.CatalogEntryId,
                        Name = lineItem.GetStringValue(Constants.Metadata.LineItem.DisplayName),
                        ArticleNumber = lineItem.GetStringValue(Constants.Metadata.LineItem.ArticleNumber),
                        ImageUrl = lineItem.GetString(Constants.Metadata.LineItem.ImageUrl),
                        Color = lineItem.GetStringValue(Constants.Metadata.LineItem.Color),
                        ColorImageUrl = lineItem.GetStringValue(Constants.Metadata.LineItem.ColorImageUrl),
                        Description = lineItem.GetStringValue(Constants.Metadata.LineItem.Description),
                        Size = lineItem.GetStringValue(Constants.Metadata.LineItem.Size),
                        PlacedPrice = lineItem.PlacedPrice,
                        LineItemTotal = lineItem.Quantity * lineItem.PlacedPrice,
                        LineItemDiscount = lineItem.LineItemDiscountAmount,
                        LineItemOrderLevelDiscount = lineItem.OrderLevelDiscountAmount,
                        Quantity = Convert.ToInt32(lineItem.Quantity),
                        Url = lineItem.GetEntryLink(language)
                    });
                }

            return items;
        }

        public string GetCustomerEmail()
        {
            return _purchaseOrder.GetBillingEmail();
        }
    }
}
