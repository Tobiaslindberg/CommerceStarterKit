/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;

namespace OxxCommerceStarterKit.Web.Models
{
    public class ShippingMethodAndRate
    {
        public string ShippingMethodNameAndRate;
        public string ShippingMethodName;
        public Guid ShippingMethodId;
        public decimal ShippingRate;

        public ShippingMethodAndRate(string name, string nameAndPrice, decimal shippingRate, Guid id)
        {
            ShippingMethodName = name;
            ShippingMethodNameAndRate = nameAndPrice;
            ShippingMethodId = id;
            ShippingRate = shippingRate;
        }
    }
}
