/* jshint -W099 */
/* global jQuery:false */
/* global angular:false */
(function($, productApp) {

	"use strict";

	productApp.factory("productService", function ($http, $q) {
		return {
		    get: function (productData, language, pageNumber, pageSize) {
				var deferred = $q.defer();

		        $http.post('/' + language + '/api/Shopping/GetProducts?_=' + new Date().getTime().toString(), {productData: productData, page: pageNumber, pageSize: pageSize }).
					success(function (data, status, headers, config) {
						deferred.resolve(data);
					}).
					error(function (data, status, headers, config) {
						deferred.reject(status);
					});
				return deferred.promise;
			}
		};

	});

})(jQuery, window.productApp);