/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using BVNetwork.Bvn.FileNotFound.Logging;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Commerce.Routing;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Segments;
using log4net;
using Mediachase.BusinessFoundation.Data;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Events;
using OxxCommerceStarterKit.Core;
using OxxCommerceStarterKit.Core.Services;
using OxxCommerceStarterKit.Web.ResetPassword;
using Postal;
using EmailService = OxxCommerceStarterKit.Web.Services.Email.EmailService;
using IEmailService = OxxCommerceStarterKit.Web.Services.Email.IEmailService;

namespace OxxCommerceStarterKit.Web.Business.Initialization
{
	[ModuleDependency(typeof(EPiServer.Commerce.Initialization.InitializationModule))]
	public class CommerceInitialization : IInitializableModule, IConfigurableModule
	{
		private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public void Initialize(InitializationEngine context)
		{
			MapCatalogRoute(RouteTable.Routes);

			GlobalFilters.Filters.Add(new HandleErrorAttribute());
			GlobalFilters.Filters.Add(ServiceLocator.Current.GetInstance<PageContextActionFilter>());

			ModelMetadataProviders.Current = new CustomModelMetadataProvider();
			DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

			
			var connectionString = ConfigurationManager.ConnectionStrings["EcfSqlConnection"].ConnectionString;
			DataContext.Current = new DataContext(connectionString);


			var associationTypeRepository = context.Locate.Advanced.GetInstance<GroupDefinitionRepository<AssociationGroupDefinition>>();

			// Note! this had a bug in 8.0.0, fixed in 8.0.1 and later
			associationTypeRepository.Add(new AssociationGroupDefinition() { Name = Constants.AssociationTypes.SameStyle });
			associationTypeRepository.Add(new AssociationGroupDefinition() { Name = Constants.AssociationTypes.Default });
			associationTypeRepository.Delete(Constants.AssociationTypes.RecommendedProducts);
		}

		/// <summary>
		/// Configure default routing for EPiServer Commerce catalog content
		/// </summary>
		/// <remarks>
		/// TODO: If you want to remove the name of the catalog from the url, set the
		/// Catalog:RemoveCatalogFromUrl appSetting to true
		/// </remarks>
		/// <param name="routes"></param>
		private void MapCatalogRoute(RouteCollection routes)
		{
			string removeCatalogFromUrlSetting = System.Configuration.ConfigurationManager.AppSettings["Catalog:RemoveCatalogFromUrl"];
			bool removeCatalogFromUrl = false;
			bool.TryParse(removeCatalogFromUrlSetting, out removeCatalogFromUrl);
			if (removeCatalogFromUrl == false)
			{
				CatalogRouteHelper.MapDefaultHierarchialRouter(routes, false);

			}
			else
			{
				// This will pick the first catalog, and strip it from all urls (in and out)
				var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
				var referenceConverter = ServiceLocator.Current.GetInstance<ReferenceConverter>();
				var languageSelectionFactory = ServiceLocator.Current.GetInstance<LanguageSelectorFactory>();
				var routingSegmentLoader = ServiceLocator.Current.GetInstance<IRoutingSegmentLoader>();

				var firstCatalog =
					contentLoader.GetChildren<CatalogContent>(referenceConverter.GetRootLink()).FirstOrDefault();
				if (firstCatalog != null)
				{

					routes.RegisterPartialRouter(
						partialRouter: new HierarchicalCatalogPartialRouter(() => SiteDefinition.Current.StartPage,
							commerceRoot: firstCatalog,
							supportSeoUri: false,
							contentLoader: contentLoader,
							languageSelectorFactory: languageSelectionFactory,
							routingSegmentLoader: routingSegmentLoader));
				}
			}
		}

		public void Preload(string[] parameters)
		{
		}

		public void Uninitialize(InitializationEngine context)
		{
		}

		public void ConfigureContainer(ServiceConfigurationContext context)
		{
			// Important configuration. Determines the current market
			// TODO: Verify that you want to resolve the market from the language of the start page.
			//       You can also use the CurrentMarketProfile class to store the value in the session
			context.Container.Configure(c => c.For<ICurrentMarket>().Singleton().Use<CurrentMarketFromStartPage>());

			context.Container.Configure(c => c.For<IResetPasswordService>().Use<ResetPasswordService>());
			context.Container.Configure(c => c.For<IResetPasswordRepository>().Use<ResetPasswordRepository>());
			context.Container.Configure(c=> c.For<ICartService>().Use<CartService>());

			context.Container.Configure(c => c.For<IEmailService>().Use<EmailService>());

			// Postal
			context.Container.Configure(c => c.For<Postal.IEmailService>().Use<Postal.EmailService>());
			context.Container.Configure(c => c.For<IEmailViewRenderer>().Use<EmailViewRenderer>());
			context.Container.Configure(c => c.For<IEmailParser>().Use<EmailParser>());
		}
	}
}
