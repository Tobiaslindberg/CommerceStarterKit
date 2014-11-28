/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OxxCommerceStarterKit.Core.PaymentProviders
{
    public class ShaCalculator : EncryptionCalculator
    {
        private readonly string _keyi;
        private readonly string _keyo;
        private readonly string _key;

        public ShaCalculator()
        {

        }

        public ShaCalculator(string keyi, string keyo, string key)
        {
            _keyi = keyi;
            _keyo = keyo;
            _key = key;
        }

        public string GetHex(OrderInfo message)
        {
            return base.HashSHAHex(_keyi,_keyo, message.ToString());
        }

        public string GetMac(OrderInfo info)
        {
            return calculateMac(info.GetOrderParams(), _key);
        }

        public string TestMac()
        {
            Dictionary<string,string> p = new Dictionary<string, string>();

            

            p.Add("amount","100");
            p.Add("currency","EUR");
            return calculateMac(p, _key);
        }


        private string calculateMac(IDictionary params_dict, string K_hexEnc)
        {
            //Create the message for MAC calculation sorted by the key
            ICollection keysRaw = params_dict.Keys;
            List<string> keys = new List<string>();
            foreach (var key in keysRaw)
            {
                keys.Add(key.ToString());
            }
            keys.Sort();

            string msg = "";
            foreach (var key in keys)
            {
                if (key != keys[0]) msg += "&";
                msg += key + "=" + params_dict[key];
            }

            //Decoding the secret Hex encoded key and getting the bytes for MAC calculation
            var K_bytes = new byte[K_hexEnc.Length / 2];
            for (int i = 0; i < K_bytes.Length; i++)
            {
                K_bytes[i] = byte.Parse(K_hexEnc.Substring(i * 2, 2), NumberStyles.HexNumber);
            }

            //Getting bytes from message
            var encoding = new UTF8Encoding();
            byte[] msg_bytes = encoding.GetBytes(msg);

            //Calculate MAC key
            var hash = new HMACSHA256(K_bytes);
            byte[] mac_bytes = hash.ComputeHash(msg_bytes);
            string mac = BitConverter.ToString(mac_bytes).Replace("-", "").ToLower();

            return mac;
        }
    }
}
