/* jshint -W099 */
/* global jQuery:false */
/* global angular:false */
(function ($, productApp) {

	"use strict";

	productApp.factory("articleService", function ($http, $q) {
		return {
			get: function (searchTerm, language, pageNumber, pageSize) {
				var deferred = $q.defer();

				$http.post('/' + language + '/api/ArticleSearch/GetArticles?_=' + new Date().getTime().toString() , {
					searchTerm: searchTerm, page: pageNumber, pageSize: pageSize
				}).
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