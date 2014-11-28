/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using Mediachase.Commerce.Orders;

namespace OxxCommerceStarterKit.Core.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        PurchaseOrder GetOrderByTrackingNumber(string trackingNumber);
		List<PurchaseOrder> GetOrdersByUserId(Guid customerId);
    }
}
