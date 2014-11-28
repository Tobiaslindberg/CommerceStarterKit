/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Http;
using EPiServer.Find;
using EPiServer.Find.Api.Facets;
using EPiServer.Find.Framework;
using EPiServer.Find.Framework.Statistics;
using OxxCommerceStarterKit.Web.Models.FindModels;

namespace OxxCommerceStarterKit.Web.Api
{
    public class ShoppingController : BaseApiController
    {
        public class FacetViewModel
        {
            public string Name { get; set; }
            public int Count { get; set; }
            public bool Selected { get; set; }
            public string Size { get; set; }
			public string Id { get; set; }
        }

        public class SizeFacetViewModel
        {
            public string SizeUnit { get; set; }
            public bool SomeIsSelected { get; set; }
            public List<FacetViewModel> SizeFacets { get; set; }
        }


        public class ProductSearchData
        {

            [DefaultValue(1)]
            public int Page { get; set; }
            [DefaultValue(10)]
            public int PageSize { get; set; }
            public ProductData ProductData { get; set; }

        }
        public class ProductData
        {
            public List<int> SelectedProductCategories { get; set; }
            public List<string> SelectedColorFacets { get; set; }
            public List<string> SelectedSizeFacets { get; set; }
            public List<string> SelectedFitsFacets { get; set; }
            public List<string> SelectedMainCategoryFacets { get; set; }
            public List<string> SelectedRegionFacets { get; set; }
            public List<string> SelectedGrapeFacets { get; set; }
            public List<string> SelectedCountryFacets { get; set; } 
            public string SearchTerm { get; set; }
        }


