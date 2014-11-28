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
	 * @param {int} reference - size guide page reference
	 * @param {object} [options]
	 * @constructor
	 */
	commercestarterkit.HelpDialog = function(reference, options) {

		/** @type {Object} */
		this._settings = {};

		/** @type {Object} */
		this._settings.options = $.extend({}, defaultOptions, options);

		/** @type {int} */
		this._reference = reference;

		/** @type {jQuery} */
		this._$el = $('#helpDialog');

		/** @type {jQuery} */
		this._$content = this._$el.find('.modal-body');

		/** @type {Object} */
		this._$closeButton  = this._$el.find('a.close');


		this.init();
	};


//********************************************************************************
//*PROTOTYPE/PUBLIC FUNCTIONS*****************************************************
//********************************************************************************
	commercestarterkit.HelpDialog.prototype = {

		constructor: commercestarterkit.HelpDialog,

		/**
		 * Init the product dialog
		 */
		init: function() {

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
			Oxx.AjaxUtils.ajax(Oxx.AjaxUtils.url('DefaultPage', 'Get'), {
				reference: this._reference
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

			this._$content.html(response);

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