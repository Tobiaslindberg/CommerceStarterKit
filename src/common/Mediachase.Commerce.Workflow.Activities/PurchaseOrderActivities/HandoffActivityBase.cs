using System.Collections.Generic;
using System.ComponentModel;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;
using Mediachase.Commerce.Inventory;

namespace EPiCode.Commerce.Workflow.Activities
{
    public partial class HandoffActivityBase : OrderGroupActivityBase
    {
        public static DependencyProperty PickupWarehouseInShipmentProperty = DependencyProperty.Register("PickupWarehouseInShipment", typeof(Dictionary<int, IWarehouse>), typeof(HandoffActivityBase));

        [ValidationOption(ValidationOption.Required)]
        [Browsable(true)]
        public Dictionary<int, IWarehouse> PickupWarehouseInShipment
        {
            get
            {
                return (Dictionary<int, IWarehouse>)(base.GetValue(HandoffActivityBase.PickupWarehouseInShipmentProperty));
            }
            set
            {
                base.SetValue(HandoffActivityBase.PickupWarehouseInShipmentProperty, value);
            }
        }

        public HandoffActivityBase()
			:base()
        {
            InitializeComponent();
        }
    }
}
