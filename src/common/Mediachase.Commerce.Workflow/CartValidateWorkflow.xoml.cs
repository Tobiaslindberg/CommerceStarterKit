using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Mediachase.Commerce.Orders;
using System.Collections.Specialized;
using System.Collections.Generic;
using Mediachase.Commerce.Inventory;
using System.Linq;

namespace EPiCode.Commerce.Workflow
{
	public partial class CartValidateWorkflow : SequentialWorkflowActivity
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
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        public StringDictionary Warnings
        {
            get
            {
                return _Warnings;
            }
            set
            {
                _Warnings = value;
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

        private void CheckInstoreInventory(object sender, ConditionalEventArgs e)
        {
            e.Result = PickupWarehouseInShipment != null && PickupWarehouseInShipment.Any();
        }
	}
}
