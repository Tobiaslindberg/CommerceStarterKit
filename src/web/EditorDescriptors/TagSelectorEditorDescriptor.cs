/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using OxxCommerceStarterKit.Web.EditorDescriptors.Attributes;

namespace OxxCommerceStarterKit.Web.EditorDescriptors
{
    [EditorDescriptorRegistration(TargetType = typeof(List<string>), UIHint = "TagSelector")]
   public class TagSelectorEditorDescriptor : EditorDescriptor
    {
        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            ClientEditingClass = "app.editors.TagSelector";
            TagSelectorApiAttribute apiAttribute = attributes.FirstOrDefault(x => x.GetType() == typeof(TagSelectorApiAttribute)) as TagSelectorApiAttribute;
            if(apiAttribute != null)
            {
                EditorConfiguration["apiUrl"] = apiAttribute.ApiUri;
            }
            base.ModifyMetadata(metadata, attributes);
        }
       
    }
}
