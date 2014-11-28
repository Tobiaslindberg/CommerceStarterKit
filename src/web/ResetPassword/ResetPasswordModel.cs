/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace OxxCommerceStarterKit.Web.ResetPassword
{
    [EPiServerDataStore(AutomaticallyRemapStore = true, AutomaticallyCreateStore = true)]
    public class ResetPasswordModel
    {
        public ResetPasswordModel() { }

        public DateTime ExpireDate { get; set; }
        public string Hash { get; set; }
        public Identity Id { get; set; }
        public string UserName { get; set; }
    }
}
