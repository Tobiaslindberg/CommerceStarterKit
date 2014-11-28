using System.ComponentModel;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;
using log4net;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;


namespace EPiCode.Commerce.Workflow.Activities
{
	public partial class ReturnFormBaseActivity: Activity
	{
		public static DependencyProperty ReturnOrderFormProperty = DependencyProperty.Register("ReturnOrderForm", typeof(OrderForm), typeof(ReturnFormBaseActivity));

		protected readonly ILog Logger;
	
		[ValidationOption(ValidationOption.Required)]
		[Browsable(true)]
		public OrderForm ReturnOrderForm
		{
			get
			{
				return (OrderForm)(base.GetValue(ReturnFormBaseActivity.ReturnOrderFormProperty));
			}
			set
			{
				base.SetValue(ReturnFormBaseActivity.ReturnOrderFormProperty, value);
			}
		}

		protected ReturnFormStatus ReturnFormStatus
		{
			get
			{
				return ReturnFormStatusManager.GetReturnFormStatus(ReturnOrderForm);
			}
		}

		protected void ChangeReturnFormStatus(ReturnFormStatus newStatus)
		{
			this.ReturnOrderForm.Status = newStatus.ToString();
		}

		public ReturnFormBaseActivity()
		{
			Logger = LogManager.GetLogger(GetType());
			InitializeComponent();
		}
	}
}
