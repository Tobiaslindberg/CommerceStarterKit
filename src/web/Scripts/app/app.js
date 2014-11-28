/* jshint -W099 */
/* global jQuery:false */
/* global angular:false */
(function($, productApp) {

	"use strict";

	productApp = angular.module('productApp', ['ngResource', 'ui.bootstrap']);
    productApp.directive('loadDispatcher', function() {
        return {
            link:function(scope, element, attrs) {
                element.bind('load', function () {
                    scope.imageLoaded();
                });
            }
        };
    });
	window.productApp = productApp;

})(jQuery, window.productApp);
