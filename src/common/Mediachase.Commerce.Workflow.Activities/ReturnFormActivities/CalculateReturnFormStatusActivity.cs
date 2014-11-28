using System.Workflow.ComponentModel;
using Mediachase.Commerce.Orders;
using System;

namespace EPiCode.Commerce.Workflow.Activities
{
	public partial class CalculateReturnFormStatusActivity : ReturnFormBaseActivity
	{
		public CalculateReturnFormStatusActivity()
			:base()
		{
			InitializeComponent();
		}

		protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
		{
			try
			{
				var newStatus = CalculateReturnFormStatus();
				if (newStatus != base.ReturnFormStatus)
				{
					ChangeReturnFormStatus(newStatus);
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

		private ReturnFormStatus CalculateReturnFormStatus()
		{
			ReturnFormStatus retVal = base.ReturnFormStatus;

			return retVal;
		}
	}
}
