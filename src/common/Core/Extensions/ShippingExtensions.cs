/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Find.Api;
using Mediachase.Commerce.Core.Common;
using Mediachase.Commerce.Orders;

namespace OxxCommerceStarterKit.Core.Extensions
{
    public static class ShippingExtensions
    {
        public static void SetShipmentLineItemQuantity(this OrderForm orderForm)
        {
            if (orderForm == null)
            {
                throw new ArgumentNullException("orderForm");
            }
            if (!orderForm.Shipments.Any())
            {
                throw new ApplicationException("No shipping defined");
            }

            var indexed = orderForm.Shipments[0].LineItemIndexes;

            if (indexed != null)
            {
                var pairs = new List<int>();
               
                    string[] strArrays = indexed;
                    for (int i = 0; i < (int)strArrays.Length; i++)
                    {
                        string str1 = indexed[i];
                        string[] strArrays1 = str1.Split(new char[] { ':' });
                        int str2 = Int32.Parse(strArrays1[0]);
                        
                        pairs.Add(str2);
                    }

                foreach (var pair in pairs)
                {
                    orderForm.Shipments[0].RemoveLineItemIndex(pair);
                }
            }

            for (int i = 0; i < orderForm.LineItems.Count; i++)
            {
                orderForm.Shipments[0].AddLineItemIndex(i,orderForm.LineItems[i].Quantity);
            }
        }
    }
}
