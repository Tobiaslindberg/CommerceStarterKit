/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OxxCommerceStarterKit.Core.Attributes;

namespace OxxCommerceStarterKit.Web.Business.Initialization
{
    //http://weblogs.asp.net/seanmcalinden/archive/2010/06/11/custom-asp-net-mvc-2-modelmetadataprovider-for-using-custom-view-model-attributes.aspx
    /// <summary>
    /// TODO: Needs documentation about it's usage
    /// </summary>
    public class CustomModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata CreateMetadata(
            IEnumerable<Attribute> attributes,
            Type containerType,
            Func<object> modelAccessor,
            Type modelType,
            string propertyName)
        {
            var enumerable = attributes as Attribute[] ?? attributes.ToArray();
            var modelMetadata = base.CreateMetadata(enumerable, containerType, modelAccessor, modelType, propertyName);
            enumerable.OfType<MetadataAttribute>().ToList().ForEach(x => x.Process(modelMetadata));
            return modelMetadata;
        }
    }
}
