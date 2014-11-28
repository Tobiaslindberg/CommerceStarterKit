/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace OxxCommerceStarterKit.Web.EditorDescriptors
{
    [EditorDescriptorRegistration(TargetType = typeof(string), UIHint = "FloatingEditor")]
    public class FloatingEditorDescriptor : EditorDescriptor
    {
        public override void ModifyMetadata(
            ExtendedMetadata metadata,
            IEnumerable<Attribute> attributes)
        {
            metadata.CustomEditorSettings["uiWrapperType"] = EPiServer.Shell.UiWrapperType.Floating;
        }
    }
}
