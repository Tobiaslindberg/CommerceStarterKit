/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using Mediachase.Commerce.Orders;

namespace OxxCommerceStarterKit.Web.Services.Email
{
	public interface IEmailService
	{
		bool SendResetPasswordEmail(string email, string subject, string body, string passwordHash, string resetUrl);
		bool SendWelcomeEmail(string email, string subject, string body);
		bool SendOrderReceipt(PurchaseOrder order);
		bool SendDeliveryReceipt(PurchaseOrder order, string language = null);
	}
}
