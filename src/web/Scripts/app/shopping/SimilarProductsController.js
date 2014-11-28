/* jshint -W099 */
/* global jQuery:false */
/* global angular:false */
(function($, productApp) {

    productApp.controller('SimilarProductsController', function ($scope) {
        $scope.init = function (id,language) {
            $scope.language = language;
			$scope.visible = false;

            var findIndexId = id + '_' + language;
            $.getJSON('/' + $scope.language + '/api/SimilarProducts/GetSimilarProducts', { indexId: findIndexId, _: new Date().getTime().toString() }).success(function (data) {
                $scope.similarProducts = data;
				$scope.visible = typeof($scope.similarProducts) !== 'undefined' && $scope.similarProducts.length > 0;

                $scope.$apply();
            });
        };
        var loadCount = 0;
        $scope.imageLoaded = function() {
            if (loadCount++ === $scope.similarProducts.length - 1) {
                initSimilarProducts();
            }
        }
    });

   

})(jQuery, window.productApp);

function initSimilarProducts() {
    $('#similar-products .slider').flexslider({
        animation: 'slide',
        controlNav: false,
        slideshow: false,
        prevText: '',
        nextText: '',
        itemWidth: 120,
        maxItems: 6,
        animationLoop: false
    });
};