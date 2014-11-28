/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using Mediachase.BusinessFoundation.Data.Business;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Inventory;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Security;
using OxxCommerceStarterKit.Core;
using OxxCommerceStarterKit.Core.Extensions;
using OxxCommerceStarterKit.Web.Models.Catalog;
using OxxCommerceStarterKit.Web.Services.Email;

namespace OxxCommerceStarterKit.Web.Controllers
{
	public class PaymentBaseController<T> : PageControllerBase<T> where T : PageData
    {
		protected void OnPaymentComplete(PurchaseOrder order)
		{
			// Create customer if anonymous
			CreateUpdateCustomer(order, User.Identity);
            
            var shipment = order.OrderForms.First().Shipments.First();

            OrderStatusManager.ReleaseOrderShipment(shipment);
            OrderStatusManager.PickForPackingOrderShipment(shipment);

            order.AcceptChanges();

			// Send Email receipt 
		    bool sendOrderReceiptResult = SendOrderReceipt(order);
            _log.Debug("Sending receipt e-mail - " + (sendOrderReceiptResult ? "success" : "failed"));

            try
            {
                // Not extremely important that this succeeds. 
                // Stocks are continually adjusted from ERP.
                AdjustStocks(order); 
            }
            catch (Exception e)
            {
                _log.Error("Error adjusting inventory after purchase.", e);
            }

		    ForwardOrderToErp(order);


		}

	    protected bool SendOrderReceipt(PurchaseOrder order)
		{
			var emailService = ServiceLocator.Current.GetInstance<IEmailService>();
			return emailService.SendOrderReceipt(order);
		}

	    protected void ForwardOrderToErp(PurchaseOrder purchaseOrder)
        {
            // TODO: Implement for your solution
        }

		public void CreateUpdateCustomer(PurchaseOrder order, System.Security.Principal.IIdentity identity)
		{
			// try catch so this does not interrupt the order process.
			try
			{
				var billingAddress = order.OrderAddresses.FirstOrDefault(x => x.Name == Constants.Order.BillingAddressName);
				var shippingAddress = order.OrderAddresses.FirstOrDefault(x => x.Name == Constants.Order.ShippingAddressName);

				// create customer if anonymous, or update join customer club and selected values on existing user
				MembershipUser user = null;
				if (!identity.IsAuthenticated)
				{
					string email = billingAddress.Email.Trim();

					user = Membership.GetUser(email);
					if (user == null)
					{
						var customer = CreateCustomer(email, Guid.NewGuid().ToString(),billingAddress.DaytimePhoneNumber,  billingAddress, shippingAddress, false, createStatus =>
						{
							_log.Error("Failed to create user during order completion. " + createStatus.ToString());
						});
						if (customer != null)
						{
							order.CustomerId = Guid.Parse(customer.PrimaryKeyId.Value.ToString());
							order.CustomerName = customer.FirstName + " " + customer.LastName;
							order.AcceptChanges();

							SetExtraCustomerProperties(order, customer);

                            RegisterPageController.SendWelcomeEmail(billingAddress.Email);
						}
					}
					else
					{
						var customer = CustomerContext.Current.GetContactForUser(user);
						order.CustomerName = customer.FirstName + " " + customer.LastName;
						order.CustomerId = Guid.Parse(customer.PrimaryKeyId.Value.ToString()); 
						order.AcceptChanges();
						SetExtraCustomerProperties(order, customer);

					}
				}
				else
				{
					user = Membership.GetUser(identity.Name);
					var customer = CustomerContext.Current.GetContactForUser(user);
					SetExtraCustomerProperties(order, customer);
				}
			}
			catch (Exception ex)
			{
				// Log here
				_log.Error("Error during creating / update user", ex);
			}
		}

		protected static CustomerContact CreateCustomer(string email, string password, string phone, OrderAddress billingAddress, OrderAddress shippingAddress, bool hasPassword, Action<MembershipCreateStatus> userCreationFailed)
		{
			MembershipCreateStatus createStatus;
			var user = Membership.CreateUser(email, password, email, null, null, true, out createStatus);
			switch (createStatus)
			{
				case MembershipCreateStatus.Success:

					SecurityContext.Current.AssignUserToGlobalRole(user, AppRoles.EveryoneRole);
					SecurityContext.Current.AssignUserToGlobalRole(user, AppRoles.RegisteredRole);

					var customer = CustomerContext.Current.GetContactForUser(user);
					customer.FirstName = billingAddress.FirstName;
					customer.LastName = billingAddress.LastName;
					customer.FullName = string.Format("{0} {1}", customer.FirstName, customer.LastName);
					customer.SetPhoneNumber(phone);
					customer.SetHasPassword(hasPassword);

					var customerBillingAddress = CustomerAddress.CreateForApplication(AppContext.Current.ApplicationId);
					OrderAddress.CopyOrderAddressToCustomerAddress(billingAddress, customerBillingAddress);
					customer.AddContactAddress(customerBillingAddress);
					customer.SaveChanges();
					customer.PreferredBillingAddressId = customerBillingAddress.AddressId;
					customerBillingAddress.Name = string.Format("{0}, {1} {2}", customerBillingAddress.Line1, customerBillingAddress.PostalCode, customerBillingAddress.City);
					CheckCountryCode(customerBillingAddress);
					BusinessManager.Update(customerBillingAddress);
					customer.SaveChanges();

					var customerShippingAddress = CustomerAddress.CreateForApplication(AppContext.Current.ApplicationId);
					OrderAddress.CopyOrderAddressToCustomerAddress(shippingAddress, customerShippingAddress);
					customer.AddContactAddress(customerShippingAddress);
					customer.SaveChanges();
					customer.PreferredShippingAddressId = customerShippingAddress.AddressId;
					customerShippingAddress.Name = string.Format("{0}, {1} {2}", customerShippingAddress.Line1, customerShippingAddress.PostalCode, customerShippingAddress.City);
					CheckCountryCode(customerShippingAddress);
					BusinessManager.Update(customerShippingAddress);
					customer.SaveChanges();

					return customer;
				default:
					userCreationFailed(createStatus);
					break;

			}
			return null;
		}


