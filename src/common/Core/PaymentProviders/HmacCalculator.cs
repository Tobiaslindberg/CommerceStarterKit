/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

namespace OxxCommerceStarterKit.Core.PaymentProviders
{
    public class HmacCalculator : EncryptionCalculator
    {
        private readonly string _key;

        public HmacCalculator()
        {
            
        }

        public HmacCalculator(string key)
        {
            _key = key;
        }

        public string GetHex(OrderInfo message)
        {
            return base.HashHMACHex(_key, message.ToString());
        }
    }
}
