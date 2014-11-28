/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using EPiServer.Core;
using EPiServer.GoogleAnalytics.Models;
using EPiServer.GoogleAnalytics.Web.Tracking;

namespace OxxCommerceStarterKit.Web.Business.Analytics
{
    public class UniversalSyntaxEx : UniversalSyntax
    {
        protected string _gaScript =
            "(function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){\r\n(i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),\r\nm=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)\r\n})(window,document,'script','//www.google-analytics.com/analytics.js','ga');";
        
        
        public override string BuildTrackingScript(ScriptBuilderContext appenderContext, SiteTrackerSettings siteSettings,
            out bool requiresScriptReference)
        {
            requiresScriptReference = false;
            if (siteSettings == null)
            {
                return null;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("/* Begin GA Script */");
            stringBuilder.AppendLine(_gaScript);

            stringBuilder.AppendLine(string.Format("ga('create', '{0}', 'auto');", siteSettings.TrackingId));

            stringBuilder.AppendLine("// Extended Tracking");
            stringBuilder.AppendLine(AppendExtendedTracking(siteSettings, ref requiresScriptReference));

            stringBuilder.AppendLine("// Plugin Script");
            stringBuilder.AppendLine(this.GetPluginScript());

            if (siteSettings.TrackAuthors && !string.IsNullOrEmpty(appenderContext.Author))
            {
                stringBuilder.AppendLine("// Custom Author Tracking");
                stringBuilder.AppendLine(this.GetCustomDimension("Author", CustomVariables.AuthorVariable, appenderContext.Author));
            }
            
            ContentReference contentReference = new ContentReference(appenderContext.PageId);
            
            ICollection<AnalyticsInteraction> interactions =  EPiServer.GoogleAnalytics.Helpers.Extensions.GetInteractions(appenderContext.InteractionStore);

            // This is where the interesting stuff happens
            // All custom interactions are added here
            stringBuilder.AppendLine("// Begin Interactions");
            foreach (AnalyticsInteraction interaction in interactions)
            {
                // Skip any interactions that are tied to a specific page
                if (ContentReference.IsNullOrEmpty(interaction.ContentLink) == false && 
                    contentReference.Equals(interaction.ContentLink) == false)
                {
                    continue;
                }
                stringBuilder.AppendLine(interaction.GetUAScript());
            }
            stringBuilder.AppendLine("// End Interactions");
            
            // Clear any interactions that should not persist
            // across a request
            this.ClearRedundantInteractions(interactions);

            stringBuilder.AppendLine("ga('send', 'pageview');");
            stringBuilder.AppendLine("/* End GA Script */");
            return stringBuilder.ToString();
        }

        protected string AppendExtendedTracking(SiteTrackerSettings siteSettings, ref bool requiresScriptReference)
        {
            Dictionary<string, object> extendedTracking = this.GetExtendedTracking(siteSettings);
            requiresScriptReference = extendedTracking.Count > 0;
            return this.SerializeTrackerSettings(extendedTracking, siteSettings.TrackingScriptOption.ToString());
        }

        private string GetCustomDimension(string dimensionName, string dimensionIndex, string value)
        {
            return string.Format("ga('set', '{0}{1}', '{2}');", dimensionName, dimensionIndex, value);
        }
    }
}
