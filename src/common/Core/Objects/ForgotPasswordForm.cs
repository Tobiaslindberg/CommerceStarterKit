/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.ComponentModel.DataAnnotations;

namespace OxxCommerceStarterKit.Core.Objects
{
    public class ForgotPasswordForm  //: IForgotPassword
    {
        public string Token { get; set; }
        public string FullUrl { get; set; }

        [Required]
        public string Mail { get; set; }
        public bool SentMail { get; set; }

		public string ValidationMessage { get; set; }
    }
}
