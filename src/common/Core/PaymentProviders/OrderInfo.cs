/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace OxxCommerceStarterKit.Core.PaymentProviders
{
    public class OrderInfo
    {
        public OrderInfo()
        {
            OrderLines = new List<string>();
            /// TODO: Hardcoded, needs to be retrieved from settings
            VatPercent = 25;
        }

        public string Merchant { get; set; }
        public int Amount { get; set; }
        public int VatPercent { get; set; }
        public string Currency { get; set; }
        public string AcceptReturnUrl { get; set; }
        public string CancelReturnUrl { get; set; }
        public bool IsTest { get; set; }
        public string OrderId { get; set; }
        public string OrderLinesHeader { get; set; }
        public List<string> OrderLines { get; set; }
        public bool ExpandOrderInformation { get; set; }
        

        /// <summary>
        /// Returns the DIBS parameter string used for MAC calculation
        /// </summary>
        /// <remarks>
        /// Arguments must be added alphabetically, but in two groups. First
        /// upper case parameters alphabetically, and then lower case parameters
        /// alphabetically.
        /// All parameters that start with "s_" must be part of the calculation.
        /// Required Parameters are:
        /// acceptReturnUrl, amount, currency, merchant, orderId
        /// </remarks>
        /// <returns>
        /// A <see cref="System.String" /> with all neccessary parameters for
        /// MAC calculation.
        /// </returns>
        public override string ToString()
        {
            List<string> strings = new List<string>();

            IDictionary orderParams = GetOrderParams();
            foreach (DictionaryEntry orderParam in orderParams)
            {
                strings.Add(string.Format("{0}={1}", orderParam.Key.ToString(), orderParam.Value.ToString()));
            }
            return String.Join("&", strings);
        }

        public IDictionary GetOrderParams()
        {
            var dictionary = new OrderedDictionary();
            
            if (!string.IsNullOrEmpty(AcceptReturnUrl))
                dictionary.Add("acceptReturnurl", AcceptReturnUrl);

            if (Amount != 0)
                dictionary.Add("amount", Amount.ToString());

            if (!string.IsNullOrEmpty(CancelReturnUrl))
                dictionary.Add("cancelReturnUrl", CancelReturnUrl);

            if (!string.IsNullOrEmpty(Currency))
                dictionary.Add("currency", Currency);

            if(ExpandOrderInformation)
                dictionary.Add("expandOrderInformation", "1");

            if (!string.IsNullOrEmpty(Merchant))
                dictionary.Add("merchant", Merchant);

            // Because of the sorting rules, order lines needs 
            // to come before the headers in the calculation array
            if (OrderLines.Any())
            {
                for (int i = 0; i < OrderLines.Count; i++)
                {
                    int rowNumber = i + 1;
                    dictionary.Add("oiRow" + rowNumber.ToString(), OrderLines[i]);
                }
            }

            if (!string.IsNullOrEmpty(OrderLinesHeader))
            {
                dictionary.Add("oiTypes", OrderLinesHeader);
            }

            if (!string.IsNullOrEmpty(OrderId))
                dictionary.Add("orderid", OrderId);

            // DIBS only accepts "1" for this parameter
            if (IsTest)
                dictionary.Add("test", "1");

            return dictionary;

        }
    }
}
