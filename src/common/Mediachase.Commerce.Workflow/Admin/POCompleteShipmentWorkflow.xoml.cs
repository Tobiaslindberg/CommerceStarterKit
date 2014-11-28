using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Inventory;
using System.Collections.Generic;

namespace EPiCode.Commerce.Workflow.Admin
{
	public partial class POCompleteShipmentWorkflow : SequentialWorkflowActivity
	{
		private Shipment _shipment;

		/// <summary>
		/// Gets or sets the order group.
		/// </summary>
		/// <value>The order group.</value>
		public Shipment Shipment
		{
			get
			{
				return _shipment;
			}
			set
			{
				_shipment = value;
			}
		}

		private OrderGroup _orderGroup;

		/// <summary>
		/// Gets or sets the order group.
		/// </summary>
		/// <value>The order group.</value>
		public OrderGroup OrderGroup
		{
			get
			{
				return _orderGroup;
			}
			set
			{
				_orderGroup = value;
			}
		}

		private Dictionary<int, IWarehouse> _PickupWarehouseInShipment;
		/// <summary>
		/// Get or sets the warehouse that is designated to fulfill the in-store pickup order.
		/// </summary>
		public Dictionary<int, IWarehouse> PickupWarehouseInShipment
		{
			get
			{
				return _PickupWarehouseInShipment;
			}
			set
			{
				_PickupWarehouseInShipment = value;
			}
		}
	}
}
