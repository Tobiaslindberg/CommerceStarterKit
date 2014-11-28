/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Linq;
using EPiServer.Data.Dynamic;

namespace OxxCommerceStarterKit.Web.ResetPassword
{
    public class ResetPasswordRepository : IResetPasswordRepository
    {
        private static DynamicDataStore Store
        {
            get
            {                
                return typeof(ResetPasswordModel).GetStore();
            }
        }

        public string Add(string username, int expireTimeInHours)
        {
            ResetPasswordModel resetLink = null;
            // check to see if reset link already exist
            var query = Store.LoadAll<ResetPasswordModel>().
               Where(x => x.UserName == username).FirstOrDefault();

            var expireDateTime = DateTime.Now.AddHours(expireTimeInHours);

            if (query != null)
                resetLink = query;
            else
                resetLink = new ResetPasswordModel()
                {
                    UserName = username,
                    Hash = Guid.NewGuid().ToString()
                };

            // make sure we update expire date time
            resetLink.ExpireDate = expireDateTime;
            Store.Save(resetLink);

            return resetLink.Hash;
        }

        public ResetPasswordModel Find(string hash)
        {
            return Store.LoadAll<ResetPasswordModel>().FirstOrDefault(x => x.Hash == hash);
        }

        public int RemoveExpiredTokens()
        {
            var expiredTokens = Store.LoadAll<ResetPasswordModel>().Where(m => m.ExpireDate < DateTime.Now).ToList();
            var numberOfTokens = expiredTokens.Count();
            foreach (var token in expiredTokens)
            {
                Store.Delete(token.Id);
            }
            return numberOfTokens;
        }

        public void Delete(string hash)
        {
            ResetPasswordModel link = Store.LoadAll<ResetPasswordModel>().
               Where(x => x.Hash == hash).FirstOrDefault();
            if (link != null)
                Store.Delete(link);
        }
    }
}
