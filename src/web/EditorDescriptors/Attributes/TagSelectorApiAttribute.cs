/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OxxCommerceStarterKit.Web.EditorDescriptors.Attributes
{
    public class TagSelectorApiAttribute : Attribute
    {
        private readonly string _apiUri;

        public TagSelectorApiAttribute(string apiUri)
        {
            _apiUri = apiUri;
        }

        public string ApiUri
        {
            get { return _apiUri; }
        }
    }
}
