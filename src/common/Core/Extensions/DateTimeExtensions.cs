/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;

namespace OxxCommerceStarterKit.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime? GetNullableDateTime(this DateTime? dateTime)
        {
            // used for getting local time for Price ValidUntil field
            return Convert.ToDateTime(dateTime).ToLocalTime();
        }
    }
}
