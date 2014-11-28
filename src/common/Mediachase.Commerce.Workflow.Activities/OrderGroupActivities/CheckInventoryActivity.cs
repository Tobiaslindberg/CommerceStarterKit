using System;
using System.Collections.Generic;
using System.Linq;
using System.Workflow.ComponentModel;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Inventory;
using Mediachase.Commerce.Orders;
using System.Diagnostics;

namespace EPiCode.Commerce.Workflow.Activities
{
	public partial class CheckInventoryActivity : OrderGroupActivityBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CheckInventoryActivity"/> class.
		/// </summary>
		public CheckInventoryActivity()
			: base()
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
				this.ValidateItems();

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

		private void ValidateItems()
		{
			//We don't need to validate quantity in the wishlist
			var orderForms = OrderGroup.OrderForms.ToArray();
			var lineItems = orderForms.SelectMany(x => x.LineItems.ToArray());
			var validLineItems = lineItems.Where(x => x.CatalogEntryId != "0" && !String.IsNullOrEmpty(x.CatalogEntryId) && !x.CatalogEntryId.StartsWith("@"));
			foreach (LineItem lineItem in validLineItems)
			{
				List<string> changeQtyReason = new List<string>();
                
                decimal newQty;
                if (lineItem.IsInventoryAllocated)
                {
                    newQty = lineItem.Quantity;
                }
                else
                {
                    newQty = GetNewLineItemQty(lineItem, changeQtyReason);
                }

				var changeQtyReasonDisplay = String.Join(" and ", changeQtyReason.ToArray());
				if (newQty == 0)
				{
					// Remove item if it reached this stage
                    //Debugger.Break();
                    Warnings.Add("LineItemRemoved-" + lineItem.LineItemId.ToString(), String.Format("Item \"{0}\" has been removed from the cart because it is no longer available or there is not enough quantity.", lineItem.DisplayName));
					DeleteLineItemFromShipments(lineItem);
					// Delete item
					lineItem.Delete();
				}
				else
				{
					var delta = lineItem.Quantity - newQty;
					if (delta != 0)
					{
						lineItem.Quantity -= delta;
						ChangeShipmentsLineItemQty(lineItem, delta);
                        Warnings.Add("LineItemQtyChanged-" + lineItem.LineItemId.ToString(),
									 String.Format("Item \"{0}\" quantity has been changed {1}", lineItem.DisplayName, changeQtyReasonDisplay));
					}
				}
			}

		}

		private decimal GetNewLineItemQty(LineItem lineItem, List<string> changeQtyReason)
		{
			var newLineItemQty = lineItem.Quantity;

			if (newLineItemQty < lineItem.MinQuantity || newLineItemQty > lineItem.MaxQuantity)
			{
				newLineItemQty = Math.Max(lineItem.MinQuantity, newLineItemQty);
				if (newLineItemQty != lineItem.Quantity)
				{
					changeQtyReason.Add("by Min Quantity setting");
				}
				newLineItemQty = Math.Min(lineItem.MaxQuantity, newLineItemQty);
				if (newLineItemQty != lineItem.Quantity)
				{
					changeQtyReason.Add("by Max Quantity setting");
				}
			}

			if (lineItem.InventoryStatus == (int)InventoryTrackingStatus.Enabled)
			{
                IWarehouse warehouse = ServiceLocator.Current.GetInstance<IWarehouseRepository>().Get(lineItem.WarehouseCode);
                IWarehouseInventory inventory = null;
                CatalogKey catalogKey = new CatalogKey(lineItem.Parent.Parent.ApplicationId, lineItem.CatalogEntryId);
                if (warehouse != null)
                {
                    if (!warehouse.IsActive)
                    {
                        return 0;
                    }
                    
                    inventory = ServiceLocator.Current.GetInstance<IWarehouseInventoryService>().Get(
                        new CatalogKey(lineItem.Parent.Parent.ApplicationId, lineItem.CatalogEntryId), warehouse);
                    
                }
                
                // if no inventory, return 0;
                if (inventory == null)
                {
                    return 0;
                }

                decimal availableQuantity = inventory.InStockQuantity - inventory.ReservedQuantity;
                // item exists with appropriate quantity
				if (availableQuantity < newLineItemQty)
				{
                    if (availableQuantity > 0) // there still exist items in stock
					{
						// check if we can backorder some items
                        if ((inventory.AllowBackorder | inventory.AllowPreorder))
						{
							//Increase stock qty by backorder qty if 
							var availStockAndBackorderQty = availableQuantity + inventory.BackorderQuantity;
							if (availStockAndBackorderQty >= newLineItemQty)
							{
								//NONE
							}
							else
							{
								newLineItemQty = availStockAndBackorderQty;
								changeQtyReason.Add("by BackOrder quantity");
							}
						}
						else
						{
                            newLineItemQty = availableQuantity;
						}
					}
					else
					{
                        if ((inventory.AllowBackorder | inventory.AllowPreorder) && inventory.PreorderQuantity > 0)
						{
							if (inventory.PreorderQuantity >= newLineItemQty)
							{
								//NONE
							}
							else
							{
								newLineItemQty = inventory.PreorderQuantity;
								changeQtyReason.Add("by Preorder quantity");
							}
						}
                        else if ((inventory.AllowBackorder | inventory.AllowPreorder) && inventory.BackorderQuantity > 0)
						{
							if (inventory.BackorderQuantity >= newLineItemQty)
							{
							}
							else
							{
								newLineItemQty = inventory.BackorderQuantity;
								changeQtyReason.Add("by BackOrder quantity");
							}
						}
						else
						{
							newLineItemQty = 0;
						}
					}
				}
			}
			return newLineItemQty;
		}

		private void DeleteLineItemFromShipments(LineItem lineItem)
		{
			var orderForm = OrderGroup.OrderForms.ToArray().FirstOrDefault();
			if (orderForm != null)
			{
				var lineItemIndex = orderForm.LineItems.IndexOf(lineItem);
				var allShipmentContainsLineItem = orderForm.Shipments.ToArray().Where(x => Shipment.GetShipmentLineItems(x).Contains(lineItem));
				foreach (var shipment in allShipmentContainsLineItem)
				{
					shipment.RemoveLineItemIndex(lineItemIndex);
				}
			}
		}
		private void ChangeShipmentsLineItemQty(LineItem lineItem, decimal delta)
		{
			var orderForm = OrderGroup.OrderForms.ToArray().FirstOrDefault();
			if (orderForm != null)
			{
				var lineItemIndex = orderForm.LineItems.IndexOf(lineItem);
				var allShipmentContainsLineItem = orderForm.Shipments.ToArray().Where(x => Shipment.GetShipmentLineItems(x).Contains(lineItem));
				foreach (var shipment in allShipmentContainsLineItem)
				{
					//Decrease qty in all shipment contains line item
					var shipmentQty = Shipment.GetLineItemQuantity(shipment, lineItem.LineItemId);
					var newShipmentQty = shipmentQty - delta;
					newShipmentQty = newShipmentQty > 0 ? newShipmentQty : 0;
					//Set new line item qty in shipment
					shipment.SetLineItemQuantity(lineItemIndex, newShipmentQty);
					delta -= Math.Min(delta, shipmentQty);

					if (delta == 0)
						break;
				}
			}
		}
	}
}
