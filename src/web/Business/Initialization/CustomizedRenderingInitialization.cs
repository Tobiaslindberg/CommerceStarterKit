/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using OxxCommerceStarterKit.Web.Business.Rendering;

namespace OxxCommerceStarterKit.Web.Business.Initialization
{
    /// <summary>
    /// Module for customizing templates and rendering.
    /// </summary>
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class CustomizedRenderingInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            //Add custom view engine allowing partials to be placed in additional locations
            //Note that we add it first in the list to optimize view resolving when using DisplayFor/PropertyFor
            ViewEngines.Engines.Insert(0, new SiteViewEngine());

            // Remove Visual Basic
            foreach (var engine in ViewEngines.Engines)
            {
                DisableVbhtml(engine as RazorViewEngine);
            }

            context.Locate.TemplateResolver()
                .TemplateResolved += TemplateCoordinator.OnTemplateResolved;
        }

        public void Uninitialize(InitializationEngine context)
        {
            ServiceLocator.Current.GetInstance<TemplateResolver>()
                .TemplateResolved -= TemplateCoordinator.OnTemplateResolved;
        }

        public void Preload(string[] parameters)
        {
        }

        public void DisableVbhtml(RazorViewEngine engine)
        {
            if (engine == null)
                return;

            engine.AreaViewLocationFormats = RemoveVbhtml(engine.AreaViewLocationFormats);
            engine.AreaMasterLocationFormats = RemoveVbhtml(engine.AreaMasterLocationFormats);
            engine.AreaPartialViewLocationFormats = RemoveVbhtml(engine.AreaPartialViewLocationFormats);
            engine.ViewLocationFormats = RemoveVbhtml(engine.ViewLocationFormats);
            engine.MasterLocationFormats = RemoveVbhtml(engine.MasterLocationFormats);
            engine.PartialViewLocationFormats = RemoveVbhtml(engine.PartialViewLocationFormats);
            engine.FileExtensions = RemoveVbhtml(engine.FileExtensions);
        }


        private string[] RemoveVbhtml(IEnumerable<string> source)
        {
            return source.Where(s => !s.Contains("vbhtml")).ToArray();
        }

    }
}
