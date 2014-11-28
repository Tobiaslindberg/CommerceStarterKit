using System;
using System.Linq;
using System.Workflow.ComponentModel;
using Mediachase.Commerce.Orders;

namespace EPiCode.Commerce.Workflow.Activities
{
	public partial class CalculatePurchaseOrderStatusActivity : PurchaseOrderBaseActivity
	{
		public CalculatePurchaseOrderStatusActivity()
			:base()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Executes the activity.
		/// </summary>
		/// <param name="executionContext">The execution context of the activity.</param>
		/// <returns>
		/// The <see cref="T:System.Workflow.ComponentModel.ActivityExecutionStatus"/> of the activity after executing the activity.
		/// </returns>
		protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
		{
			try
			{
				if (this.OrderGroup == null)
				{
					throw new NullReferenceException(this.OrderGroup is PurchaseOrder ? "purchaseOrder" : "PaymentPlan");
				}

				CalculateOrderShipmentsStatusesByOrderStatus(base.OrderStatus);

				//Calculate status only for specified statuses
				if ((base.OrderStatus & (OrderStatus.InProgress | 
									OrderStatus.PartiallyShipped)) == base.OrderStatus)
				{
					var newStatus = CalculateOrderGroupStatus();
					if (newStatus != base.OrderStatus)
					{
						base.ChangeOrderStatus(newStatus);
					}
				}
				// Retun the closed status indicating that this activity is complete.
				return ActivityExecutionStatus.Closed;
			}
			catch (Exception ex)
			{
				Logger.Error(GetType().Name + ": " + ex.Message, ex);
				// An unhandled exception occured.  Throw it back to the WorkflowRuntime.
				throw;
			}
		}

		private OrderStatus CalculateOrderGroupStatus()
		{
			OrderStatus retVal = base.OrderStatus;
			var shipments = base.OrderGroup.OrderForms[0].Shipments.ToArray();
			//var purchaseOrder = base.OrderGroup as Mediachase.Commerce.Orders.PurchaseOrder;

			OrderShipmentStatus avtiveStates = OrderShipmentStatus.InventoryAssigned | OrderShipmentStatus.AwaitingInventory 
											   | OrderShipmentStatus.Released | OrderShipmentStatus.Packing;

			bool canceledFound = shipments.Any(x => GetShipmentStatus(x) == OrderShipmentStatus.Cancelled);
			bool completedFound = shipments.Any(x => GetShipmentStatus(x) == OrderShipmentStatus.Shipped);
			bool activeFound = shipments.Any(x => (GetShipmentStatus(x) & avtiveStates) == GetShipmentStatus(x));

			if (!canceledFound && !completedFound && !activeFound)
			{
				//Not found canceled,active,completed
				
			} else if (canceledFound && !completedFound && !activeFound)
			{
				//All canceled
				//retVal = OrderStatus.Cancelled;
			}
			else if (!canceledFound && completedFound && !activeFound)
			{
				//All completed
				retVal = OrderStatus.Completed;
			}
			else if (!canceledFound && !completedFound && activeFound)
			{
				//All active
			}
			else if (canceledFound && completedFound && !activeFound)
			{
				//Found  cancelled and completed
				retVal = OrderStatus.Completed;
			}
			else if (canceledFound && !completedFound && activeFound)
			{
				//Found cancelled and active
			}
			else if (!canceledFound && completedFound && activeFound)
			{
				//Found completted and active
				retVal = OrderStatus.PartiallyShipped;
			}
			else if (canceledFound && completedFound && activeFound)
			{
				//Found cancelled and active and completed
				retVal = OrderStatus.PartiallyShipped;
			}
		
		
			
			return retVal;
		}

		private void CalculateOrderShipmentsStatusesByOrderStatus(OrderStatus orderStatus)
		{
			var shipments = base.OrderGroup.OrderForms[0].Shipments.ToArray();

			foreach (Shipment shipment in shipments)
			{
				if (string.IsNullOrEmpty(shipment.Status))
				{
					shipment.Status = OrderShipmentStatus.InventoryAssigned.ToString();
				}
				//End states
				if (shipment.Status == OrderShipmentStatus.Shipped.ToString() ||
					shipment.Status == OrderShipmentStatus.Cancelled.ToString())
				{
					continue;
				}
				//Inherit order state for shipment
				if (orderStatus == OrderStatus.Cancelled)
				{
					shipment.Status = OrderShipmentStatus.Cancelled.ToString();
				}
				else if (orderStatus == OrderStatus.OnHold)
				{
					shipment.Status = OrderShipmentStatus.OnHold.ToString();
				}
				else
				{
					//Restore back from onHold to prev state
					if (shipment.Status == OrderShipmentStatus.OnHold.ToString())
					{
						shipment.Status = shipment.PrevStatus;
					}
				}
			}
		}
		
	}
}
