using System;
using System.Workflow.ComponentModel;

namespace EPiCode.Commerce.Workflow.Activities
{
    public partial class CreatePurchaseOrderActivity : OrderGroupActivityBase
    {
        public CreatePurchaseOrderActivity()
			: base()
        {
            InitializeComponent();
        }

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
			try
			{
				// Validate the properties at runtime
				this.ValidateRuntime();

				var cart = OrderGroup as Mediachase.Commerce.Orders.Cart;

				if (cart != null)
				{
					var purchaseOrder = cart.SaveAsPurchaseOrder();

					cart.Delete();
					cart.AcceptChanges();
					OrderGroup = purchaseOrder;
				}
				return ActivityExecutionStatus.Closed;
			}
			catch (Exception ex)
			{
				Logger.Error(GetType().Name + ": " + ex.Message, ex);
				throw;
			}
        }
    }
}
