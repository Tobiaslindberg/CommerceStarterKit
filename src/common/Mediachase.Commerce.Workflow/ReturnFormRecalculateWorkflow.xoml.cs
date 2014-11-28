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

namespace EPiCode.Commerce.Workflow
{
	public partial class ReturnFormRecalculateWorkflow : SequentialWorkflowActivity
	{
		private OrderForm _returnOrderForm;

		
		public OrderForm ReturnOrderForm
		{
			get
			{
				return _returnOrderForm;
			}
			set
			{
				_returnOrderForm = value;
			}
		}
	}
}
