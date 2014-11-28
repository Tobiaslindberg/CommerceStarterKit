/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;

namespace OxxCommerceStarterKit.Core.Repositories.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        IList<TEntity> GetAll();
        TEntity Get(int id);
        void Save(TEntity model);
        void Delete(int id);
    }
}
