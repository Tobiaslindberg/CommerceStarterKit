/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Web.Security;
using Mediachase.Commerce.Customers;
using OxxCommerceStarterKit.Core.Extensions;
using OxxCommerceStarterKit.Core.Objects;
using OxxCommerceStarterKit.Core.Repositories.Interfaces;

namespace OxxCommerceStarterKit.Core.Repositories
{
    public class ContactRepository : IRepository<ContactInformation>
    {
        private readonly CustomerContact _contact;

        public ContactRepository(CustomerContact contact)
        {
            _contact = contact;
        }

        public ContactRepository(string email)
        {
            MembershipUser user = Membership.GetUser(email);
            if (user == null) throw new ApplicationException("There is no membership for user: '" + email + "'");

            _contact = CustomerContext.Current.GetContactForUser(user);

        }

        public ContactRepository() : this(CustomerContext.Current.CurrentContact)
        {
            
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IList<ContactInformation> GetAll()
        {
            throw new NotImplementedException();
        }

        public ContactInformation Get(int id)
        {
            throw new NotImplementedException();
        }

        public void Save(ContactInformation contactInformation)
        {
            _contact.FullName = string.Format("{0} {1}", contactInformation.FirstName, contactInformation.LastName);
            _contact.FirstName = contactInformation.FirstName;
            _contact.LastName = contactInformation.LastName;
            _contact.Email = contactInformation.Email;
            _contact.SetPhoneNumber(contactInformation.PhoneNumber);           
            _contact.SaveChanges();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public ContactInformation Get()
        {
			if (_contact != null)
			{
				return new ContactInformation()
				{
					FirstName = _contact.FirstName,
					LastName = _contact.LastName,
					Email = _contact.Email,
					PhoneNumber = _contact.PhoneNumber()
				};
			}
			else
			{
				return new ContactInformation();
			}
        }
    }
}
