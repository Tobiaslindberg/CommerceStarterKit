using System.Collections.Generic;
using System.Collections.Specialized;
using System.Workflow.Activities;
using Mediachase.Commerce.Inventory;
using Mediachase.Commerce.Orders;

namespace EPiCode.Commerce.Workflow.Admin
{
	public partial class POSplitShipmentsWorkflow : SequentialWorkflowActivity
	{
		private OrderGroup _OrderGroup;

		/// <summary>
		/// Gets or sets the order group.
		/// </summary>
		/// <value>The order group.</value>
		public OrderGroup OrderGroup
		{
			get
			{
				return _OrderGroup;
			}
			set
			{
				_OrderGroup = value;
			}
		}

		private StringDictionary _Warnings = new StringDictionary();
		/// <summary>
		/// Gets the warnings.
		/// </summary>
		/// <value>The warnings.</value>
		public StringDictionary Warnings
		{
			get
			{
				return _Warnings;
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
