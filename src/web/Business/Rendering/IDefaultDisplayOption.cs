/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

namespace OxxCommerceStarterKit.Web.Business.Rendering
{
    /// <summary>
    /// The default display option to use for a block when rendered in a content area. 
    /// If not implemented, the default tag is empty (or what is spesified by the content area itself.)
    /// </summary>
    /// <remarks>
    /// Implement this interface to change what will be the default display option for a
    /// block when used in a content area.
    /// </remarks>
    public interface IDefaultDisplayOption
    {
        string Tag { get; }
    }
}
