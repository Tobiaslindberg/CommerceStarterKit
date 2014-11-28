using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Commerce.Orders;

namespace EPiCode.Commerce.Workflow.Activities
{
    /// <summary>
    /// This activity records the usage of the promotions so this information can be used to inforce various customer and application based limits.
    /// </summary>
	public partial class RecordPromotionUsageActivity : CartActivityBase
	{
        public static DependencyProperty UsageStatusProperty = DependencyProperty.Register("UsageStatus", typeof(PromotionUsageStatus), typeof(RecordPromotionUsageActivity));
      
        /// <summary>
        /// Gets or sets the usage status.
        /// </summary>
        /// <value>The usage status.</value>
        [ValidationOption(ValidationOption.Required)]
        [Browsable(true)]
        public PromotionUsageStatus UsageStatus
        {
            get
            {
                return (PromotionUsageStatus)(base.GetValue(RecordPromotionUsageActivity.UsageStatusProperty));
            }
            set
            {
                base.SetValue(RecordPromotionUsageActivity.UsageStatusProperty, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordPromotionUsageActivity"/> class.
        /// </summary>
        public RecordPromotionUsageActivity()
			:base()
		{
			InitializeComponent();
        }

        /// <summary>
        /// Called by the workflow runtime to execute an activity.
        /// </summary>
        /// <param name="executionContext">The <see cref="T:System.Workflow.ComponentModel.ActivityExecutionContext"/> to associate with this <see cref="T:System.Workflow.ComponentModel.Activity"/> and execution.</param>
        /// <returns>
        /// The <see cref="T:System.Workflow.ComponentModel.ActivityExecutionStatus"/> of the run task, which determines whether the activity remains in the executing state, or transitions to the closed state.
        /// </returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            try
            {
                // Validate the properties at runtime
                this.ValidateRuntime();

                // Calculate order discounts
                this.RecordPromotions();

                // Retun the closed status indicating that this activity is complete.
                return ActivityExecutionStatus.Closed;
            }
            catch (Exception ex)
            {
                // An unhandled exception occured.  Throw it back to the WorkflowRuntime.
				Logger.Error(GetType().Name + ": " + ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Records the promotions.
        /// 
        /// Step 1: Load the existing usage that is related to the current order (if any).
        /// Step 2: Record/update the usage of lineitem, order and shipment level discounts.
        /// 
        /// The CustomerId can be taken from the Current Order.CustomerId.
        /// </summary>
        private void RecordPromotions()
        {
            List<Discount> discounts = new List<Discount>();

            OrderGroup group = this.OrderGroup;

            // if the order has been just added, skip recording the discounts
            if (group.ObjectState == Mediachase.MetaDataPlus.MetaObjectState.Added)
                return;

            PromotionUsageStatus status = this.UsageStatus;

            PromotionDto promotions = PromotionManager.GetPromotionDto();

            foreach (OrderForm form in group.OrderForms)
            {
                // Add order level discounts
                foreach (Discount discount in form.Discounts)
                {
                    discounts.Add(discount);
                }

                // Add lineitem discounts
                foreach (LineItem item in form.LineItems)
                {
                    foreach (Discount discount in item.Discounts)
                    {
                        discounts.Add(discount);
                    }
                }

                // Add shipping discounts
                foreach (Shipment shipment in form.Shipments)
                {
                    foreach (ShipmentDiscount discount in shipment.Discounts)
                    {
                        // Test for shipment rate calculation skip - this is not a real promotion.
                        if (discount.ShipmentId == shipment.ShipmentId && discount.DiscountName.Equals("@ShipmentSkipRateCalc"))
                        {
                            continue;
                        }
                        discounts.Add(discount);
                    }
                }
            }

            // Load existing usage Dto for the current order
            PromotionUsageDto usageDto = PromotionManager.GetPromotionUsageDto(0, Guid.Empty, group.OrderGroupId);

            // Clear all old items first
            if (usageDto.PromotionUsage.Count > 0)
            {
                foreach (PromotionUsageDto.PromotionUsageRow row in usageDto.PromotionUsage)
                {
                    row.Delete();
                }
            }

            // Now process the discounts
            foreach (Discount discount in discounts)
            {
                // we only record real discounts that exist in our database
                if (discount.DiscountId <= 0)
                    continue;

                PromotionUsageDto.PromotionUsageRow row = usageDto.PromotionUsage.NewPromotionUsageRow();
                row.CustomerId = group.CustomerId;
                row.LastUpdated = DateTime.UtcNow;
                row.OrderGroupId = group.OrderGroupId;
                row.PromotionId = discount.DiscountId;
                row.Status = status.GetHashCode();
                row.Version = 1; // for now version is always 1

                usageDto.PromotionUsage.AddPromotionUsageRow(row);
            }

            // Save the promotion usage
            PromotionManager.SavePromotionUsage(usageDto);
        }
      
	}
}