        /// <summary>
        /// If the customer has not selected a country, we pick the 
        /// first country for the current market.
        /// </summary>
        /// <param name="address">The address.</param>
		private static void CheckCountryCode(CustomerAddress address)
		{
			if (string.IsNullOrEmpty(address.CountryCode))
			{
				var currentMarket = ServiceLocator.Current.GetInstance<ICurrentMarket>();
				var market = currentMarket.GetCurrentMarket();
				if (market != null && market.Countries.Any())
				{
					address.CountryCode = market.Countries.FirstOrDefault();
				}
			}
		}


        /// <summary>
        /// If customer has joined the members club, then add the interest areas to the
        /// customer profile.
        /// </summary>
        /// <remarks>
        /// The request to join the member club is stored on the order during checkout.
        /// </remarks>
        /// <param name="order">The order.</param>
        /// <param name="customer">The customer.</param>
		private void SetExtraCustomerProperties(PurchaseOrder order, CustomerContact customer)
		{
            // TODO: Refactor for readability
			// member club
			if (order.OrderForms[0][Constants.Metadata.OrderForm.CustomerClub] != null && ((bool)order.OrderForms[0][Constants.Metadata.OrderForm.CustomerClub]) == true)
			{
				customer.CustomerGroup = Constants.CustomerGroup.CustomerClub;

				// categories
				if (!string.IsNullOrEmpty(order.OrderForms[0][Constants.Metadata.OrderForm.SelectedCategories] as string))
				{
					var s = (order.OrderForms[0][Constants.Metadata.OrderForm.SelectedCategories] as string).Split(',').Select(x =>
					{
						int i = 0;
						Int32.TryParse(x, out i);
						return i;
					}).Where(x => x > 0).ToArray();
					customer.SetCategories(s);
				}
				customer.SaveChanges();
			}
		}


	    protected void AdjustStocks(PurchaseOrder order)
		{
			var warehouseRepository = ServiceLocator.Current.GetInstance<IWarehouseRepository>();
			var warehousesCache = warehouseRepository.List();
			var warehouseInventory = ServiceLocator.Current.GetInstance<IWarehouseInventoryService>();

            var expirationCandidates = new HashSet<ProductContent>();

            var referenceConverter = ServiceLocator.Current.GetInstance<ReferenceConverter>();
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();

            // Adjust inventory
			foreach (OrderForm f in order.OrderForms)
			{
				foreach (LineItem i in f.LineItems)
				{
                    try
                    {
                        var warehouse = warehousesCache.Where(w => w.Code == i.WarehouseCode).First();
                        var catalogEntry = CatalogContext.Current.GetCatalogEntry(i.CatalogEntryId);
                        var catalogKey = new CatalogKey(catalogEntry);
                        var inventory = new WarehouseInventory(warehouseInventory.Get(catalogKey, warehouse));

                        //inventory.ReservedQuantity += i.Quantity; 
                        if ((inventory.InStockQuantity -= i.Quantity) <= 0)
                        {
                            var contentLink = referenceConverter.GetContentLink(i.CatalogEntryId);
                            var variant = contentRepository.Get<VariationContent>(contentLink);

                            expirationCandidates.Add((ProductContent)variant.GetParent());
                        }

                        warehouseInventory.Save(inventory);
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Unable to adjust inventory.", ex);
                    }

				}
			}

            // TODO: Determine if you want to unpublish products with no sellable variants
            // ExpireProductsWithNoInventory(expirationCandidates, contentRepository);
            // Alterntive approach is to notify the commerce admin about the products without inventory

		}

        /// <summary>
        /// Expires any products where no variants have any inventory. This effectively means there
        /// is nothing to sell for this product.
        /// </summary>
        /// <param name="expirationCandidates">The expiration candidates.</param>
        /// <param name="contentRepository">The content repository.</param>
	    protected static void ExpireProductsWithNoInventory(HashSet<ProductContent> expirationCandidates, IContentRepository contentRepository)
	    {
	        var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
	        var linksRepository = ServiceLocator.Current.GetInstance<ILinksRepository>();
	        var languageSelector = ServiceLocator.Current.GetInstance<ILanguageSelector>();
	        var warehouseInventoryService = ServiceLocator.Current.GetInstance<IWarehouseInventoryService>();

	        foreach (var p in expirationCandidates)
	        {
                // TODO: Perform quality check on this. Not well tested, and we swallow the exception
	            try
	            {
	                var variants =
	                    contentLoader.GetChildren<VariationContent>(p.ContentLink)
	                        .Concat(
	                            contentLoader.GetItems(p.GetVariantRelations(linksRepository).Select(x => x.Target),
	                                languageSelector).OfType<VariationContent>());

                    // If no variants for a product has inventory, expire the product
	                if (!variants.Any(v =>
	                {
	                    var catalogKey = new CatalogKey(AppContext.Current.ApplicationId, v.Code);

	                    return warehouseInventoryService.List(catalogKey).Any(inventory => inventory.InStockQuantity > 0);
	                }))
	                {
	                    var writableClone = (ProductContent) p.CreateWritableClone();

	                    writableClone.StopPublish = DateTime.Now;
	                    contentRepository.Save(writableClone, SaveAction.Publish, AccessLevel.NoAccess);
	                }
	            }
	            catch (Exception ex)
	            {
                    _log.Error("Cannot expire products with no inventory", ex);
	            }
	        }
	    }
    }
}
