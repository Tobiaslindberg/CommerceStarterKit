/* jshint -W099 */
/* global jQuery:false */
/* global angular:false */
(function($, Oxx, commercestarterkit) {

	"use strict";


//********************************************************************************
//*NAMESPACES ********************************************************************
//********************************************************************************

	commercestarterkit = window.commercestarterkit = (!commercestarterkit) ? {} : commercestarterkit;

//********************************************************************************
//*STATIC CLASS VARIABLES*********************************************************
//********************************************************************************
	var defaultOptions = {

	};

//********************************************************************************
//*CONSTRUCTOR********************************************************************
//********************************************************************************

	/**
	 *
	 * @param {int} productId
	 * @param {object} [options]
	 * @constructor
	 */
	commercestarterkit.ProductDialog = function(productId, options) {

		/** @type {Object} */
		this._settings = {};

		/** @type {Object} */
		this._settings.options = $.extend({}, defaultOptions, options);

		/** @type {int} */
		this._productId = productId;

		/** @type {commercestarterkit.Product} */
		this._product = undefined;

		/** @type {Object} */
		this._$el = $('#productDialog');

		/** @type {Object} */
		this._$closeButton  = this._$el.find('a.close');

		/** @type {Object} */
		this._$content = this._$el.find('.product-page');


		this.init();
	};


//********************************************************************************
//*PROTOTYPE/PUBLIC FUNCTIONS*****************************************************
//********************************************************************************
	commercestarterkit.ProductDialog.prototype = {

		constructor: commercestarterkit.ProductDialog,

		/**
		 * Init the product dialog
		 */
		init: function() {

			commercestarterkit.showFullScreenLoader();

			this._$closeButton.on('click', $.proxy(this.onClose, this));

			this._loadDialogContent();
		},


//********************************************************************************
//*PRIVATE OBJECT METHODS ********************************************************
//********************************************************************************

		/**
		 * Load the dialog content with ajax
		 *
		 * @private
		 */
		_loadDialogContent: function() {
			Oxx.AjaxUtils.ajax(Oxx.AjaxUtils.url('FashionProductContent', 'Product'), {
				productId: this._productId
			}, $.proxy(this._dialogContentLoadedCallback, this), 'post', 'html');
		},

//********************************************************************************
//*CALLBACK METHODS **************************************************************
//********************************************************************************

		/**
		 *
		 * @param response
		 * @private
		 */
		_dialogContentLoadedCallback: function(response) {

			commercestarterkit.hideFullScreenLoader();

			this._$content.html(response);
			var me = this;

			$('body').injector().invoke(function($compile, $rootScope) {
				$compile(me._$content.find('> div'))($rootScope);
				$rootScope.$apply();
			});


			this._$content.parent().parent().css('width', this._$el.parent().width() + 'px').css('max-width', '95%');

			this._$el.modal({
				keyboard: false
			});

		},


//********************************************************************************
//*EVENT METHODS******************************************************************
//********************************************************************************

		/**
		 * Hit close button, closes the dialog
		 * @param event
		 */
		onClose: function(event) {
			event.preventDefault();
			event.stopPropagation();

			this._$el.modal('hide');
		}


	};

})(jQuery, window.Oxx, window.commercestarterkit);