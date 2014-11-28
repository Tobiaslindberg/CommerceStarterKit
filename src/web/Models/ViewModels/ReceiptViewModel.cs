/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using EPiServer.Core;
using OxxCommerceStarterKit.Core.Objects.SharedViewModels;
using OxxCommerceStarterKit.Web.Models.PageTypes.System;

namespace OxxCommerceStarterKit.Web.Models.ViewModels
{
	public class ReceiptViewModel : PageViewModel<ReceiptPage>
	{
		public ReceiptViewModel()
		{
		}

		public ReceiptViewModel(ReceiptPage currentPage)
			: base(currentPage)
		{
			ThankYouText = currentPage.ThankYouText;
			ThankYouTitle = currentPage.ThankYouTitle;
		}

		public OrderViewModel Order { get; set; }
		public string CheckoutMessage { get; set; }

	    public string ThankYouTitle { get; set; }
	    public XhtmlString ThankYouText { get; set; }

	}
}
