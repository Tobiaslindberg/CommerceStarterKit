/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Http;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Find.Framework;
using EPiServer.Find.Framework.Statistics;
using EPiServer.Web.Routing;

namespace OxxCommerceStarterKit.Web.Api
{
    public class ArticleSearchController : BaseApiController
    {
        public class ArticleSearchData
        {
            public string SearchTerm { get; set; }
            [DefaultValue(1)]
            public int Page { get; set; }
            [DefaultValue(10)]
            public int PageSize { get; set; }
        }
        public class ArticleObject
        {
            public string DisplayName { get; set; }
            public string Image { get; set; }
            public string Url { get; set; }
            public string Description { get; set; }
        }

        [HttpPost]
        public object GetArticles(ArticleSearchData articleSearchData)
        {
            try
            {
                string language = Language;
                var queryResult = SearchClient.Instance.Search<PageData>()
                    .For(articleSearchData.SearchTerm)
                    .Filter(x => x.Language.Name.Match(language))
                    .Skip((articleSearchData.Page - 1)*articleSearchData.PageSize)
                    .Take(articleSearchData.PageSize)
                    .Track()
                    .FilterForVisitor()
                    .Select(y => new ArticleObject
                    {
                        DisplayName = y.PageName,
                        Image = "",
                        Url = UrlResolver.Current.GetUrl(y.PageLink)
                    }).StaticallyCacheFor(TimeSpan.FromMinutes(1)).GetResult();
                var totalMatching = queryResult.TotalMatching;
                var result = new
                {
                    articles = queryResult.ToList(),
                    totalResult = totalMatching
                };
                return result;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (ServiceException)
            {
                return null;
            }
        }
    }
}
