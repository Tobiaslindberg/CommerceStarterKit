using System;
using System.Collections.Generic;
using System.Linq;
using System.Workflow.ComponentModel;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Inventory;
using Mediachase.Commerce.Orders;
using Mediachase.MetaDataPlus;

namespace EPiCode.Commerce.Workflow.Activities
{
	public partial class AdjustInstoreInventoryActivity : HandoffActivityBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="AdjustInstoreInventoryActivity"/> class.
        /// </summary>
        public AdjustInstoreInventoryActivity()
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
                // Check for multiple warehouses. In the default, we simply reject processing an order if the application has
                //  multiple warehouses. Any corresponding fulfillment process is the responsibility of the client.
                this.CheckMultiWarehouse();

                // Validate the properties at runtime
                this.ValidateRuntime();

				var orderForm = OrderGroup.OrderForms[0];
                var orderForms = OrderGroup.OrderForms.ToArray();
                IEnumerable<Shipment> shipments = orderForms.SelectMany(of => of.Shipments.ToArray());
				var orderLineItems = orderForm.LineItems.ToArray();
                IEnumerable<LineItem> lineItems = orderForms.SelectMany(x => x.LineItems.ToArray());
                foreach (Shipment shipment in shipments)
                {
                    foreach (var lineItem in orderLineItems)
                    {
                        // check if line item belongs to current shipment
                        if (Shipment.GetLineItemQuantity(shipment, lineItem.LineItemId) == 0)
                        {
                            continue;
                        }

                        AdjustStockItemQuantity(shipment, lineItem);
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

		private static void AdjustStockItemQuantity(Shipment shipment, LineItem lineItem)
		{
			if (lineItem.CatalogEntryId == "0" && 
				String.IsNullOrEmpty(lineItem.CatalogEntryId) &&
				lineItem.CatalogEntryId.StartsWith("@"))
			{
				return;
			}
			var entryDto = CatalogContext.Current.GetCatalogEntryDto(lineItem.CatalogEntryId, 
																	new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));

			var catalogEntry = GetCatalogEntry(entryDto);
			if (catalogEntry != null && CheckNeedEntryTracking(catalogEntry))
			{
                decimal delta = GetLineItemAdjustedQuantity(shipment, lineItem);
                IWarehouse warehouse = ServiceLocator.Current.GetInstance<IWarehouseRepository>().Get(shipment.WarehouseCode);
                IWarehouseInventory inventory = ServiceLocator.Current.GetInstance<IWarehouseInventoryService>()
                    .Get(new CatalogKey(catalogEntry), warehouse);
                if (inventory != null)
                {
                    AdjustStockInventoryQuantity(lineItem, inventory, delta);
                }
			}
		}

        /// <summary>
        /// Adjusts the stock inventory quantities. Method assumes only inventory TRACKING is enabled.
        /// </summary>
        /// <param name="lineItem">Line item object in cart.</param>
        /// <param name="inventory">Inventory associated with the line item's catalog entry.</param>
        /// <param name="delta">The change in inventory.</param>
        private static void AdjustStockInventoryQuantity(LineItem lineItem, IWarehouseInventory inventory, decimal delta)
		{
            if (inventory == null)
            {
                if (delta != 0)
                {
                    //throw new Exception("Inventory cannot be null with non-zero delta.");
                    return;
                }
                return;
            } 
            
            WarehouseInventory editedInventory = new WarehouseInventory(inventory);

			//arrival
			if (delta > 0)
			{
				// need distribute delta between InStock, Backorder, Preorder.
				if (lineItem.InStockQuantity > 0)
				{
					var backorderDelta = Math.Min(delta, lineItem.BackorderQuantity - inventory.BackorderQuantity);
					var preorderdelta = Math.Min(delta, lineItem.PreorderQuantity - inventory.PreorderQuantity);
					editedInventory.PreorderQuantity += preorderdelta;
					editedInventory.BackorderQuantity += backorderDelta;
					editedInventory.InStockQuantity += delta - backorderDelta - preorderdelta;
				} //need distribute delta between Preorder and Backorder
				else if (lineItem.InStockQuantity == 0)
				{
					if (lineItem.PreorderQuantity > 0)
					{
						editedInventory.PreorderQuantity += delta;
					}
					else if (lineItem.BackorderQuantity > 0)
					{
						editedInventory.BackorderQuantity += delta;
					}
				}
			}//consumption
			else
			{
				delta = Math.Abs(delta);
                bool inventoryEnabled = inventory.InventoryStatus == InventoryTrackingStatus.Enabled;
                if (inventory.InStockQuantity >= delta + inventory.ReservedQuantity) // Adjust the main inventory
                {
                    //just simply subtract from Instock quantity
                    editedInventory.InStockQuantity -= delta;
                }
                //Instock quantity is larger than delta but smaller than delta and reserved quantity
                else if (inventory.InStockQuantity >= inventory.ReservedQuantity)
                {
                    if (inventoryEnabled)
                    {
                        editedInventory.BackorderQuantity -= delta - (editedInventory.InStockQuantity - inventory.ReservedQuantity);
                        editedInventory.InStockQuantity = inventory.ReservedQuantity;
                    }
                    else
                    {
                        editedInventory.InStockQuantity -= delta;
                    }
                }   
				else if (inventory.InStockQuantity > 0) // there still exist items in stock
				{
					// Calculate difference between currently availbe and backorder
					var backorderDelta = delta - (inventory.InStockQuantity - (inventoryEnabled ? inventory.ReservedQuantity : 0));

					// Update inventory
                    if (inventoryEnabled)
                    {
                        editedInventory.InStockQuantity = inventory.ReservedQuantity;
                        editedInventory.BackorderQuantity -= backorderDelta; 
                    }
                    else
                    {
                        editedInventory.InStockQuantity -= delta;
                    }
				}
				else if (inventory.InStockQuantity <= 0)
				{
					// Update inventory
					editedInventory.InStockQuantity = -delta;

                    if (inventoryEnabled)
                    {
                        if (inventory.PreorderQuantity == 0)
                            editedInventory.BackorderQuantity -= delta;
                        else
                            editedInventory.PreorderQuantity -= delta; 
                    }
				}
			}

            ServiceLocator.Current.GetInstance<IWarehouseInventoryService>().Save(editedInventory);
		}

		private static decimal GetLineItemAdjustedQuantity(Shipment shipment, LineItem lineItem)
		{

			decimal retVal = 0m;
            decimal lineItemShipmentQuantity = shipment != null ? Shipment.GetLineItemQuantity(shipment, lineItem.LineItemId) : lineItem.Quantity;
			if (lineItem.Parent.Parent is Mediachase.Commerce.Orders.Cart)
			{
                retVal = -lineItemShipmentQuantity;
			}
			else if (lineItem.ObjectState == MetaObjectState.Added)
			{
                retVal = -lineItemShipmentQuantity;
			}
			else if (lineItem.ObjectState == MetaObjectState.Deleted)
			{
                retVal = lineItemShipmentQuantity;
			}
			else if (lineItem.ObjectState == MetaObjectState.Modified)
			{
				retVal = lineItem.OldQuantity - lineItem.Quantity;
			}
			
			return retVal;
		}

		private static bool CheckNeedEntryTracking(CatalogEntryDto.CatalogEntryRow catalogEntry)
		{
			if (catalogEntry == null)
			{
				throw new ArgumentNullException("catalogEntry");
			}
			var retVal = false;
			var entryVariations = GetEntryVariations(catalogEntry);
			var variation = entryVariations.FirstOrDefault();
			if (variation != null)
			{
				retVal = variation.TrackInventory;
			}
			return retVal;
		}

		private static CatalogEntryDto.CatalogEntryRow GetCatalogEntry(CatalogEntryDto entryDto)
		{
			if (entryDto == null)
			{
				throw new ArgumentNullException("entryDto");
			}
			return entryDto.CatalogEntry.FirstOrDefault();
		}

		private static IEnumerable<CatalogEntryDto.VariationRow> GetEntryVariations(CatalogEntryDto.CatalogEntryRow catalogEntry)
		{
			if (catalogEntry == null)
			{
				throw new ArgumentNullException("catalogEntry");
			}
			return catalogEntry.GetVariationRows();
		}
    }
}
