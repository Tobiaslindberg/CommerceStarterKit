/* jshint -W099 */
/* global jQuery:false */
/* global angular:false */
(function ($, productApp) {

    "use strict";

    productApp.controller('ProductAndSearchController', function ($scope, $location, $window, productService, articleService) {
        $scope.selectedProductCategories = [];
        $scope.selectedColorFacets = [];
        $scope.selectedSizeFacets = [];
        $scope.selectedFitFacets = [];
        $scope.selectedMainCategoryFacets = [];

        $scope.selectedRegionFacets = [];
        $scope.selectedGrapeFacets = [];
        $scope.selectedCountryFacets = [];

        $scope.page = 1;
        $scope.articlePageNumber = 1;
        $scope.showMore = false;
        $scope.showMoreArticles = false;
        $scope.nrOfColumns = 3;
        $scope.pageSizeAtFiveColumns = 25;
        $scope.ShowCategoriesAsCheckboxes = false;
        $scope.ShowMainCategoryFacets = false;
        $scope.SearchDone = false;
        $scope.SearchPage = false;
		$scope.windowWidth = $window.innerWidth;
        /*Selected all filters */
        $scope.categoriesSelectedAll = true;
        $scope.colorSelectedAll = true;
        $scope.fitSelectAll = true;
        $scope.sizeSelectAll = true;
        $scope.regionsSelectedAll = true;
        $scope.grapesSelectedAll = true;
        $scope.countriesSelectedAll = true;

        $scope.init = function (preSelectedCategory, mainCategory, language, pageSize) {
            $scope.language = language;
            if (preSelectedCategory) {
                $scope.selectedProductCategories = preSelectedCategory.split(",");
            } else {
                $scope.selectedProductCategories = [];
            }
            if (mainCategory) {
                $scope.selectedMainCategoryFacets = [mainCategory];
            } else {
                $scope.selectedMainCategoryFacets = [];
            }
            $scope.productPageSize = pageSize;
            $scope.pageSizeAtThreeColumns = pageSize;
            $scope.reset();
        };
        $scope.reset = function() {
            $scope.selectedColorFacets = [];
            $scope.selectedSizeFacets = [];
            $scope.selectedFitFacets = [];
            $scope.selectedRegionFacets = [];
            $scope.selectedGrapeFacets = [];
            $scope.selectedCountryFacets = [];

        };

        $scope.initSearch = function (language, productPageSize, articlePageSize, searchedQuery) {
            $scope.SearchPage = true;
            $scope.language = language;
            $scope.productPageSize = productPageSize;
            $scope.articlePageSize = articlePageSize;
            $scope.pageSizeAtThreeColumns = productPageSize;
            $scope.ShowCategoriesAsCheckboxes = true;
            $scope.ShowMainCategoryFacets = true;
            if (searchedQuery) {
                $scope.queryTerm = searchedQuery;
                //$scope.search();
            }
        };

        $scope.search = function () {
            setProductdata();
            $scope.loadProductData();
            $scope.loadArticles();
            $scope.SearchDone = true;
        };

        $scope.loadArticles = function () {
			$scope.showLoader();
            articleService.get($scope.queryTerm, $scope.language, $scope.articlePageNumber, $scope.articlePageSize).then(
                function (data) {
					$scope.hideLoader();
                    if ($scope.articlePageNumber === 1) {
                        $scope.articles = data.articles;
                    } else {
                        {
                            $scope.articles.push.apply($scope.articles, data.articles);
                        }
                    }
                    $scope.articlesTotalResult = data.totalResult;
                    $scope.showMoreArticles = data.totalResult > $scope.articles.length;

                }
           );

        };

        $scope.loadProductData = function () {
			$scope.showLoader();
            productService.get($scope.productData, $scope.language, $scope.page, $scope.productPageSize).then(
			  function (data) {
				  $scope.hideLoader();
			      if ($scope.page === 1) {
			          $scope.products = data.products;
			      } else {
			          {
			              $scope.products.push.apply($scope.products, data.products);
			          }
			      }
			      $scope.totalResult = data.totalResult;
			      $scope.showMore = data.totalResult > $scope.products.length;
			      $scope.productCategories = data.productCategoryFacets;
			      $scope.mainCategoryFacets = data.mainCategoryFacets;
			      $scope.productColorFacets = data.productColorFacets;
			      $scope.allSizeFacetLists = data.allSizeFacetLists;
			      $scope.productFitFacets = data.productFitFacets;
			      $scope.productRegionFacets = data.productRegionFacets;
			      $scope.productGrapeFacets = data.productGrapeFacets;
			      $scope.productCountryFacets = data.productCountryFacets;

			  },
			  function (statusCode) {
				  $scope.hideLoader();
			      $scope.status = statusCode;
			  }
		  );
        };

		$scope.showLoader = function() {
			$scope.loaderVisible = true;
		};

		$scope.hideLoader = function() {
			$scope.loaderVisible = false;
		};

        function updateWithData() {
            $scope.page = 1;
            setProductdata();
            $scope.loadProductData();
        }

        function setNewProductDataAndUpdate() {
            $scope.page = 1;
            setProductdata();
            updateUrlWithFacets();
            $scope.loadProductData();
        }

        function setProductdata() {
            $scope.productData = {
                SearchTerm: $scope.queryTerm,
                SelectedProductCategories: $scope.selectedProductCategories,
                SelectedMainCategoryFacets: $scope.selectedMainCategoryFacets,
                SelectedColorFacets: $scope.selectedColorFacets,
                SelectedSizeFacets: $scope.selectedSizeFacets,
                SelectedFitsFacets: $scope.selectedFitFacets,
                SelectedRegionFacets: $scope.selectedRegionFacets,
                SelectedGrapeFacets: $scope.selectedGrapeFacets,
                SelectedCountryFacets: $scope.selectedCountryFacets
        };
        }

        //Update url, adding selected facets for product type, colors, sizes and fits
        function updateUrlWithFacets() {
            if ($scope.selectedMainCategoryFacets.length > 0) {
                $location.search('gender', $scope.selectedMainCategoryFacets.join(','));
            } else {
                $location.search('gender', null);
            }
            if ($scope.selectedProductCategories.length > 0) {
                $location.search('categories', $scope.selectedProductCategories.join(','));
            } else {
                $location.search('categories', null);
            }
            if ($scope.selectedColorFacets.length > 0) {
                $location.search('colors', $scope.selectedColorFacets.join(","));
            } else {
                $location.search('colors', null);
            }
            if ($scope.selectedSizeFacets.length > 0) {
                $location.search('sizes', $scope.selectedSizeFacets.join(","));
            } else {
                $location.search('sizes', null);
            }
            if ($scope.selectedFitFacets.length > 0) {
                $location.search('fits', $scope.selectedFitFacets.join(","));
            } else {
                $location.search('fits', null);
            }
            if ($scope.selectedRegionFacets.length > 0) {
                $location.search('regions', $scope.selectedRegionFacets.join(","));
            } else {
                $location.search('regions', null);
            }
            if ($scope.selectedGrapeFacets.length > 0) {
                $location.search('grapes', $scope.selectedGrapeFacets.join(","));
            } else {
                $location.search('grapes', null);
            }
            if ($scope.selectedCountryFacets.length > 0) {
                $location.search('countries', $scope.selectedCountryFacets.join(","));
            } else {
                $location.search('countries', null);
            }
        }

        //We have a watch here, so users can send links with search query and filters, back and forward button will in the browser will also work
        $scope.$watch(function () { return $location.search(); }, function (newUrl, oldUrl) {
            if (jQuery.isEmptyObject(newUrl) && !$scope.SearchPage) {
                $scope.reset();
                updateWithData();
            } else {
                //if (oldUrl != newUrl) {
                    var anyFilters = false;
                    if ($location.search().gender) {
                        $scope.selectedMainCategoryFacets = $location.search().gender.split(',');
                        anyFilters = true;
                    } else {
                        $scope.selectedMainCategoryFacets = [];
                    }
                    if ($location.search().categories) {
                        $scope.selectedProductCategories = $location.search().categories.split(',');
                        anyFilters = true;
                    } else {
                        $scope.selectedProductCategories = [];
                    }
                    if ($location.search().colors) {
                        $scope.selectedColorFacets = $location.search().colors.split(',');
                        anyFilters = true;
                    } else {
                        $scope.selectedColorFacets = [];
                    }
                    if ($location.search().sizes) {
                        $scope.selectedSizeFacets = $location.search().sizes.split(',');
                        anyFilters = true;
                    } else {
                        $scope.selectedSizeFacets = [];
                    }
                    if ($location.search().fits) {
                        $scope.selectedFitFacets = $location.search().fits.split(',');
                        anyFilters = true;
                    } else {
                        $scope.selectedFitFacets = [];
                    }
                    if ($location.search().regions) {
                        $scope.selectedRegionFacets = $location.search().regions.split(',');
                        anyFilters = true;
                    } else {
                        $scope.selectedRegionFacets = [];
                    }
                    if ($location.search().grapes) {
                        $scope.selectedGrapeFacets = $location.search().grapes.split(',');
                        anyFilters = true;
                    } else {
                        $scope.selectedGrapeFacets = [];
                    }
                    if ($location.search().countries) {
                        $scope.selectedCountryFacets = $location.search().countries.split(',');
                        anyFilters = true;
                    } else {
                        $scope.selectedCountryFacets = [];
                    }
                    if ($scope.SearchPage) {
                        if ($scope.queryTerm || anyFilters) {
                            $scope.search();
                        }
                    } else {
                        setNewProductDataAndUpdate();

                    }
                }
            //}
        });

        $scope.LoadMore = function () {
            $scope.page++;
            $scope.loadProductData();
        };

        $scope.LoadMoreArticles = function () {
            $scope.articlePageNumber++;
            $scope.loadArticles();
        };


        var updateSelected = function (action, id, arrayToUpdate) {
            if (action === 'add') {
                arrayToUpdate.push(id);
                setNewProductDataAndUpdate();
            }
            if (action === 'remove') {
                arrayToUpdate.splice(arrayToUpdate.indexOf(id), 1);
                setNewProductDataAndUpdate();
            }
        };

        $scope.updateProductSelection = function ($event, id) {
            var checkbox = $event.target;
            var action = (checkbox.checked ? 'add' : 'remove');
            updateSelected(action, id, $scope.selectedProductCategories);
            $scope.categoriesSelectedAll = $scope.selectedProductCategories > 0;
        };
        $scope.updateColorSelection = function ($event, id) {
            var checkbox = $event.target;
            var action = (checkbox.checked ? 'add' : 'remove');
            updateSelected(action, id, $scope.selectedColorFacets);
            $scope.colorSelectedAll = $scope.selectedColorFacets > 0;
        };
        $scope.updateSizeSelection = function ($event, id, sizeType) {
            var checkbox = $event.target;
            var action = (checkbox.checked ? 'add' : 'remove');
            updateSelected(action, id, $scope.selectedSizeFacets);
            $scope.sizeSelectAll = $scope.selectedSizeFacets > 0;
        };
        $scope.updateFitSelection = function ($event, id) {
            var checkbox = $event.target;
            var action = (checkbox.checked ? 'add' : 'remove');
            updateSelected(action, id, $scope.selectedFitFacets);
            $scope.fitSelectAll = $scope.selectedFitFacets > 0;
        };
        $scope.updateMainCategorySelection = function ($event, id) {
            var checkbox = $event.target;
            var action = (checkbox.checked ? 'add' : 'remove');
            updateSelected(action, id, $scope.selectedMainCategoryFacets);
        };
        $scope.updateRegionSelection = function ($event, id) {
            var checkbox = $event.target;
            var action = (checkbox.checked ? 'add' : 'remove');
            updateSelected(action, id, $scope.selectedRegionFacets);
            $scope.regionsSelectedAll = $scope.selectedRegionFacets > 0;
        };
        $scope.updateGrapeSelection = function ($event, id) {
            var checkbox = $event.target;
            var action = (checkbox.checked ? 'add' : 'remove');
            updateSelected(action, id, $scope.selectedGrapeFacets);
            $scope.grapesSelectedAll = $scope.selectedGrapeFacets > 0;
        };
        $scope.updateCountrySelection = function ($event, id) {
            var checkbox = $event.target;
            var action = (checkbox.checked ? 'add' : 'remove');
            updateSelected(action, id, $scope.selectedCountryFacets);
            $scope.countriesSelectedAll = $scope.selectedCountryFacets > 0;
        };


        $scope.updateViewNrOfColumns = function ($event) {
            var $selector = $($event.currentTarget),
				mode = $selector.data('mode');
            if (mode !== $scope.nrOfColumns) {
                $scope.nrOfColumns = mode;
                $scope.productPageSize = mode === 5 ? $scope.pageSizeAtFiveColumns : $scope.pageSizeAtThreeColumns;
                if ($scope.totalResult > $scope.productPageSize) {
                    $scope.page = 1;
                    $scope.loadProductData();
                }
                else if ($scope.products.length < $scope.productPageSize) {
                    $scope.page = 1;
                    $scope.loadProductData();
                }

            }
        };

        $scope.checkAll = function (facetListToUpdate) {
            facetListToUpdate.length = 0;
            setNewProductDataAndUpdate();
        };

    });


})(jQuery, window.productApp);