        [HttpPost]
        public object GetProducts(ProductSearchData productSearchData)
        {
            try
            {
                string language = Language;
                //Starting the find query
                var query = SearchClient.Instance.Search<FindProduct>(GetLanguage(language));

                // search term
                query = ApplyTermFilter(productSearchData, query, true);

                // common filters
                query = ApplyCommonFilters(productSearchData, query, language);



                // selected categories
                if (productSearchData.ProductData.SelectedProductCategories != null &&
                    productSearchData.ProductData.SelectedProductCategories.Any())
                {
                    query = query.Filter(x => GetCategoryFilter(productSearchData.ProductData.SelectedProductCategories));
                }

                // selected colors
                if (productSearchData.ProductData.SelectedColorFacets != null &&
                    productSearchData.ProductData.SelectedColorFacets.Any())
                {
                    query = query.Filter(x => GetColorFilter(productSearchData.ProductData.SelectedColorFacets));
                }

                // selected sizes
                if (productSearchData.ProductData.SelectedSizeFacets != null &&
                    productSearchData.ProductData.SelectedSizeFacets.Any())
                {
                    query = query.Filter(x => GetSizeFilter(productSearchData.ProductData.SelectedSizeFacets));
                }

                // selected fits
                if (productSearchData.ProductData.SelectedFitsFacets != null &&
                    productSearchData.ProductData.SelectedFitsFacets.Any())
                {
                    query = query.Filter(x => GetFitFilter(productSearchData.ProductData.SelectedFitsFacets));
                }

                // selected region
                if (productSearchData.ProductData.SelectedRegionFacets != null &&
                    productSearchData.ProductData.SelectedRegionFacets.Any())
                {
                    query = query.Filter(x => GetRegionFilter(productSearchData.ProductData.SelectedRegionFacets));
                }

                // selected grapes
                if (productSearchData.ProductData.SelectedGrapeFacets != null &&
                    productSearchData.ProductData.SelectedGrapeFacets.Any())
                {
                    query = query.Filter(x => GetGrapeFilter(productSearchData.ProductData.SelectedGrapeFacets));
                }

                //selected countries
                if (productSearchData.ProductData.SelectedCountryFacets != null &&
                    productSearchData.ProductData.SelectedCountryFacets.Any())
                {
                    query = query.Filter(x => GetCountryFilter(productSearchData.ProductData.SelectedCountryFacets));
                }

                // execute search
                query = query.Skip((productSearchData.Page - 1)*productSearchData.PageSize)
                    .Take(productSearchData.PageSize);
                var searchResult = query.StaticallyCacheFor(TimeSpan.FromMinutes(1)).GetResult();
                //Done with search query

                #region Facet search

                //Facets for product cagetories, get all, only filtered on search term and main category(menn/dame) if selected
                var productFacetQuery = SearchClient.Instance.Search<FindProduct>(GetLanguage(language));

                // search term
                productFacetQuery = ApplyTermFilter(productSearchData, productFacetQuery);

                // common filters
                productFacetQuery = ApplyCommonFilters(productSearchData, productFacetQuery, language);


                // categories
                if (productSearchData.ProductData.SelectedMainCategoryFacets != null &&
                    productSearchData.ProductData.SelectedMainCategoryFacets.Any())
                {
                    productFacetQuery =
                        productFacetQuery.Filter(
                            x => GetMainCategoryFilter(productSearchData.ProductData.SelectedMainCategoryFacets));
                }

                // execute 
                var productFacetsResult = productFacetQuery
                    .TermsFacetFor(x => x.ParentCategoryName)
                    .TermsFacetFor(x => x.MainCategoryName)
                    .Take(0)
                    .GetResult();

                // results
                var productCategoryFacetsResult = productFacetsResult.TermsFacetFor(x => x.ParentCategoryName).Terms;
                var productMainCategoryFacetResult = productFacetsResult.TermsFacetFor(x => x.MainCategoryName).Terms;
                var allProductCategoryFacets = CreateCategoryFacetViewModels(productCategoryFacetsResult,
                    productSearchData.ProductData.SelectedProductCategories.Select(x => x.ToString()).ToList());
                var allMainCategoryFacets = CreateFacetViewModels(productMainCategoryFacetResult,
                    productSearchData.ProductData.SelectedMainCategoryFacets);

                //Facets - To get all, color, size and fit facets, based on selected product categories
                var facetsQuery = SearchClient.Instance.Search<FindProduct>(GetLanguage(language));

                // search term
                facetsQuery = ApplyTermFilter(productSearchData, facetsQuery);

                // common filters
                facetsQuery = ApplyCommonFilters(productSearchData, facetsQuery, language);

                // execute search
                var facetsResult = facetsQuery
                    .Filter(x => GetCategoryFilter(productSearchData.ProductData.SelectedProductCategories))
                    .Filter(x => GetMainCategoryFilter(productSearchData.ProductData.SelectedMainCategoryFacets))
                    .TermsFacetFor(x => x.Color, r => r.Size = 50)
                    .TermsFacetFor(x => x.Fit, r => r.Size = 50)
                    .TermsFacetFor(x => x.SizesList, r => r.Size = 200)
                    .TermsFacetFor(x => x.Region, r => r.Size = 50)
                    .TermsFacetFor(x => x.GrapeMixList, r => r.Size = 50)
                    .TermsFacetFor(x => x.Country, r => r.Size = 50)
                    .Take(0)
                    .StaticallyCacheFor(TimeSpan.FromMinutes(1))
                    .GetResult();

                // results
                var productColorFacetsResult = facetsResult.TermsFacetFor(x => x.Color).Terms;
                var productFitFacetsResult = facetsResult.TermsFacetFor(x => x.Fit).Terms;
                var productsizesResult = facetsResult.TermsFacetFor(x => x.SizesList).Terms;
                var productRegionResult = facetsResult.TermsFacetFor(x => x.Region).Terms;
                var productGrapeResult = facetsResult.TermsFacetFor(x => x.GrapeMixList).Terms;
                var productCountryResult = facetsResult.TermsFacetFor(x => x.Country).Terms;
                var allColorFacets = CreateFacetViewModels(productColorFacetsResult,
                    productSearchData.ProductData.SelectedColorFacets);
                var allFitFacets = CreateFacetViewModels(productFitFacetsResult,
                    productSearchData.ProductData.SelectedFitsFacets);
                var allRegionFacets = CreateFacetViewModels(productRegionResult,
                    productSearchData.ProductData.SelectedRegionFacets);
                var allGrapeFacets = CreateFacetViewModels(productGrapeResult,
                    productSearchData.ProductData.SelectedGrapeFacets);
                var allcountryFacets = CreateFacetViewModels(productCountryResult,
                    productSearchData.ProductData.SelectedCountryFacets);
                //Testing different size type facets
                var allDifferentSizeFacets = GetAllDifferentSizeFacets(productsizesResult,
                    productSearchData.ProductData.SelectedSizeFacets);

                #endregion

                var totalMatching = searchResult.TotalMatching;

                var result = new
                {
                    products = searchResult.ToList(),
                    productCategoryFacets = allProductCategoryFacets,
                    productColorFacets = allColorFacets,
                    allSizeFacetLists = allDifferentSizeFacets,
                    productFitFacets = allFitFacets,
                    productRegionFacets = allRegionFacets,
                    productGrapeFacets = allGrapeFacets,
                    productCountryFacets = allcountryFacets,
                    mainCategoryFacets = allMainCategoryFacets,
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

		private static ITypeSearch<FindProduct> ApplyCommonFilters(ProductSearchData productSearchData, ITypeSearch<FindProduct> query, string language)
		{
		    return query.Filter(x => x.Language.Match(language))
		        .Filter(x => x.ShowInList.Match(true));
		}

		private static ITypeSearch<FindProduct> ApplyTermFilter(ProductSearchData productSearchData, ITypeSearch<FindProduct> query, bool trackSearchTerm = false)
		{
			if (!string.IsNullOrEmpty(productSearchData.ProductData.SearchTerm))
			{
				query = query.For(productSearchData.ProductData.SearchTerm)
					.InFields(x => x.Name, x => x.MainCategoryName, x => string.Join(",", x.Color),
						x => x.DisplayName, x => x.Fit, x => x.Description.ToString(), x => string.Join(",", x.ParentCategoryName))
						.InAllField();

				if (trackSearchTerm)
				{
					query = query.Track();
				}
			}
			return query;
		}

        private List<SizeFacetViewModel> GetAllDifferentSizeFacets(IEnumerable<TermCount> facetResult, List<string> selectedSizes)
        {
            List<SizeFacetViewModel> allDifferentSizeFacets = new List<SizeFacetViewModel>();

			foreach (var sizeFacet in facetResult.OrderBy(x => SortSizes(x.Term)))
            {
				string term = sizeFacet.Term.ToLower();
				string[] typeAndSize = term.Split('/');
                string sizeType = typeAndSize[0];
                string size = typeAndSize[1];
				if (typeAndSize.Length == 3)
				{
					sizeType = typeAndSize[1];
					size = typeAndSize[2];
				}
				
                bool newSizeTypeList = true;
				SizeFacetViewModel sizeList = allDifferentSizeFacets.FirstOrDefault(x => x.SizeUnit == sizeType);
				if (sizeList == null)
				{
					sizeList = new SizeFacetViewModel();
					sizeList.SizeUnit = sizeType;
					sizeList.SizeFacets = new List<FacetViewModel>();
				}
				else
				{
					newSizeTypeList = false;
				}
				// check for duplicate entry and join them
				var facetViewModel = sizeList.SizeFacets.FirstOrDefault(x => x.Name == term);
				if (facetViewModel == null)
				{
					facetViewModel = new FacetViewModel();
					facetViewModel.Name = term;
					facetViewModel.Size = size;
					facetViewModel.Count = sizeFacet.Count;
					sizeList.SizeFacets.Add(facetViewModel);
				}
				else
				{
					facetViewModel.Count += sizeFacet.Count;
				}
				facetViewModel.Selected = selectedSizes != null && selectedSizes.Contains(term);

                if (sizeList.SizeFacets.Any(x => x.Selected.Equals(true)))
                {
                    sizeList.SomeIsSelected = true;
                }
                if (newSizeTypeList)
                {
                    allDifferentSizeFacets.Add(sizeList);
                }
            }
            return allDifferentSizeFacets;
        }

		/// <summary>
		/// Add a number in front of letter sizes to specify ordering
		/// </summary>
		/// <param name="term">Examples: unisex/xxl , 85-105, 85, inch/82-82, cm ny/20</param>
		/// <returns></returns>
		public static int SortSizes(string term) {
			if (term == null)
			{
				return 10000000;
			}
			term = term.ToLower();
			string sizeType = "", sizeUnit = "", size = "";
			var terms = term.Split('/');
			if (terms.Length == 2)
			{
				sizeUnit = terms[0];
				size = terms[1];
			}
			else if(terms.Length == 3)
			{
				sizeType = terms[0];
				sizeUnit = terms[1];
				size = terms[2];
			}
			else
			{
				return 10000000;
			}

			if (sizeType.StartsWith("m") &&
				sizeUnit == "standard") // herre jakke
			{
				// group sizes by ranges
				int sizeNumber = ParseInt(size, 1000000);
				if (sizeNumber == 1000000)
				{
					return sizeNumber;
				}
				if (sizeNumber >= 46 && sizeNumber <= 64)
				{
					return sizeNumber + 1000;
				}
				else if (sizeNumber >= 23 && sizeNumber <= 31)
				{
					return sizeNumber + 10000;
				}
				else if (sizeNumber >= 146 && sizeNumber <= 160)
				{
					return sizeNumber + 100000;
				}
				return sizeNumber + 1000000;
			}

			if (sizeUnit == "unisex")
			{
				var sort = LetterSizeSortIndex.blokk;
				Enum.TryParse<LetterSizeSortIndex>(size, true, out sort);
				return ((int)sort);
			}


			// check for number
			if (size.Contains("-"))
			{
				return GetSortScoreForTwoSizes(size);
			}
			else
			{
				return ParseInt(size, 100000);
			}
		}
		public static int ParseInt(string input, int defaultValue = 0)
		{
			int val = defaultValue;
			if (Int32.TryParse(input, out val))
			{
				return val;
			}
			else
			{
				return defaultValue;
			}
		}
		private static int GetSortScoreForTwoSizes(string term)
		{
			string sizePart = "";
			int sizeNumber = 0;
			sizePart = term.Split('-')[0];
			sizeNumber = ParseInt(sizePart, 100000) * 100;
			sizePart = term.Split('-')[1];
			sizeNumber += ParseInt(sizePart);
			return sizeNumber;
		}
		private enum LetterSizeSortIndex
		{
			xxxs = 10,
			xxs = 11,
			xs = 12,
			s = 13,
			m = 14,
			l = 15,
			xl = 16,
			xxl = 17,
			xxxl = 18,
			xxxxl = 19,
			xxxxxl = 20,
			xxxxxxl = 21,
			blokk = 50
		}

        private List<FacetViewModel> CreateFacetViewModels(IEnumerable<TermCount> facetResult, List<string> selectedFacets)
        {
            return facetResult.Select(
                        term =>
                            new FacetViewModel
                            {
                                Count = term.Count,
                                Name = term.Term,
                                Selected = selectedFacets != null && selectedFacets.Contains(term.Term)
                            }).ToList();
        }

		private List<FacetViewModel> CreateCategoryFacetViewModels(IEnumerable<TermCount> facetResult, List<string> selectedFacets)
		{
			return facetResult.Select(
						term =>{
							string id = term.Term.Contains("|") ? term.Term.Split('|')[0] : term.Term;
							return new FacetViewModel
								{
									Count = term.Count,
									Name = term.Term.Contains("|") ? term.Term.Split('|')[1] : term.Term,
									Id = id,
									Selected = selectedFacets != null && selectedFacets.Contains(id)
								};
						}).ToList();
		}
		

        private FilterBuilder<FindProduct> GetCategoryFilter(List<int> categories)
        {
            var filter = SearchClient.Instance.BuildFilter<FindProduct>();
            return categories.Aggregate(filter, (current, category) => current.Or(x => x.ParentCategoryId.Match(category)));
        }
        private FilterBuilder<FindProduct> GetMainCategoryFilter(List<string> mainCategories)
        {
            var filter = SearchClient.Instance.BuildFilter<FindProduct>();
            return mainCategories.Aggregate(filter, (current, mainCategory) => current.Or(x => x.MainCategoryName.Match(mainCategory)));
        }

        private FilterBuilder<FindProduct> GetColorFilter(List<string> colorsList)
        {
            var filter = SearchClient.Instance.BuildFilter<FindProduct>();
            return colorsList.Aggregate(filter, (current, color) => current.Or(x => x.Color.Match(color)));
        }
        private FilterBuilder<FindProduct> GetSizeFilter(List<string> sizeList)
        {
            var filter = SearchClient.Instance.BuildFilter<FindProduct>();
            return sizeList.Aggregate(filter, (current, size) => current.Or(x => x.SizesList.MatchCaseInsensitive(size)));
        }
        private FilterBuilder<FindProduct> GetFitFilter(List<string> fitList)
        {
            var filter = SearchClient.Instance.BuildFilter<FindProduct>();
            return fitList.Aggregate(filter, (current, fit) => current.Or(x => x.Fit.Match(fit)));
        }
        private FilterBuilder<FindProduct> GetRegionFilter(List<string> regionList)
        {
            var filter = SearchClient.Instance.BuildFilter<FindProduct>();
            return regionList.Aggregate(filter, (current, region) => current.Or(x => x.Region.Match(region)));
        }
        private FilterBuilder<FindProduct> GetGrapeFilter(List<string> grapeList)
        {
            var filter = SearchClient.Instance.BuildFilter<FindProduct>();
            return grapeList.Aggregate(filter, (current, grape) => current.Or(x => x.GrapeMixList.Match(grape)));
        }
        private FilterBuilder<FindProduct> GetCountryFilter(List<string> countryList)
        {
            var filter = SearchClient.Instance.BuildFilter<FindProduct>();
            return countryList.Aggregate(filter, (current, country) => current.Or(x => x.Country.Match(country)));
        }

    }
}
