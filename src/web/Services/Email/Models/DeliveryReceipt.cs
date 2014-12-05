/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Markets;
using Mediachase.Commerce.Orders;
using OxxCommerceStarterKit.Core.Objects.SharedViewModels;

namespace OxxCommerceStarterKit.Web.Services.Email.Models
{
    public class DeliveryReceipt : EmailBase
    {
		private PurchaseOrder _purchaseOrder;
		public PurchaseOrder PurchaseOrder { get { return _purchaseOrder; } }

		private OrderViewModel _orderViewModel;
		public OrderViewModel OrderViewModel { get { return _orderViewModel; } }

		public string PurchaseOrderNumber { get; set; }
		public string BackendOrderNumber { get; set; }
		public string TrackingNumber { get; set; }

		public DeliveryReceipt(PurchaseOrder purchaseOrder)
		{
		    if (purchaseOrder == null) throw new ArgumentNullException("purchaseOrder cannot be null");

		    _purchaseOrder = purchaseOrder;
			_orderViewModel = new OrderViewModel(_purchaseOrder);

			To = _orderViewModel.Email;

            var localizationService = ServiceLocator.Current.GetInstance<LocalizationService>();
            ICurrentMarket currentMarket = ServiceLocator.Current.GetInstance<ICurrentMarket>();
            IMarketService marketService = ServiceLocator.Current.GetInstance<IMarketService>();
            IMarket market = GetMarketForOrder(purchaseOrder, marketService, currentMarket);

            string emailSubject = localizationService.GetStringByCulture("/common/receipt/email/subject", market.DefaultLanguage);
                
            Subject = string.Format(emailSubject, _purchaseOrder.TrackingNumber);
            BackendOrderNumber = _orderViewModel.ErpOrderNumber;
            PurchaseOrderNumber = _purchaseOrder.TrackingNumber;

			// Get first shipment tracking number
            if (_purchaseOrder.OrderForms != null &&
				_purchaseOrder.OrderForms.Count > 0 &&
				_purchaseOrder.OrderForms[0].Shipments != null &&
				_purchaseOrder.OrderForms[0].Shipments.Count > 0)
			{
				TrackingNumber = _purchaseOrder.OrderForms[0].Shipments[0].ShipmentTrackingNumber;
			}
		}

        protected IMarket GetMarketForOrder(PurchaseOrder purchaseOrder, IMarketService marketService, ICurrentMarket currentMarket)
        {
            if (purchaseOrder.MarketId != null && purchaseOrder.MarketId.Value != null)
            {
                return marketService.GetMarket(purchaseOrder.MarketId);
            }
            return currentMarket.GetCurrentMarket();
        }
    }
}
