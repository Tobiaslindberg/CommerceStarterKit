/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;

namespace OxxCommerceStarterKit.Core.PaymentProviders.DIBS
{
    /// <summary>
    /// Code behind for EditTab.ascx
    /// </summary>
    public partial class EditTab : CoreBaseUserControl, IAdminContextControl, IAdminTabControl
    {
        #region IAdminContextControl Members

        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void LoadContext(System.Collections.IDictionary context)
        {

        }

        #endregion

        #region IAdminTabControl Members

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(System.Collections.IDictionary context)
        {
            
        }

        #endregion
    }
}
