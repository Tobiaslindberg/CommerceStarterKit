/* jshint -W099 */
/* global jQuery:false */
/* global angular:false */
(function($, productApp) {

	"use strict";

	productApp.factory("handleCartService", function($resource) {
		return {
		    getItems: function (language, name) {
		        var provider = $resource("/" + language + "/api/cart/getitems/", {}, {
		            query: { method: "GET", isArray: true }
		        });
		        var response = provider.query({ cart: name, _: new Date().getTime().toString() });
		        return response;
		    },

		    getCart: function (language, name) {
		        var provider = $resource("/" + language + "/api/cart/getCart/", {}, {
                    query: { method: "GET" }
                });
                var response = provider.query({ cart: name, _: new Date().getTime().toString() });
                return response;
            },

		    update: function (language, product, name) {
		        var provider = $resource("/" + language + "/api/cart/update/", {}, {
					Update: { method: "POST" , params: { cartName: name }}
				});
				var response = provider.Update(product);
				return response;
			},

		    remove: function (language, product, name) {
		        var provider = $resource("/" + language + "/api/cart/remove/", {}, {
					Remove: { method: "POST" , params:{ cartName: name } }
				});
				var response = provider.Remove(product);
				return response;
			},

		    empty: function (language, name) {
		        var provider = $resource("/" + language + "/api/cart/empty/", {}, {
					Empty: { method: "POST", params: {name:name} }
				});
				var response = provider.Empty();
				return response;
			},

			addToCart: function (language, product) {
				//console.log("Adding product " + product.code);
			    var provider = $resource("/" + language + "/api/cart/AddToCart/", {}, {
					Add:{method:"POST", params:{}}
				});
				var response = provider.Add(product);
				return response;
			},

			addToWishlist: function (language, product) {
				//console.log("Adding product " + product.code);
			    var provider = $resource("/" + language + "/api/cart/AddToWishlist/", {}, {
					Add: { method: "POST", params: {} }
				});
				var response = provider.Add(product,
					function () {
						//console.log(response);
					},
					function () {
						//console.log(response);
					});
				return response;
			},

			moveToWishlist: function (language, product) {
			    var provider = $resource("/" + language + "/api/cart/MoveToWishlist/", {}, {
					Move: { method: "POST", params: {} }
				});
				var response = provider.Move(product);
				return response;
			},


			moveAllToCart: function (language, name) {
			    var provider = $resource("/" + language + "/api/cart/MoveAllToCart/", {}, {
					Move: { method: "POST", params: {name:name} }
				});
				var response = provider.Move();
				return response;
			},

			moveToCart: function (language, product) {
			    var provider = $resource("/" + language + "/api/cart/MoveToCart/", {}, {
					Move: { method: "POST", params: {} }
				});
				var response = provider.Move(product);
				return response;
			},


			addDiscountCode: function (language, code) {
			    var provider = $resource("/" + language + "/api/cart/AddDiscountCode/", {}, {
					Add: { method: "POST", params: { code: code } }
				});

				var response = provider.Add();
				return response;
			}
		};
	});


})(jQuery, window.productApp);