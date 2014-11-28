/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using EPiServer.Find;
using EPiServer.Find.Api.Facets;
using EPiServer.Find.Framework;
using OxxCommerceStarterKit.Web.Models.FindModels;

namespace OxxCommerceStarterKit.Web.Api
{
    public class TagController : BaseApiController
    {
        [HttpGet]
        public object GetBrands(string q)
        {
            try
            {
                if (q == null)
                {
                    q = "";
                }
                string language = Language;
                //Starting the find query
                var query = SearchClient.Instance.Search<WineFindProduct>(GetLanguage(language));
                if (!string.IsNullOrEmpty(q))
                    query = query.Filter(x => x.Brand.PrefixCaseInsensitive(q));
                var facetResult =  query.TermsFacetFor(x => x.Brand)
                    .Take(0)
                    .StaticallyCacheFor(TimeSpan.FromMinutes(1))
                    .GetResult();
                var tags = facetResult
                    .TermsFacetFor(x => x.Brand).Terms;
                return TagList(tags);
            }
            catch (ServiceException)
            {
                return null;
            }
        }


        [HttpGet]
        public object GetCountries(string q)
        {
            try
            {
                if (q == null)
                {
                    q = "";
                }
                string language = Language;
                //Starting the find query
                var query = SearchClient.Instance.Search<WineFindProduct>(GetLanguage(language));
                if (!string.IsNullOrEmpty(q))
                    query = query.Filter(x => x.Country.PrefixCaseInsensitive(q));
                var facetResult = query.TermsFacetFor(x => x.Country)
                    .Take(0)
                    .StaticallyCacheFor(TimeSpan.FromMinutes(1))
                    .GetResult();
                var tags = facetResult
                    .TermsFacetFor(x => x.Country).Terms;
                return TagList(tags);
            }
            catch (ServiceException)
            {
                return null;
            }
        }

        private object TagList(IEnumerable<TermCount> tags)
        {
            List<SelectItem> brands = new List<SelectItem>();
            if (tags.Any())
            {
                int id = 1;
                foreach (var tag in tags)
                {
                    SelectItem selectItem = new SelectItem();
                    selectItem.id = id;
                    selectItem.text = tag.Term;
                    brands.Add(selectItem);
                    id++;
                }
                return brands;
            }
            return null;
        }
        public class SelectItem
        {
            public int id { get; set; }
            public string text { get; set; }
        }
    }
}
