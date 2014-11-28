/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using OxxCommerceStarterKit.Core.Objects;

namespace OxxCommerceStarterKit.Core.Services
{
    public interface IOrderService
    {
        List<LineItem> GetItems();

        string GetCustomerEmail();
    }
}
