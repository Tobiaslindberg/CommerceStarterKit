/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Website.Helpers;
using OxxCommerceStarterKit.Core.Extensions;
using OxxCommerceStarterKit.Core.Objects.SharedViewModels;
using OxxCommerceStarterKit.Core.PaymentProviders;
using OxxCommerceStarterKit.Core.PaymentProviders.DIBS;
using OxxCommerceStarterKit.Core.PaymentProviders.Payment;
using OxxCommerceStarterKit.Core.Repositories.Interfaces;
using OxxCommerceStarterKit.Web.Business;
using OxxCommerceStarterKit.Web.Business.Analytics;
using OxxCommerceStarterKit.Web.Models.PageTypes;
using OxxCommerceStarterKit.Web.Models.PageTypes.Payment;
using OxxCommerceStarterKit.Web.Models.PageTypes.System;
using OxxCommerceStarterKit.Web.Models.ViewModels;
using OxxCommerceStarterKit.Web.Models.ViewModels.Payment;
using LineItem = OxxCommerceStarterKit.Core.Objects.LineItem;

namespace OxxCommerceStarterKit.Web.Controllers
{
	public class DibsPaymentController : PaymentBaseController<DibsPaymentPage>
	{
		private readonly IContentRepository _contentRepository;
		private readonly IOrderRepository _orderRepository;
	    private readonly SiteConfiguration _siteConfiguration;

		public DibsPaymentController(IContentRepository contentRepository, IOrderRepository orderRepository)
		{
			_contentRepository = contentRepository;
			_orderRepository = orderRepository;
            _siteConfiguration = SiteConfiguration.Current();
		}

		[RequireSSL]
		public ActionResult Index(DibsPaymentPage currentPage)
		{
			CartHelper ch = new CartHelper(Cart.DefaultName);

			ch.Cart.AcceptChanges();

			var orderInfo = new OrderInfo()
			{
                // Dibs expect the order to be without decimals
				Amount = Convert.ToInt32(ch.Cart.Total * 100),
				Currency = ch.Cart.BillingCurrency,
				OrderId = ch.Cart.GeneratePredictableOrderNumber(),
				IsTest = Tools.IsAppSettingTrue("DIBS.TestMode"),
                ExpandOrderInformation = true
			};

            DibsPaymentViewModel model = new DibsPaymentViewModel(_contentRepository, currentPage, orderInfo, ch.Cart);

            // Get cart and track it
            Api.CartController cartApiController = new Api.CartController();
            cartApiController.Language = currentPage.LanguageBranch;
            List<Core.Objects.LineItem> lineItems = cartApiController.GetItems(Cart.DefaultName);
            TrackBeforePayment(lineItems);

			return View(model);
		}

        private void TrackBeforePayment(IEnumerable<LineItem> lineItems)
        {
            // Track Analytics. 
            GoogleAnalyticsTracking tracking = new GoogleAnalyticsTracking(ControllerContext.HttpContext);
            
            // Add the products
            int i = 1;
            foreach (LineItem lineItem in lineItems)
            {
                tracking.ProductAdd(code: lineItem.Code,
                    name: lineItem.Name,
                    quantity: (int)lineItem.Quantity,
                    price: (double)lineItem.PlacedPrice,
                    position: i
                    );
                i++;
            }

            // Step 3 is to pay
            tracking.Action("checkout", "{\"step\":3}");

            // Send it off
            tracking.Custom("ga('send', 'pageview');");
        }

        /// <summary>
        /// Processes the payment after we get back from the
        /// payment provider
        /// </summary>
        /// <param name="result">The result posted from the provider.</param>
		[HttpPost]
		[RequireSSL]
		public ActionResult ProcessPayment(DibsPaymentPage currentPage, DibsPaymentResult result)
		{
            ReceiptPage receiptPage = _contentRepository.Get<ReceiptPage>(_siteConfiguration.Settings.ReceiptPage);

		    if(_log.IsDebugEnabled)
			    _log.DebugFormat("Payment processed: {0}", result);

			CartHelper cartHelper = new CartHelper(Cart.DefaultName);
			string message = "";
			OrderViewModel orderViewModel = null;

			if (cartHelper.Cart.OrderForms.Count > 0)
			{
				var payment = cartHelper.Cart.OrderForms[0].Payments[0] as DibsPayment;

				if (payment != null)
				{
					payment.CardNumberMasked = result.CardNumberMasked;
					payment.CartTypeName = result.CardTypeName;
					payment.TransactionID = result.Transaction;
					payment.TransactionType = TransactionType.Authorization.ToString();
					payment.Status = result.Status;
					cartHelper.Cart.Status = DIBSPaymentGateway.PaymentCompleted;
				}
				else
				{
					throw new Exception("Not a DIBS Payment");
				}

				var orderNumber = cartHelper.Cart.GeneratePredictableOrderNumber();

				_log.Debug("Order placed - order number: " + orderNumber);

				cartHelper.Cart.OrderNumberMethod = CartExtensions.GeneratePredictableOrderNumber;

				var results = OrderGroupWorkflowManager.RunWorkflow(cartHelper.Cart, OrderGroupWorkflowManager.CartCheckOutWorkflowName);
				message = string.Join(", ", OrderGroupWorkflowManager.GetWarningsFromWorkflowResult(results));

				var order = _orderRepository.GetOrderByTrackingNumber(orderNumber);
				
				// Must be run after order is complete, 
                // This will release the order for shipment and 
                // send the order receipt by email
				OnPaymentComplete(order);

				orderViewModel = new OrderViewModel(order);
			}

			ReceiptViewModel model = new ReceiptViewModel(receiptPage);
			model.CheckoutMessage = message;
			model.Order = orderViewModel;

            // Track successfull order in Google Analytics
            TrackAfterPayment(model);

			return View("ReceiptPage", model);
		}

	    private void TrackAfterPayment(ReceiptViewModel model)
	    {
            // Track Analytics 
            GoogleAnalyticsTracking tracking = new GoogleAnalyticsTracking(ControllerContext.HttpContext);

            // Add the products
            int i = 1;
            foreach (OrderLineViewModel orderLine in model.Order.OrderLines)
            {
                if(string.IsNullOrEmpty(orderLine.Code) == false)
                {
                    tracking.ProductAdd(code: orderLine.Code,
                        name: orderLine.Name,
                        quantity: orderLine.Quantity,
                        price: (double)orderLine.Price,
                        position: i
                        );
                    i++;
                }
            }

            // And the transaction itself
            tracking.Purchase(model.Order.OrderNumber,
                null, (double) model.Order.TotalAmount, (double) model.Order.Tax, (double) model.Order.Shipping);
	    }

	    [HttpPost]
        [RequireSSL]
        public ActionResult CancelPayment(DibsPaymentPage currentPage, DibsPaymentResult result)
        {
            CancelPaymentViewModel model = new CancelPaymentViewModel(currentPage, result);
            return View("CancelPayment", model);
        }
    }
}
