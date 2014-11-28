/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using OxxCommerceStarterKit.Core.Extensions;
using OxxCommerceStarterKit.Web.Models.Catalog;

namespace OxxCommerceStarterKit.Web.Models.ViewModels
{
    public class FashionProductViewModel : CatalogContentViewModel<FashionProductContent>
    {
        public FashionProductViewModel(FashionProductContent currentContent)
            : base(currentContent)
        {
            CatalogContent = currentContent;
        }

        public IEnumerable<System.Web.Mvc.SelectListItem> Color { get; set; }
		public IEnumerable<SelectListItem> Size { get; set; }
        public IVariationViewModel<FashionItemContent> FashionItemViewModel { get; set; }
		public XhtmlString DeliveryAndReturns { get; set; }
		public bool IsSellable { get; set; }
		public string SizeUnit { get; set; }
		public string SizeType { get; set; }

		public int SizeGuideReference { get; set; }

		private List<MediaData> _media;
        public List<MediaData> Media
        {
            get
            {                
                if (_media == null)
                {
                    var contentLoader = ServiceLocator.Current.GetInstance<EPiServer.IContentLoader>();
                    List<ContentReference> mediaReferences = ContentWithAssets.AssetImageUrls();
                    _media = new List<MediaData>();
                    foreach (var mediaReference in mediaReferences)
                    {
                        var file = contentLoader.Get<MediaData>(mediaReference);
                        if (file != null)
                        {
                            _media.Add(file);
                        }
                    }
                }
                return _media;
            }
        }
    }
}
