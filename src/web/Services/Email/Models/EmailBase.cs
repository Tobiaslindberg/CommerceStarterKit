/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using log4net;
using OxxCommerceStarterKit.Web.Models.ViewModels.Email;

namespace OxxCommerceStarterKit.Web.Services.Email.Models
{
	/// <summary>
	/// Uses Postal http://aboutcode.net/postal/
	///  and PreMailer.Net https://github.com/milkshakesoftware/PreMailer.Net
	///  from NuGet packages
	/// </summary>
	public abstract class EmailBase : Postal.Email, INotificationSettings
	{
		public string To { get; set; }
		public string From { get; set; }
		public string Subject { get; set; }
		public string Header { get; set; }
		public string Footer { get; set; }

		public static SendEmailResponse SendEmail(Postal.Email email)
		{
			var log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
			return SendEmail(email, log);
		}

		public static SendEmailResponse SendEmail(Postal.Email email, ILog log)
		{
			var output = new SendEmailResponse();

#if !DEBUG
			try
			{
#endif
				// Process email with Postal
				var emailService = ServiceLocator.Current.GetInstance<Postal.IEmailService>();
				using (var message = emailService.CreateMailMessage(email))
				{
					var htmlView = message.AlternateViews.FirstOrDefault(x => x.ContentType.MediaType.ToLower() == "text/html");
					if (htmlView != null)
					{
						string body = new StreamReader(htmlView.ContentStream).ReadToEnd();

						// move ink styles inline with PreMailer.Net
						var result = PreMailer.Net.PreMailer.MoveCssInline(body, false, "#ignore");

						htmlView.ContentStream.SetLength(0);
						var streamWriter = new StreamWriter(htmlView.ContentStream);

						streamWriter.Write(result.Html);
						streamWriter.Flush();

						htmlView.ContentStream.Position = 0;
					}

					// send email with default smtp client. (the same way as Postal)
					using (var smtpClient = new SmtpClient())
					{
						smtpClient.Send(message);
					}
				}
				output.Success = true;

#if !DEBUG
			}


			catch (Exception ex)
			{
				log.Error(ex);
				output.Exception = ex;
			}

#endif
			return output;
		}

		public class SendEmailResponse
		{
			public bool Success { get; set; }
			public Exception Exception { get; set; }
		}


		public static NotificationSettings GetNotificationSettings(string language = null)
		{
			var contentTypeRepository = ServiceLocator.Current.GetInstance<EPiServer.DataAbstraction.IContentTypeRepository>();
			var pageCriteriaQueryService = ServiceLocator.Current.GetInstance<IPageCriteriaQueryService>();
			var criterias = new PropertyCriteriaCollection();
			var criteria = new PropertyCriteria();
			criteria.Condition = EPiServer.Filters.CompareCondition.Equal;
			criteria.Name = "PageTypeID";
			criteria.Type = PropertyDataType.PageType;
			criteria.Value = contentTypeRepository.Load("NotificationSettings").ID.ToString();
			criteria.Required = true;
			criterias.Add(criteria);
            var page = language == null ? pageCriteriaQueryService.FindPagesWithCriteria(PageReference.StartPage, criterias).FirstOrDefault() : pageCriteriaQueryService.FindPagesWithCriteria(PageReference.StartPage, criterias, language).FirstOrDefault();
			if (page is NotificationSettings)
			{
				return (NotificationSettings)page;
			}
			return null;
		}
	}
}
