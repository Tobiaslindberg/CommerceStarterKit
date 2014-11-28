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
using OxxCommerceStarterKit.Core;

namespace OxxCommerceStarterKit.Web.EditorDescriptors
{
    [EditorDescriptorRegistration(TargetType = typeof (string), UIHint = Constants.UIHint.PaymentMethod)]
    public class PaymentMethodEditorDescriptor : EditorDescriptor
    {

        public override void ModifyMetadata(
            ExtendedMetadata metadata,
            IEnumerable<Attribute> attributes)
        {
            SelectionFactoryType = typeof (PaymentMethodSelectionFactory);

            ClientEditingClass =
                "epi.cms.contentediting.editors.SelectionEditor";

            base.ModifyMetadata(metadata, attributes);
        }
    }
}  
