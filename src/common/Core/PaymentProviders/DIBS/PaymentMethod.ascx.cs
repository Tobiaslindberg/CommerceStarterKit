/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Website;
using Mediachase.Commerce.Website.BaseControls;

namespace OxxCommerceStarterKit.Core.PaymentProviders.DIBS
{
    /// <summary>
    ///	Implements User interface for generic gateway
    /// </summary>
    public partial class PaymentMethod : BaseStoreUserControl, IPaymentOption
    {  
        
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, System.EventArgs e)
        {
            if (Request.Form["paymentprovider"] != null 
                && Request.Form["paymentprovider"].Equals(DIBSPayment.DIBSSystemName))
            {
                ErrorManager.GenerateError(CancelledMessage);
            }

            if (!IsPostBack)
            {
                PaymentMethodDto dibs = PaymentManager.GetPaymentMethodBySystemName(DIBSPayment.DIBSSystemName, SiteContext.Current.LanguageName);
                string processingUrl = DIBSPaymentGateway.GetParameterByName(dibs, DIBSPaymentGateway.ProcessingUrl).Value;
                string MD5K1 = DIBSPaymentGateway.GetParameterByName(dibs, DIBSPaymentGateway.MD5Key1).Value;
                string MD5K2 = DIBSPaymentGateway.GetParameterByName(dibs, DIBSPaymentGateway.MD5Key2).Value;

                if (string.IsNullOrEmpty(processingUrl) || string.IsNullOrEmpty(MD5K1) || string.IsNullOrEmpty(MD5K2))
                {
                    ConfigMessage.Text = LocalizationService.GetString("/Commerce/Checkout/DIBS/DIBSSettingsError");
                }
            }
        }

        #region IPaymentOption Members

        /// <summary>
        /// Validates input data for the control. In case of Credit card pre authentication will be the best way to
        /// validate. The error message if any should be displayed within a control itself.
        /// </summary>
        /// <returns>Returns false if validation is failed.</returns>
        public bool ValidateData()
        {
            return true;
        }

        /// <summary>
        /// This method is called before the order is completed. This method should check all the parameters
        /// and validate the credit card or other parameters accepted.
        /// </summary>
        /// <param name="form"></param>
        /// <returns>bool</returns>
        public Mediachase.Commerce.Orders.Payment PreProcess(OrderForm form)
        {
            OtherPayment otherPayment = new OtherPayment {TransactionType = TransactionType.Authorization.ToString()};
            return (Mediachase.Commerce.Orders.Payment)otherPayment;
        }

        /// <summary>
        /// This method is called after the order is placed. This method should be used by the gateways that want to
        /// redirect customer to their site.
        /// </summary>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public bool PostProcess(OrderForm form)
        {
            return true;
        }


        /// <summary>
        /// Gets the message when payment is cancelled or declined.
        /// </summary>
        /// <value>The message.</value>
        protected string CancelledMessage
        {
            get
            {
                return LocalizationService.GetString("/Commerce/Checkout/DIBS/Cancelled");
            }
        }
        #endregion

        private static LocalizationService LocalizationService
        {
            get { return ServiceLocator.Current.GetInstance<LocalizationService>(); }
        }
    }
}
