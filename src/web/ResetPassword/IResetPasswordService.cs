/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Web.Security;

namespace OxxCommerceStarterKit.Web.ResetPassword
{
    public interface IResetPasswordService
    {
        bool ChangePassword(MembershipUser user, string newPassword, string hash);
        string GenerateResetHash(string emailAddress);
        bool IsResetUrlValid(string hash, out MembershipUser user);
        bool IsValidPassword(string password);
        int RemoveExpiredTokens();
    }
}
