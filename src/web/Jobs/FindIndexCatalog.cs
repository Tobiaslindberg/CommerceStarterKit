/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using EPiServer;
using EPiServer.BaseLibrary.Scheduling;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Framework;
using EPiServer.Framework.Localization;
using EPiServer.PlugIn;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Markets;
using Mediachase.Commerce.Pricing;
using OxxCommerceStarterKit.Web.Api;
using OxxCommerceStarterKit.Web.Helpers;
using OxxCommerceStarterKit.Web.Models.Blocks.Contracts;
using OxxCommerceStarterKit.Web.Models.Catalog;
using OxxCommerceStarterKit.Web.Models.FindModels;

namespace OxxCommerceStarterKit.Web.Jobs
{
	[ScheduledPlugIn(DisplayName = "Index Product Catalog")]
	public class FindIndexCatalog : JobBase
	{
		class IndexInformation
		{
			public IndexInformation()
			{
				MachineName = Environment.MachineName;

			}
			public int NumberOfProductsIndexed { get; set; }
			public long Duration { get; set; }
			public string MachineName { get; set; }
			public int NumberOfProductsRemoved { get; set; }
			public int NumberOfProductsFound { get; set; }
			public int NumberOfProductsInIndex { get; set; }
			public int NumberOfProductsFoundAfterExpiredFilter { get; set; }

			public override string ToString()
			{
				return string.Format("Found {6}/{4}, indexed {0} and removed {3}/{5} products in {1}ms on {2}", NumberOfProductsIndexed, Duration,
					MachineName, NumberOfProductsRemoved, NumberOfProductsFound, NumberOfProductsInIndex, NumberOfProductsFoundAfterExpiredFilter);
			}
		}


		private bool _stopSignaled;
		readonly ReferenceConverter referenceConverter = ServiceLocator.Current.GetInstance<ReferenceConverter>();
		readonly IContentLoader contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();

		public FindIndexCatalog()
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
		/// Starts the job
		/// </summary>
		/// <returns>A status message that will be logged</returns>
		public override string Execute()
		{


			IndexInformation info = new IndexInformation();
			Stopwatch tmr = Stopwatch.StartNew();

			IClient client = SearchClient.Instance;

		    
			//Delete all
			client.Delete<FindProduct>(x => x.MatchType(typeof(FindProduct)));
            
            var findHelper = ServiceLocator.Current.GetInstance<FindHelper>();
			var language = LanguageSelector.MasterLanguage();
			var localizationService = ServiceLocator.Current.GetInstance<LocalizationService>();
			var marketService = ServiceLocator.Current.GetInstance<IMarketService>();
			var allMarkets = marketService.GetAllMarkets();
			var priceService = ServiceLocator.Current.GetInstance<IPriceService>();
			var linksRepository = ServiceLocator.Current.GetInstance<ILinksRepository>();

		    

		    IEnumerable<ContentReference> contentLinks = contentLoader.GetDescendents(Root);
            
			

			int bulkSize = 100;
			foreach (CultureInfo availableLocalization in localizationService.AvailableLocalizations)
			{
				var market = allMarkets.FirstOrDefault(m => m.DefaultLanguage.Equals(availableLocalization));
				if (market == null)
				{
					continue;
				}
				string language2 = availableLocalization.Name.ToLower();
				

				int allContentsCount = contentLinks.Count();
				for (var i = 0; i < allContentsCount; i += bulkSize)
				{
					var items = contentLoader.GetItems(contentLinks.Skip(i).Take(bulkSize), new LanguageSelector(availableLocalization.Name));
					var items2 = items.OfType<IIndexableContent>().ToList();

					foreach (var content in items2)
					{
						info.NumberOfProductsFound++;

						OnStatusChanged(String.Format("Searching product {0}/{1} - {2}", i + 1, allContentsCount, content.Name));
                        
                        if (content.ShouldIndex())
                        {
							info.NumberOfProductsFoundAfterExpiredFilter++;

                            var findProduct = content.GetFindProduct(market); //findHelper.GetProductAndVariants(content as CatalogContentBase, availableLocalization, market);

							if (findProduct != null)
							{
								client.Index(findProduct);
								info.NumberOfProductsIndexed++;							
							}
						}

						//For long running jobs periodically check if stop is signaled and if so stop execution
						if (_stopSignaled)
						{
							tmr.Stop();
							info.Duration = tmr.ElapsedMilliseconds;
							break;
						}

					}

					//For long running jobs periodically check if stop is signaled and if so stop execution
					if (_stopSignaled)
					{
						tmr.Stop();
						info.Duration = tmr.ElapsedMilliseconds;
						break;
					}

				}
			
			}

			if (_stopSignaled)
			{
				return "Stop of job was called. " + info.ToString();
			}


			tmr.Stop();
			info.Duration = tmr.ElapsedMilliseconds;

			return info.ToString();
		}


		public ContentReference Root
		{

            get { return referenceConverter.GetContentLink(-2147483638, CatalogContentType.Catalog, 0); }
            //get { return referenceConverter.GetRootLink(); }
		}
	}


}
