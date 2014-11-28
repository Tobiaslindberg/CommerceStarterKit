/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.ComponentModel.DataAnnotations;
using OxxCommerceStarterKit.Core.Attributes;

namespace OxxCommerceStarterKit.Core.Objects
{
    public class LoginForm
    {
        [LocalizedDisplayName("/common/account/username")]
        [DataType(DataType.Text)]
        [Required]
        public string Username { get; set; }

        [LocalizedDisplayName("/common/account/password")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [LocalizedDisplayName("/common/account/rememberme")]
        public bool RememberMe { get; set; }

		public string ValidationMessage { get; set; }
    }
}
