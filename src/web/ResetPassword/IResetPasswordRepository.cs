/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

namespace OxxCommerceStarterKit.Web.ResetPassword
{
    public interface IResetPasswordRepository
    {
        string Add(string username, int expireTimeInHours);
        void Delete(string hash);
        ResetPasswordModel Find(string hash);
        int RemoveExpiredTokens();
    }
}
