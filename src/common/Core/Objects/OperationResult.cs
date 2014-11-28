/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;

namespace OxxCommerceStarterKit.Core.Objects
{
    public class OperationResult
    {
        public bool Success { get; set; }

        public List<string> Messages { get; set; }

        public OperationResult()
        {
            Success = true;
            Messages = new List<string>();
        }
    }

    public class OperationResult<T> : OperationResult
    {
        public OperationResult()
        {
            Content = new List<T>();
        }
        public List<T> Content { get; set; } 
    }
}
