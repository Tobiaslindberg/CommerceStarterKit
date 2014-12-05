/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using EPiServer.BaseLibrary.Scheduling;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using log4net;
using Mediachase.Commerce.Orders;
using OxxCommerceStarterKit.Core;
using OxxCommerceStarterKit.Core.Services;

namespace OxxCommerceStarterKit.Web.Jobs
{
	public class ExportOrderInformation
	{
		public string PurchaseOrderNumber { get; set; }
		public string ExternalOrderId { get; set; }
	}


	[ScheduledPlugIn(DisplayName = "Export Orders to Backend System", 
        Description = "Checks all open orders and checks if they are ready to be exported to a back end system for further processing.")]
	public class ExportOrderJob : JobBase
	{
		private bool _stopSignaled;
		protected static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public ExportOrderJob()
		{
			IsStoppable = true;
		}

		/// <summary>
		/// Called when a user clicks on Stop for a manually started job, or when ASP.NET shuts down.
		/// </summary>
		public override void Stop()
		{
			_stopSignaled = true;
		}

		/// <summary>
		/// Retrieves order for export and calls the IExportOrderService for each one
		/// </summary>
		/// <returns>A status message to be stored in the database log and visible from admin mode</returns>
		public override string Execute()
		{
            PrincipalInfo.CurrentPrincipal = PrincipalInfo.CreatePrincipal("admin");
			//Call OnStatusChanged to periodically notify progress of job for manually started jobs
			OnStatusChanged("Starting looking for orders to export.");

			Stopwatch tmr = Stopwatch.StartNew();
			List<ExportOrderInformation> results = new List<ExportOrderInformation>();
			List<PurchaseOrder> orders = GetOrdersToExport();
			tmr.Stop();

			_log.DebugFormat("Found {0} orders to export in {1}ms", orders.Count, tmr.ElapsedMilliseconds);


			if (_stopSignaled) return "Job was stopped";

			IExportOrderService service = ServiceLocator.Current.GetInstance<IExportOrderService>();

			foreach (PurchaseOrder purchaseOrder in orders)
			{
				if (_stopSignaled) return "Job was stopped";
				OnStatusChanged(string.Format("Exporting order: {0}", purchaseOrder.TrackingNumber));

				results.Add(ExportOrder(purchaseOrder, service));
			}
			
			return string.Format("Exported {0} orders", results.Count);
		}

		private ExportOrderInformation ExportOrder(PurchaseOrder purchaseOrder, IExportOrderService service)
		{
            // Export to back-end system, and retrieves an external id that is stored on the order
			string externalOrderNumber = service.ExportOrder(purchaseOrder);
			_log.DebugFormat("Exported {0} to external system, got {1} back", purchaseOrder.TrackingNumber, externalOrderNumber);
			
			ExportOrderInformation result = new ExportOrderInformation
			{
				ExternalOrderId = externalOrderNumber,
				PurchaseOrderNumber = purchaseOrder.TrackingNumber
			};

			return result;
		}

		private List<PurchaseOrder> GetOrdersToExport()
		{
			// For more complex searches, see
			// http://world.episerver.com/Blogs/Shannon-Gray/Dates/2012/12/EPiServer-Commerce-Order-Search-Made-Easy/
			PurchaseOrder[] activeOrders = OrderContext.Current.FindActiveOrders();

			List<PurchaseOrder> orders = new List<PurchaseOrder>();
			foreach (PurchaseOrder purchaseOrder in activeOrders)
			{
                // TODO: Identify orders that should be exported to back-end (ERP) system
                // You need to determine what causes an order to be ready for export.
                // Suggested approach: Store order id from backend (erp) on order, if the id is null,
                // it has not been exported yet. Assume you get some sort of tracking id back from your
                // backend system.
                if (string.IsNullOrWhiteSpace(purchaseOrder[Constants.Metadata.PurchaseOrder.BackendOrderNumber] as string))
                    orders.Add(purchaseOrder);
			}

			return orders;
		}
	}
}
