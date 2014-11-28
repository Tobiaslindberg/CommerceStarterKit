using System;
using System.ComponentModel;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;

namespace EPiCode.Commerce.Workflow.Activities
{
	public partial class PurchaseOrderBaseActivity: OrderGroupActivityBase
	{

		public static DependencyProperty ChangingOrderStatusEvent = DependencyProperty.Register("ChangingOrderStatus", typeof(EventHandler), typeof(PurchaseOrderBaseActivity));
		public static DependencyProperty ChangedOrderStatusEvent = DependencyProperty.Register("ChangedOrderStatus", typeof(EventHandler), typeof(PurchaseOrderBaseActivity));

		public PurchaseOrderBaseActivity()
			:base()
		{
			InitializeComponent();
		}


		private const string EventsCategory = "Handlers";

		#region Public Events
		/// <summary>
		/// Occurs when [changing order status].
		/// </summary>
		[ValidationOption(ValidationOption.Optional)]
		[Description("The ChangingOrderStatus event is raised before a order status has changed")]
		[Category(EventsCategory)]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public event EventHandler ChangingOrderStatus
		{
			add
			{
				base.AddHandler(PurchaseOrderBaseActivity.ChangingOrderStatusEvent, value);
			}
			remove
			{
				base.RemoveHandler(PurchaseOrderBaseActivity.ChangingOrderStatusEvent, value);
			}
		}


		/// <summary>
		/// Occurs when [changing order status].
		/// </summary>
		[ValidationOption(ValidationOption.Optional)]
		[Description("The ChangedOrderStatus event is raised a order status has changed")]
		[Category(EventsCategory)]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public event EventHandler ChangedOrderStatus
		{
			add
			{
				base.AddHandler(PurchaseOrderBaseActivity.ChangedOrderStatusEvent, value);
			}
			remove
			{
				base.RemoveHandler(PurchaseOrderBaseActivity.ChangedOrderStatusEvent, value);
			}
		}

		#endregion

		protected OrderStatus OrderStatus
		{
			get
			{
                if (this.OrderGroup is PurchaseOrder)
				    return OrderStatusManager.GetPurchaseOrderStatus(this.OrderGroup as Mediachase.Commerce.Orders.PurchaseOrder);
                else
                    return OrderStatusManager.GetPurchaseOrderStatus(this.OrderGroup as PaymentPlan);
			}
		}

		protected void ChangeOrderStatus(OrderStatus newStatus)
		{
			base.RaiseEvent(PurchaseOrderBaseActivity.ChangingOrderStatusEvent, this, EventArgs.Empty);

			this.OrderGroup.Status = newStatus.ToString();			

			base.RaiseEvent(PurchaseOrderBaseActivity.ChangedOrderStatusEvent, this, EventArgs.Empty);
		}

		protected static OrderShipmentStatus GetShipmentStatus(Shipment shipment)
		{
			OrderShipmentStatus retVal = OrderShipmentStatus.InventoryAssigned;
			if (!string.IsNullOrEmpty(shipment.Status))
			{
				retVal = (OrderShipmentStatus)Enum.Parse(typeof(OrderShipmentStatus), shipment.Status);
			}
			return retVal;
		}

		

	}
}
