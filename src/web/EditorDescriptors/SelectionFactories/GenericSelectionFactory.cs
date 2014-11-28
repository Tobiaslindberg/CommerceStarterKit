/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using EPiServer.Shell.ObjectEditing;

namespace OxxCommerceStarterKit.Web.EditorDescriptors.SelectionFactories
{
    public abstract class GenericSelectionFactory : ISelectionFactory
    {
        public abstract IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata);

        protected ISelectItem[] GetSelectionFromArray(params string[] items)
        {
            List<ISelectItem> selectItems = new List<ISelectItem>();
            foreach (string item in items)
            {
                selectItems.Add(new SelectItem() { Text = item, Value = item });
            }
            return selectItems.ToArray();
        }
    }
}
