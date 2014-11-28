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
    public class DeliveryReceipt : EmailBase
    {
		private PurchaseOrder _purchaseOrder;
		public PurchaseOrder PurchaseOrder { get { return _purchaseOrder; } }

		private OrderViewModel _orderViewModel;
		public OrderViewModel OrderViewModel { get { return _orderViewModel; } }


		public string PurchaseOrderNumber { get; set; }
		public string JeevesOrderNumber { get; set; }
		public string TrackingNumber { get; set; }

		public DeliveryReceipt(PurchaseOrder purchaseOrder)
		{
			_purchaseOrder = purchaseOrder;

			_orderViewModel = new OrderViewModel(_purchaseOrder);

			To = _orderViewModel.Email;

			var localizationService = ServiceLocator.Current.GetInstance<LocalizationService>();

            Subject = string.Format("Ordrebekreftelse - Sporingsnummer:", _purchaseOrder.TrackingNumber); // TODO: Hente fra språkfilen. Sett currentculture til kultur til ordren og ikke contexten vi er i nå, som er engelsk.

			JeevesOrderNumber = _orderViewModel.ErpOrderNumber;

			if (_purchaseOrder != null)
			{
				PurchaseOrderNumber = _purchaseOrder.TrackingNumber;


				if (_purchaseOrder.OrderForms != null &&
					_purchaseOrder.OrderForms.Count > 0 &&
					_purchaseOrder.OrderForms[0].Shipments != null &&
					_purchaseOrder.OrderForms[0].Shipments.Count > 0)
				{
					TrackingNumber = _purchaseOrder.OrderForms[0].Shipments[0].ShipmentTrackingNumber;
				}
			}
		}
    }
}
