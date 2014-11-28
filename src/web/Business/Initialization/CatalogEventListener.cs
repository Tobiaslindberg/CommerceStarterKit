/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Data;
using System.Linq;
using System.Reflection;
using EPiServer;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Framework;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using log4net;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Events;
using OxxCommerceStarterKit.Web.Models.FindModels;

namespace OxxCommerceStarterKit.Web.Business.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Commerce.Initialization.InitializationModule))]
	public class CatalogEventListener : IInitializableModule
	{
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Initialize(InitializationEngine context)
        {
            CatalogEventBroadcaster.Instance.LocalEntryUpdating += InstanceOnLocalEntryUpdating;
            CatalogEventBroadcaster.Instance.LocalRelationUpdating += InstanceOnLocalRelationUpdating;
            EPiServer.Business.Commerce.ProductEventManager.RelationDataUpdating += ProductEventManagerOnRelationDataUpdating;
        }

        private void InstanceOnLocalEntryUpdating(object sender, EntryEventArgs entryEventArgs)
        {
            _log.DebugFormat("Entry Updating ({0}): {1}", entryEventArgs.EventName, sender.GetType().Name);
        }

        public void Uninitialize(InitializationEngine context)
        {

        }

        public void Preload(string[] parameters)
        {

        }

		public void RelationUpdating(object source, RelationEventArgs args)
		{
			var relationDto = (CatalogRelationDto)source;

			relationDto.NodeEntryRelation.NodeEntryRelationRowChanging += (sender, e) => NodeEntryRelationRowChanging(sender, e);
		}


        private void ProductEventManagerOnRelationDataUpdating(object sender, CatalogContentUpdateEventArgs catalogContentUpdateEventArgs)
        {
            _log.DebugFormat("Relation Updating ({0}): {1}", catalogContentUpdateEventArgs.EventType,
                string.Join(" | ", catalogContentUpdateEventArgs.CatalogIds));
        }

        private void InstanceOnLocalRelationUpdating(object sender, RelationEventArgs relationEventArgs)
        {
            _log.DebugFormat("Relation Updating ({0}): {1}", relationEventArgs.EventName, sender.GetType().Name);
            RelationUpdating(sender, relationEventArgs);
        }



		protected virtual void NodeEntryRelationRowChanging(object sender, CatalogRelationDto.NodeEntryRelationRowChangeEvent e)
		{
			if (e.Action == DataRowAction.Commit || e.Action == DataRowAction.Delete || e.Action == DataRowAction.Change)
			{
				if (e.Row.RowState == DataRowState.Added || e.Row.RowState == DataRowState.Deleted || e.Row.RowState == DataRowState.Modified)
				{
					int id = 0, catalogNodeId = 0;

					if (e.Row.RowState == DataRowState.Deleted)
					{
						id = (int)e.Row["CatalogEntryId", DataRowVersion.Original];
						catalogNodeId = (int)e.Row["CatalogNodeId", DataRowVersion.Original];
					}
					else
					{
						id = e.Row.CatalogEntryId;
						catalogNodeId = e.Row.CatalogNodeId;
					}


					UpdateSearchIndexWithParentCategoryIdChanges(id, catalogNodeId, e.Row.RowState);

				}
			}
		}


		protected virtual void UpdateSearchIndexWithParentCategoryIdChanges(int id, int catalogNodeId, DataRowState action)
		{
			var referenceConverter = ServiceLocator.Current.GetInstance<ReferenceConverter>();
			var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();


			var catalogNode = CatalogContext.Current.GetCatalogNode(catalogNodeId);
			var parentCategoryLink = referenceConverter.GetContentLink(catalogNode.ID, CatalogContentType.CatalogNode);
			IContent parentCategory = null;
			try {
				parentCategory = contentLoader.Get<IContent>(parentCategoryLink, new LanguageSelector("no"));
			} catch(Exception) {
			}
			var parentCategoryName = parentCategory != null ? parentCategory.Name : "did not find the name";
			var parentCategoryId = parentCategoryLink.ID;

			IClient client = SearchClient.Instance;

			// find all languages in the index
			var results = client.Search<FindProduct>()
				.Filter(x => x.Id.Match(id))
                .StaticallyCacheFor(TimeSpan.FromMinutes(1))
				.GetResult();

			foreach (var product in results)
			{

				if (action == DataRowState.Deleted)
				{
					var categoryIndex = product.ParentCategoryId.IndexOf(parentCategoryId);
					if (categoryIndex > -1)
					{
						product.ParentCategoryId.RemoveAt(categoryIndex);
						product.ParentCategoryName.RemoveAt(categoryIndex);
						
						client.Index(product);
					}
				}
				else if(!product.ParentCategoryId.Any(x => x == parentCategoryId))
				{
					product.ParentCategoryId.Add(parentCategoryId);
					product.ParentCategoryName.Add(parentCategoryName);

					client.Index(product);
				}

				
			}
			

		}
	}
}
