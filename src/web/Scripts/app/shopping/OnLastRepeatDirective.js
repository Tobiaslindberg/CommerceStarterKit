/* jshint -W099 */
/* global jQuery:false */
/* global angular:false */
(function($, productApp) {

	"use strict";

	productApp.directive('onLastRepeat', function () {
		return function (scope, element, attrs) {
			if (scope.$last) {
				setTimeout(setEqualHeights, 10);

			}
		};

		function setEqualHeights() {
			if ($(window).width() > 737) {
				$('#equalHeights .product').equalHeights();
			}
		}

	});

})(jQuery, window.productApp);