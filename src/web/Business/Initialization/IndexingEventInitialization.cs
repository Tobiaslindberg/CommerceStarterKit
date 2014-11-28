/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Find.Cms.Conventions;
using EPiServer.Find.Framework;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using log4net;
using OxxCommerceStarterKit.Web.Helpers;
using OxxCommerceStarterKit.Web.Models.Blocks;
using OxxCommerceStarterKit.Web.Models.Blocks.Contracts;
using OxxCommerceStarterKit.Web.Models.Catalog;
using OxxCommerceStarterKit.Web.Models.Files;
using OxxCommerceStarterKit.Web.Models.FindModels;
using OxxCommerceStarterKit.Web.Models.PageTypes;
using OxxCommerceStarterKit.Web.Models.PageTypes.Payment;
using OxxCommerceStarterKit.Web.Models.PageTypes.System;
using OxxCommerceStarterKit.Web.Models.ViewModels.Email;

namespace OxxCommerceStarterKit.Web.Business.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class IndexingEventInitialization : IInitializableModule
    {
        protected static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Set this flag to temporary disable indexing while running long jobs. It is enabled by default.
        /// </summary>
        public static bool IndexingEnabled = true;

        public void Initialize(InitializationEngine context)
        {
            //We do not want to index catalog content types, since we are creating our own objects below
            ContentIndexer.Instance.Conventions.ForInstancesOf<FashionItemContent>().ShouldIndex(x => false);
            ContentIndexer.Instance.Conventions.ForInstancesOf<FashionProductContent>().ShouldIndex(x => false);

			// other page types we do not want in the index
			ContentIndexer.Instance.Conventions.ForInstancesOf<ReceiptPage>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<ContentFolder>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<OrdersPage>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<HomePage>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<CartSimpleModulePage>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<WishListPage>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<ChangePasswordPage>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<PersonalInformationPage>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<DibsPaymentPage>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<SearchPage>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<DefaultPage>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<LoginPage>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<CheckoutPage>().ShouldIndex(x => false);
			// others
			ContentIndexer.Instance.Conventions.ForInstancesOf<NotificationSettings>().ShouldIndex(x => false);
			// blocks
			ContentIndexer.Instance.Conventions.ForInstancesOf<YouTubeBlock>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<ViddlerBlock>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<VimeoBlock>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<TwoColumnsBlock>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<SocialMediaLinkBlock>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<ButtonWithHelpLinkBlock>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<SliderBlock>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<OneTwoColumnsBlock>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<PageListBlock>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<ImageFile>().ShouldIndex(x => false);
			ContentIndexer.Instance.Conventions.ForInstancesOf<GenericFile>().ShouldIndex(x => false);

			// hook up events
			// maybe read this http://talk.alfnilsson.se/2013/03/17/simplifying-event-handling-in-episechess.crver-7-cms/
			IContentEvents events = ServiceLocator.Current.GetInstance<IContentEvents>();
            events.PublishedContent += (sender, e) => events_PublishedContent(sender, e);
        }



        /// <summary>
        /// Note! This method should not fail, it will prevent the product from 
        /// being saved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void events_PublishedContent(object sender, ContentEventArgs e)
        {
            if(IndexingEnabled == false)
                return;

            FindHelper helper = ServiceLocator.Current.GetInstance<FindHelper>();
          
            if (e.Content is IIndexableContent)
            {
                try
                {
                    IndexProduct(helper, e.Content as IIndexableContent);
                }
                catch (Exception exception)
                {
                    _log.Error("Cannot index: " + e.ContentLink.ToString(), exception);
                }
            }
            else if (e.Content is FashionItemContent)
            {
                EntryContentBase parent = GetParent(e.Content as EntryContentBase);

                FashionProductContent productContent = parent as FashionProductContent;
                if (productContent != null)
                {
                    try
                    {
                        IndexProduct(helper, productContent);
                    }
                    catch (Exception exception)
                    {
                        _log.Error("Cannot index: " + e.ContentLink.ToString(), exception);
                    }

                }

            }
        }

        private static void IndexProduct(FindHelper helper, IIndexableContent p)
        {            
            if (p.ShouldIndex())
            {
            var currentMarket = ServiceLocator.Current.GetInstance<Mediachase.Commerce.ICurrentMarket>().GetCurrentMarket();
				FindProduct findProduct = p.GetFindProduct(currentMarket);
				if (findProduct != null)
				{
					IClient client = SearchClient.Instance;
					client.Index(findProduct);
				}
			}
			else
			{
				//TODO: remove product from index
                //IClient client = SearchClient.Instance;
                //var lang = p.Language;
                //client.Delete<FindProduct>(productContent.ContentLink.ID + "_" + (lang == null ? string.Empty : lang.Name));
			}
        }

        private EntryContentBase GetParent(CatalogContentBase content)
        {

            ILinksRepository linksRepository = ServiceLocator.Current.GetInstance<ILinksRepository>();
            IContentLoader contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();

            IEnumerable<Relation> parentRelations = linksRepository.GetRelationsByTarget(content.ContentLink);
            if (parentRelations.Any())
            {
                Relation firstRelation = parentRelations.FirstOrDefault();
                if (firstRelation != null)
                {
                    var parentProductContent = contentLoader.Get<EntryContentBase>(firstRelation.Source);
                    return parentProductContent;
                }
            }
            return null;
        }

        public void Preload(string[] parameters) { }

        public void Uninitialize(InitializationEngine context)
        {
            //Add uninitialization logic
        }
    }
}
