using System;
using System.Collections.Generic;
using System.Linq;
using System.Workflow.ComponentModel;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Inventory;
using Mediachase.Commerce.Orders;

namespace EPiCode.Commerce.Workflow.Activities
{
    public partial class CheckInstoreInventoryActivity : HandoffActivityBase
	{
        private List<Shipment> _PickupShipments;

		/// <summary>
		/// Initializes a new instance of the <see cref="CheckInstoreInventoryActivity"/> class.
		/// </summary>
		public CheckInstoreInventoryActivity()
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
            var orderForms = OrderGroup.OrderForms.ToArray();
            IEnumerable<Shipment> shipments = orderForms.SelectMany(of => of.Shipments.ToArray());
            // select shipments whose shipping address is a pickup warehouse. This should be determined ahead of time.
            _PickupShipments = orderForms.SelectMany(x => x.Shipments.ToArray())
                .Where(s => PickupWarehouseInShipment.Keys.Contains(s.ShipmentId)).ToList();
            // Select only the line items that belong to the pickup shipments.
            //_PickupLineItemIndices = _PickupShipments.SelectMany(s => s.LineItemIndexes).ToArray();
            
            IEnumerable<LineItem> lineItems = orderForms.SelectMany(x => x.LineItems.ToArray());
            
            //List<LineItem> pickupLineItems = new List<LineItem>();
            //foreach (string lineItemIndex in _PickupLineItemIndices)
            //{
            //    int indexInt = int.Parse(lineItemIndex);
            //    if (indexInt < lineItems.Count())
            //    {
            //        pickupLineItems.Add(lineItems.ElementAt(indexInt));
            //    }
            //}
			//We don't need to validate quantity in the wishlist
            IEnumerable<LineItem> validLineItems = lineItems.Where(x => x.CatalogEntryId != "0" && !String.IsNullOrEmpty(x.CatalogEntryId) && !x.CatalogEntryId.StartsWith("@"));

            List<Shipment> shipmentsToDelete = new List<Shipment>();
            List<LineItem> lineItemsToDelete = new List<LineItem>();

            foreach (Shipment shipment in shipments)
            {
                foreach (LineItem lineItem in validLineItems)
                {
                    // check if line item belongs to current shipment
                    if (Shipment.GetLineItemQuantity(shipment, lineItem.LineItemId) == 0)
                    {
                        continue;
                    }

                    List<string> changeQtyReason = new List<string>();
                    
                    decimal newQty;
                    if (lineItem.IsInventoryAllocated)
                    {
                        newQty = lineItem.Quantity;
                    }
                    else
                    {
                        newQty = GetNewLineItemQty(shipment, lineItem, changeQtyReason);
                    }

                    var changeQtyReasonDisplay = String.Join(" and ", changeQtyReason.ToArray());
                    if (newQty == 0)
                    {
                        // Remove item if it reached this stage
                        Warnings.Add("Shipment-" + shipment.Id.ToString() + "-LineItemRemoved-" + lineItem.Id.ToString(), 
                            String.Format("Item \"{0}\" has been removed from the cart because it is no longer available.", lineItem.DisplayName));
                        DeleteLineItemFromShipment(shipment, lineItem);
                        // Delete line item, but only if it belongs to no shipments
                        if (!orderForms.SelectMany(of => of.Shipments.ToArray()).Where(s => Shipment.GetLineItemQuantity(s, lineItem.LineItemId) > 0).Any())
                        {
                            lineItemsToDelete.Add(lineItem);
                        }
                    }
                    else
                    {
                        var delta = Shipment.GetLineItemQuantity(shipment, lineItem.LineItemId) - newQty;
                        if (delta != 0)
                        {
                            lineItem.Quantity -= delta;
                            ChangeShipmentLineItemQty(shipment, lineItem, delta);
                            Warnings.Add("Shipment-" + shipment.Id.ToString() + "-LineItemQtyChanged-" + lineItem.Id.ToString(),
                                         String.Format("Item \"{0}\" quantity has been changed {1}.", lineItem.DisplayName, changeQtyReasonDisplay));
                        }
                    }
                }

                if (shipment.LineItemIndexes.Count() == 0)
                {
                    shipmentsToDelete.Add(shipment);
                }
            }

            foreach (Shipment shipment in shipmentsToDelete)
            {
                shipment.Delete();
            }

            foreach (LineItem lineItem in lineItemsToDelete)
            {
                lineItem.Delete();
            }
		}

		private decimal GetNewLineItemQty(Shipment shipment, LineItem lineItem, List<string> changeQtyReason)
		{
            var newLineItemQty = Shipment.GetLineItemQuantity(shipment, lineItem.LineItemId);

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
                IWarehouse warehouse = ServiceLocator.Current.GetInstance<IWarehouseRepository>().Get(shipment.WarehouseCode);
                IWarehouseInventory inventory = null;
                CatalogKey catalogKey = new CatalogKey(lineItem.Parent.Parent.ApplicationId, lineItem.CatalogEntryId);
                if (warehouse != null)
                {
                    inventory = ServiceLocator.Current.GetInstance<IWarehouseInventoryService>().Get(
                        new CatalogKey(lineItem.Parent.Parent.ApplicationId, lineItem.CatalogEntryId), warehouse);
                }
                // if no inventory, return 0;
                if (inventory == null)
                {
                    return 0;
                }

				// item exists with appropriate quantity
                if (inventory.InStockQuantity < newLineItemQty)
				{
                    if (inventory.InStockQuantity > 0) // there still exist items in stock
					{
						// check if we can backorder some items
                        if (inventory.AllowBackorder | inventory.AllowPreorder)
						{
							//Increase stock qty by backorder qty if 
                            var availStockAndBackorderQty = inventory.InStockQuantity + inventory.BackorderQuantity;
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
                            newLineItemQty = inventory.InStockQuantity;
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

        private void DeleteLineItemFromShipment(Shipment shipment, LineItem lineItem)
		{
			var orderForm = OrderGroup.OrderForms.ToArray().Where(of => of.LineItems.ToArray().Contains(lineItem)).FirstOrDefault();
            if (orderForm != null)
            {
                var lineItemIndex = orderForm.LineItems.IndexOf(lineItem);
                decimal shipmentQty = Shipment.GetLineItemQuantity(shipment, lineItem.LineItemId);
                lineItem.Quantity -= shipmentQty;
                shipment.RemoveLineItemIndex(lineItemIndex);
            }
		}

		private void ChangeShipmentLineItemQty(Shipment shipment, LineItem lineItem, decimal delta)
		{
            var orderForm = OrderGroup.OrderForms.ToArray().Where(of => of.LineItems.ToArray().Contains(lineItem)).FirstOrDefault();
            if (orderForm != null)
			{
				var lineItemIndex = orderForm.LineItems.IndexOf(lineItem);
				{
					//Decrease qty in all shipment contains line item
					var shipmentQty = Shipment.GetLineItemQuantity(shipment, lineItem.LineItemId);
					var newShipmentQty = shipmentQty - delta;
					newShipmentQty = newShipmentQty > 0 ? newShipmentQty : 0;
					//Set new line item qty in shipment
					shipment.SetLineItemQuantity(lineItemIndex, newShipmentQty);
					delta -= Math.Min(delta, shipmentQty);
				}
			}
		}
	}
}
