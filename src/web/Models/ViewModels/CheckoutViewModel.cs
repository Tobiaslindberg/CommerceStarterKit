/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using Mediachase.Commerce.Orders.Dto;
using OxxCommerceStarterKit.Core.Attributes;
using OxxCommerceStarterKit.Core.Objects;
using OxxCommerceStarterKit.Web.Models.PageTypes.System;

namespace OxxCommerceStarterKit.Web.Models.ViewModels
{
    public class CheckoutViewModel : PageViewModel<CheckoutPage>
    {
        public CheckoutViewModel()
        {
            
        }

        public CheckoutViewModel(CheckoutPage currentPage) : base(currentPage)
        {            
        }

		public bool AcceptedTerms { get; set; }

		public ContentReference TermsArticle { get; set; }

        public Address BillingAddress { get; set; }

        public Address ShippingAddress { get; set; }

        [LocalizedDisplayName("/common/checkout/placeholder/socialsecuritynumber")]
        public string SocialSecurityNumber { get; set; }

		[LocalizedDisplayName("/common/checkout/placeholder/email")]
        public string Email { get; set; }

		[LocalizedDisplayName("/common/checkout/placeholder/phone")]
		public string Phone { get; set; }


        public PaymentInfo PaymentInfo { get; set; }

		[LocalizedDisplayName("/common/checkout/placeholder/password")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	
		[LocalizedDisplayName("/common/checkout/placeholder/confirmpassword")]
		[DataType(DataType.Password)]
		public string ConfirmPassword { get; set; }


		public bool MemberClub { get; set; }

		public Dictionary<string, string> AvailableCategories { get; set; }
		public int[] SelectedCategories { get; set; }
	}

    public class PaymentInfo
    {
        public PaymentInfo()
        {
            PaymentMethods = new List<PaymentMethodDto.PaymentMethodRow>();
        }

        public List<PaymentMethodDto.PaymentMethodRow> PaymentMethods { get; set; }
        public Guid SelectedPayment { get; set; }        
    }
}
