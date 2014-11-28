/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Reflection;
using log4net;
using Mediachase.Commerce.Orders;
using OxxCommerceStarterKit.Web.Services.Email.Models;

namespace OxxCommerceStarterKit.Web.Services.Email
{
	public class EmailService : IEmailService
	{
		private readonly ILog _log;

		public EmailService()
		{
			_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}


		public bool SendResetPasswordEmail(string email, string subject, string body, string passwordHash, string resetUrl)
		{
			var mailSettings = EmailBase.GetNotificationSettings();
			if (mailSettings != null)
			{
				var emailMessage = new Models.ResetPassword();
				emailMessage.From = mailSettings.From;
				emailMessage.To = email;
				emailMessage.Subject = subject;
				emailMessage.Header = mailSettings.MailHeader.ToString();
				emailMessage.Footer = mailSettings.MailFooter.ToString();
				emailMessage.Body = body;
				emailMessage.Token = passwordHash;
				emailMessage.ResetUrl = resetUrl;
				var result = EmailBase.SendEmail(emailMessage, _log);
				if (result.Success)
				{
					return true;
				}
				else
				{
					_log.Error(result.Exception);
					return false;
				}
			}
			_log.Error("Unable to get notification settings");
			return false;
		}

		public bool SendWelcomeEmail(string email, string subject, string body)
		{
			
			var mailSettings = EmailBase.GetNotificationSettings();
			if (mailSettings != null)
			{
				var emailMessage = new Welcome();
				emailMessage.From = mailSettings.From;
				emailMessage.To = email;
				emailMessage.Subject = subject;
				emailMessage.Header = mailSettings.MailHeader.ToString();
				emailMessage.Footer = mailSettings.MailFooter.ToString();
				emailMessage.Body = body;
				var result = EmailBase.SendEmail(emailMessage, _log);
				if (result.Success)
				{
					return true;
				}
				else
				{
					_log.Error(result.Exception);
					return false;
				}
			}
			_log.Error("Unable to get notification settings");
			return false;
		}

		public bool SendOrderReceipt(PurchaseOrder order)
		{
			var mailSettings = EmailBase.GetNotificationSettings();
			if (mailSettings != null)
			{
				var emailMessage = new Receipt(order);
				emailMessage.From = mailSettings.From;
				emailMessage.Header = mailSettings.MailHeader.ToString();
				emailMessage.Footer = mailSettings.MailFooter.ToString();

				var result = EmailBase.SendEmail(emailMessage);
				if (result.Success)
				{
					return true;
				}
				else
				{
					_log.Error(result.Exception);
					return false;
				}
			}
			_log.Error("Unable to get notification settings");
			return false;
		}

        public bool SendDeliveryReceipt(PurchaseOrder order, string language = null)
		{
			var mailSettings = EmailBase.GetNotificationSettings(language);
			if (mailSettings != null)
			{
				var emailMessage = new DeliveryReceipt(order);
				emailMessage.From = mailSettings.From;
				emailMessage.Header = mailSettings.MailHeader.ToString();
				emailMessage.Footer = mailSettings.MailFooter.ToString();

				var result = EmailBase.SendEmail(emailMessage);
				if (result.Success)
				{
					return true;
				}
				else
				{
					_log.Error(result.Exception);
					return false;
				}
			}
			_log.Error("Unable to get notification settings");
			return false;
		}
	}
}
