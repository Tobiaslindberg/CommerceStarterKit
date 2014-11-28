/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using EPiServer.Core;
using EPiServer.Security;
using EPiServer.Shell.Services.Rest;
using OxxCommerceStarterKit.Core;

namespace OxxCommerceStarterKit.Web.Api
{
    [RestStore(Constants.UIHint.PaymentMethod)]
    public class PaymentMethodRestStore : RestControllerBase
    {
        private List<string> _paymentMethods = new List<string>();
        
        public PaymentMethodRestStore()
        {
            _paymentMethods.Add("Not implemented yet");
        }

        public RestResult Get(string name)
        {
            if(PrincipalInfo.HasEditAccess == false)
                throw new AccessDeniedException("Rest Store requires edit access.");

            IEnumerable<string> matches;
            if (String.IsNullOrEmpty(name) || String.Equals(name, "*", StringComparison.OrdinalIgnoreCase))
            {
                // Get all of them
                matches = _paymentMethods;
            }
            else
            {
                //Remove * in the end of name          
                name = name.Substring(0, name.Length - 1);
                //Match beginning with
                matches = _paymentMethods.Where(e => e.StartsWith(name, StringComparison.OrdinalIgnoreCase));
            }
            return Rest(matches
                .OrderBy(m => m)
                .Select(m => new {Name = m, Id = m}));
        }
    }
}
