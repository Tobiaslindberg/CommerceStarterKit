/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Shell.ObjectEditing;
using Mediachase.Commerce.Orders.Managers;

namespace OxxCommerceStarterKit.Web.EditorDescriptors
{
    public class PaymentMethodSelectionFactory : ISelectionFactory
    {
        public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {

            var language = LanguageSelector.AutoDetect() as LanguageSelector;


            if (language != null)
            {
                var values = PaymentManager.GetPaymentMethods(language.LanguageBranch);
                foreach (var value in values.PaymentMethod)
                {
                    yield return new SelectItem
                    {
                        Text = string.Format("{0} ({1})", value.Name, language.LanguageBranch),
                        Value = value.PaymentMethodId
                    };
                }
            }
        }
    }
}
