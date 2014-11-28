/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

namespace OxxCommerceStarterKit.Web.Jobs
{
    public interface ISetStatus
    {
        void SetStatus(string status);
        bool StopSignaled();
    }
}
