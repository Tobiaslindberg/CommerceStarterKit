/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Linq;
using System.Web.Mvc;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Security;
using OxxCommerceStarterKit.Core.Objects.SharedViewModels;
using OxxCommerceStarterKit.Core.Repositories.Interfaces;
using OxxCommerceStarterKit.Web.Models.PageTypes;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Controllers
{
	public class OrdersPageController : PageControllerBase<OrdersPage>
	{
		private readonly IOrderRepository _orderRepository;

		public OrdersPageController(IOrderRepository orderRepository)
		{
			_orderRepository = orderRepository;
		}


		// GET: OrdersPage
		public ActionResult Index(OrdersPage currentPage)
		{
			var model = new OrdersPageViewModel(currentPage);
			model.CustomerName = CustomerContext.Current.CurrentContact.FirstName;

			var orders = _orderRepository.GetOrdersByUserId(SecurityContext.Current.CurrentUserId);
			model.Orders = orders.OrderByDescending(x => x.Created).Select(x => CreateOrderViewModel(x)).ToList();
			
			return View(model);
		}


		private OrderViewModel CreateOrderViewModel(PurchaseOrder order)
		{
			var model = new OrderViewModel(order);
			// TODO order-payment method
			model.PaymentMethod = order.ProviderId;

			return model;
		}
	}
}
