/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Web.Security;
using log4net;
using OxxCommerceStarterKit.Core;

namespace OxxCommerceStarterKit.Web.ResetPassword
{
    public class ResetPasswordService : IResetPasswordService
    {
        private readonly IResetPasswordRepository _resetPasswordRespository;
        private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
       

        public ResetPasswordService(IResetPasswordRepository resetPasswordRespository)
        {
            _resetPasswordRespository = resetPasswordRespository;
            
        }

        public string GenerateResetHash(string emailAddress)
        {
            return _resetPasswordRespository.Add(emailAddress, GetResetTime());
        }

        public bool IsResetUrlValid(string hash, out MembershipUser user)
        {
            user = null;
            var isValid = false;
            if (!string.IsNullOrEmpty(hash))
            {
                ResetPasswordModel link = _resetPasswordRespository.Find(hash);

                isValid = (link != null && link.ExpireDate > DateTime.Now);
                if (isValid)
                {
                    user = Membership.Provider.GetUser(link.UserName, false);

                    if (user == null)
                    {
                        string userName = Membership.GetUserNameByEmail(link.UserName);
                        user = Membership.Provider.GetUser(userName, false);
                    }

                    if (user != null && user.IsLockedOut)
                    {
                        user.UnlockUser();
                    }
                }
            }

            return isValid;
        }

        /// <summary>
        /// Changes password and removes hash from DDS
        /// </summary>
        /// <param name="user"></param>
        /// <param name="newPassword"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public bool ChangePassword(MembershipUser user, string newPassword, string hash)
        {
            bool success = false;
            //change password
            if (user != null)
            {
                // reset password to retrieve current password
                string resetPw = user.ResetPassword();
                success = user.ChangePassword(resetPw, newPassword);
                // clean up - remove reset link from dds
                if (!string.IsNullOrEmpty(hash))
                    _resetPasswordRespository.Delete(hash);
            }

            return success;
        }

        public bool IsValidPassword(string newPassword)
        {
            if (newPassword.Length < Membership.MinRequiredPasswordLength)
            {
                return false;
            }

            int count = 0;

            for (int i = 0; i < newPassword.Length; i++)
            {
                if (!char.IsLetterOrDigit(newPassword, i))
                {
                    count++;
                }
            }

            if (count < Membership.MinRequiredNonAlphanumericCharacters)
            {
                return false;
            }

            if (Membership.PasswordStrengthRegularExpression.Length > 0)
            {
                if (!Regex.IsMatch(newPassword, Membership.PasswordStrengthRegularExpression))
                {
                    return false;
                }
            }

            return true;
        }

        public int RemoveExpiredTokens()
        {
            return _resetPasswordRespository.RemoveExpiredTokens();
        }

        private int GetResetTime()
        {
            int resetTime = 24; //default 24 hours
            if (WebConfigurationManager.AppSettings[Constants.AppSettings.ResetPasswordExpireKey] != null)
                resetTime = Int32.Parse(WebConfigurationManager.AppSettings[Constants.AppSettings.ResetPasswordExpireKey]);
            return resetTime;
        }

    }
}
