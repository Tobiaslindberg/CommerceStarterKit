/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using EPiServer.Commerce.Catalog.ContentTypes;

namespace OxxCommerceStarterKit.Web.Models.ViewModels
{
    public class LazyVariationViewModelCollection : Lazy<IEnumerable<IVariationViewModel<VariationContent>>>
    {
        public LazyVariationViewModelCollection(Func<IEnumerable<IVariationViewModel<VariationContent>>> valueFactory)
            : base(valueFactory)
        {
        }
    }
}
