/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Website;

namespace OxxCommerceStarterKit.Core.PaymentProviders.DIBS
{
    /// <summary>
    ///	Implements User interface for generic gateway
    /// </summary>
    public class PaymentMethod : IPaymentOption
    {
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
            OtherPayment otherPayment = new OtherPayment { TransactionType = TransactionType.Authorization.ToString() };
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
