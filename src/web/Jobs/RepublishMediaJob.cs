/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using EPiServer;
using EPiServer.BaseLibrary.Scheduling;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.PlugIn;
using EPiServer.ServiceLocation;

namespace OxxCommerceStarterKit.Web.Jobs
{
    [ScheduledPlugIn(DisplayName = "Republish Media")]
    public class RepublishMediaJob : JobBase
    {
        private bool _stopSignaled;
        private List<string> _error = new List<string>();

        public RepublishMediaJob()
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
            OnStatusChanged(String.Format("Starting execution of {0}", this.GetType()));

            IContentRepository repository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var root = repository.Get<IContent>(ContentReference.SiteBlockFolder);

            PublishChildren(repository, root);

            //For long running jobs periodically check if stop is signaled and if so stop execution
            if (_stopSignaled)
            {
                return "Stop of job was called";
            }

            return string.Format("{2}: Job completed : {0} errors, Detailed: {1}", _error.Count,string.Join("</br>",_error), Environment.MachineName);
        }

        private void PublishChildren(IContentRepository repository, IContent root)
        {

            var children = repository.GetChildren<IContent>(root.ContentLink);

            foreach (IContent content in children)
            {

                try
                {
                    var imageContent = content as ImageData;
                    if (imageContent != null)
                    {
                        var unpublished = imageContent.CreateWritableClone() as ImageData;
                        repository.Save(unpublished, SaveAction.Publish);
                    }
                }
                catch (Exception e)
                {
                    _error.Add(e.Message);
                }
				if (_stopSignaled)
				{
					break;
				}
                PublishChildren(repository, content);
            }
        }
    }
}
