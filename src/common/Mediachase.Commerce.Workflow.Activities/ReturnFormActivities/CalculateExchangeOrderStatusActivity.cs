using System.Workflow.ComponentModel;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;
using System;

namespace EPiCode.Commerce.Workflow.Activities
{
	public partial class CalculateExchangeOrderStatusActivity : ReturnFormBaseActivity
	{
		public CalculateExchangeOrderStatusActivity()
			:base()
		{
			InitializeComponent();
		}

		protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
		{
			try
			{
				if (base.ReturnFormStatus == ReturnFormStatus.Complete)
				{
					//Need change ExchangeOrder from AvaitingCompletition to InProgress
					var exchangeOrder = ReturnExchangeManager.GetExchangeOrderForReturnForm(base.ReturnOrderForm);
					if (exchangeOrder != null && OrderStatusManager.GetOrderGroupStatus(exchangeOrder) == OrderStatus.AwaitingExchange)
					{
						OrderStatusManager.ProcessOrder(exchangeOrder);
						exchangeOrder.AcceptChanges();
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
	}
}
