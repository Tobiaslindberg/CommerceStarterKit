/* jshint -W099 */
/* global jQuery:false */
/* global angular:false */
(function ($, productApp) {

    "use strict";

    productApp.controller('AutoCompleteController', function ($scope) {
        $scope.init = function(language) {
            $scope.language = language;
        };
    });


    productApp.directive('autoComplete', ['$http', function ($http) {
        return function (scope, element, attrs) {
            var currentCategory = "";
			var a = element.autocomplete({
                minLength: 2,
				position: { my: 'right top', at: 'right bottom', collision: 'none'},
                source: function (request, response) {
                    $.getJSON('/' + scope.language + '/api/AutocompleteSearch/Search', { term: request.term }).success(function (data) {
                        currentCategory = "";
                        response(data.allResult);
                    });
                },

                select: function (event, ui) {
                    location.href = ui.item.Url;
                }

            });
			a.data("ui-autocomplete")._renderItem = function (ul, item) {
                if (item.ResultType !== currentCategory) {
                    currentCategory = item.ResultType;
                    $("<li><h3>" + item.GroupHeading + "</h3></li>").appendTo(ul);
                }
                if (item.ResultType === "products") {
                    return $('<li class="product"></li>')
                    .data("ui-autocomplete-item", item)
                    .append('<a class="clearfix">' + '<img src="' + item.Image + '?preset=largethumbnail" />' + item.Name + '<br/>' + item.Price + '</a>')
                    .appendTo(ul);
                } else {
                    return $("<li></li>")
                    .data("ui-autocomplete-item", item)
                    .append("<a>" + item.Name + "</a>")
                    .appendTo(ul);
                }

            };
			a.data("ui-autocomplete")._resizeMenu = function () {
				this.menu.element.addClass("search-autocomplete");
			};
        };
    }]);
})(jQuery, window.productApp);