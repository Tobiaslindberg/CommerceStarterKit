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
using EPiServer.Editor.TinyMCE;

namespace OxxCommerceStarterKit.Web.EditorDescriptors.TinyMcePlugins
{
    [TinyMCEPluginNonVisual(PlugInName = "extendWithIframeAndObject",
        ServerSideOnly = true,
        AlwaysEnabled = true,
        EditorInitConfigurationOptions = "{ extended_valid_elements: 'iframe[src|height|width]', media_strict: false }")]
    public class AllowExtendedAttributes
    {
    }
}
