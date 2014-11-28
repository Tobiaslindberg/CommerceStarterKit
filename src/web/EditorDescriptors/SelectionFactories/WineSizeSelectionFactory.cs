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
    public class WineSizeSelectionFactory : GenericSelectionFactory
    {
        public override IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            return GetSelectionFromArray("Piccolo (187.5ml)",
                "Half (375ml)",
                "Standard (750ml)",
                "Magnum (1.5L)",
                "Double Magnum (3L)",
                "Jeroboam (4.5L)",
                "Imperial (6.0L)",
                "Salmanazar (9.0L)",
                "Balthazar (12L)",
                "Nebuchadnezzar (15L)");
        }
    }
}
