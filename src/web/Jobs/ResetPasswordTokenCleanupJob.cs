/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using EPiServer.BaseLibrary.Scheduling;
using EPiServer.PlugIn;
using EPiServer.ServiceLocation;
using OxxCommerceStarterKit.Web.ResetPassword;

namespace OxxCommerceStarterKit.Web.Jobs
{
    [ScheduledPlugIn(DisplayName = "Cleanup Password Tokens")]
    public class ResetPasswordTokenCleanupJob : JobBase
    {
        private bool _stopSignaled;

        public ResetPasswordTokenCleanupJob()
        {
            IsStoppable = true;
        }

        /// <summary>
        /// Called when a user clicks on Stop for a manually started job, or when ASP.NET shuts down.
        /// </summary>
        public override void Stop()
        {
            _stopSignaled = true;
        }

        /// <summary>
        /// Called when a scheduled job executes
        /// </summary>
        /// <returns>A status message to be stored in the database log and visible from admin mode</returns>
        public override string Execute()
        {
            //Call OnStatusChanged to periodically notify progress of job for manually started jobs
            OnStatusChanged(String.Format("Starting execution."));

            //Add implementation
            var resetPasswordService = ServiceLocator.Current.GetInstance<IResetPasswordService>();

            var removeExpiredTokensCount = resetPasswordService.RemoveExpiredTokens();

            //For long running jobs periodically check if stop is signaled and if so stop execution
            if (_stopSignaled)
            {
                return "Stop of job was called";
            }

            return string.Format("Removed {0} tokens",removeExpiredTokensCount);
        }
    }
}
