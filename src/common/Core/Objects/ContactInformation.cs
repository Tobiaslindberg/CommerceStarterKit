/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using OxxCommerceStarterKit.Core.Attributes;

namespace OxxCommerceStarterKit.Core.Objects
{
    public class ContactInformation
    {
        public string Id { get; set; }

        public string Username { get; set; }

        [LocalizedDisplayName("/common/accountpages/firstname_label")]
        [Placeholder("/common/accountpages/placeholder/firstname")]
        public string FirstName { get; set; }

        [LocalizedDisplayName("/common/accountpages/lastname_label")]
        [Placeholder("/common/accountpages/placeholder/lastname")]
        public string LastName { get; set; }

        [LocalizedDisplayName("/common/accountpages/email_label")]
        [Placeholder("/common/accountpages/placeholder/email")]
        public string Email { get; set; }

         [LocalizedDisplayName("/common/accountpages/phone_label")]
         [Placeholder("/common/accountpages/placeholder/phone")]
        public string PhoneNumber { get; set; }
    }
}
