/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using Mediachase.Commerce.Customers;

namespace OxxCommerceStarterKit.Core.Extensions
{
    public static class CustomerContactExtensions
    {
        public static void SetPhoneNumber(this CustomerContact contact, string phone)
        {
            if (contact.Properties.Contains(Constants.Metadata.Customer.PhoneNumber))
            {
                contact[Constants.Metadata.Customer.PhoneNumber] = phone;
            }
        }

        public static string PhoneNumber(this CustomerContact contact)
        {
            if (contact.Properties.Contains(Constants.Metadata.Customer.PhoneNumber))
            {
                return contact[Constants.Metadata.Customer.PhoneNumber] == null ? String.Empty : contact[Constants.Metadata.Customer.PhoneNumber].ToString();
            }
            return string.Empty;
        }


		public static void SetCategories(this CustomerContact contact, int[] values)
		{
			if (contact.Properties.Contains(Constants.Metadata.Customer.Category))
			{
				contact[Constants.Metadata.Customer.Category] = values;
			}
		}


		public static int[] GetCategories(this CustomerContact contact)
		{
			if (contact.Properties.Contains(Constants.Metadata.Customer.Category))
			{
				return (int[])contact[Constants.Metadata.Customer.Category];
			}
			return null;
		}

		public static bool GetHasPassword(this CustomerContact contact)
		{
			if (contact.Properties.Contains(Constants.Metadata.Customer.HasPassword))
			{
				return contact[Constants.Metadata.Customer.HasPassword] == null ? false : (bool)contact[Constants.Metadata.Customer.HasPassword];
			}
			return false;
		}

		public static void SetHasPassword(this CustomerContact contact, bool hasPassword)
		{
			if (contact.Properties.Contains(Constants.Metadata.Customer.HasPassword))
			{
				contact[Constants.Metadata.Customer.HasPassword] = hasPassword;
			}
		}
    }
}
