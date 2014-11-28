/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.ComponentModel.DataAnnotations;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace OxxCommerceStarterKit.Core.Attributes.Validation
{
    public class ValidateEqualToAttribute : CompareAttribute
    {
        public ValidateEqualToAttribute(string otherProperty)
            : base(otherProperty)
        {   
        }

        public override string FormatErrorMessage(string name)
        {
            string requiredFormat =
                ServiceLocator.Current.GetInstance<LocalizationService>()
                    .GetString("/common/validation/compare");

            return string.Format(requiredFormat, name, OtherPropertyDisplayName ?? OtherProperty);
        }
    }
}
