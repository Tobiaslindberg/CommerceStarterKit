/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Orders;
using OxxCommerceStarterKit.Core.Objects.SharedViewModels;

namespace OxxCommerceStarterKit.Web.Services.Email.Models
{
    public class Receipt : EmailBase
    {
		private PurchaseOrder _purchaseOrder;
		public PurchaseOrder PurchaseOrder { get { return _purchaseOrder; } }

		private OrderViewModel _orderViewModel;
		public OrderViewModel OrderViewModel { get { return _orderViewModel; } }

		public Receipt(PurchaseOrder purchaseOrder)
		{
			_purchaseOrder = purchaseOrder;

			_orderViewModel = new OrderViewModel(_purchaseOrder);

			To = _orderViewModel.Email;

			var localizationService = ServiceLocator.Current.GetInstance<LocalizationService>();

			Subject = string.Format(localizationService.GetString("/common/receipt/email/subject"), _purchaseOrder.TrackingNumber);
		}


    }
}
