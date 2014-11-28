/* jshint -W099 */
/* global jQuery:false */
(function($, Oxx, commercestarterkit) {

	"use strict";


//********************************************************************************
//*NAMESPACES ********************************************************************
//********************************************************************************

	commercestarterkit = window.commercestarterkit = (!commercestarterkit) ? {} : commercestarterkit;
	commercestarterkit.WebComponent = (!commercestarterkit.WebComponent) ? {} : commercestarterkit.WebComponent;

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
	 * @param {jQuery} $el
	 * @param {object} options
	 * @constructor
	 */
	commercestarterkit.WebComponent.CheckboxListFilter = function($el, options) {

		/** @type {Object} */
		this._settings = {};

		/** @type {Object} */
		this._settings.options = $.extend({}, defaultOptions, options);

		/** @type {jQuery} */
		this._$el = $el;
		/** @type {jQuery} */
		this._$all = this._$el.find('input[type=checkbox].all');
		/** @type {jQuery} */
		this._$checkboxes = this._$el.find('input').not('input.all');

		this.init();
	};


//********************************************************************************
//*PROTOTYPE/PUBLIC FUNCTIONS*****************************************************
//********************************************************************************
	commercestarterkit.WebComponent.CheckboxListFilter.prototype = {

		constructor: commercestarterkit.WebComponent.CheckboxListFilter,

		/**
		 * Init the checkbox list
		 */
		init: function() {

			this._$all.on('change', $.proxy(this.onAllChanged, this));
			this._$checkboxes.on('change', $.proxy(this.onCheckboxChanged, this));
		},

//********************************************************************************

		getValues: function() {
			var output = [];

			if(!this._$all.is(':checked') && this._$checkboxes.filter(':checked').length !== 0) {
				this._$checkboxes.filter(':checked').each(function() {
					output.push($(this).val());
				});
			}
			return output;
		},

//********************************************************************************
//*PRIVATE OBJECT METHODS ********************************************************
//********************************************************************************

//********************************************************************************
//*CALLBACK METHODS **************************************************************
//********************************************************************************

//********************************************************************************
//*EVENT METHODS******************************************************************
//********************************************************************************
		/**
		 * Event for when a checkbox is changed
		 *
		 * @param event
		 */
		onCheckboxChanged: function(event) {
			// turn off the all checkbox
			this._$all.prop('checked', false);

			// if all checkboxes are false
			if(this._$checkboxes.filter(':checked').length === 0) {
				// turn on the all checkbox
				this._$all.prop('checked', true);
			}

			// send change event
			this._$el.trigger('filterChanged');
		},

//********************************************************************************

		/**
		 * Event for when the all checkbox is changed
		 *
		 * @param event
		 */
		onAllChanged: function(event) {

			// if the all checkbox is triggered off, turn it on again
			if(!this._$all.is(':checked')) {
				this._$all.prop('checked', true);
			} else {
				// turn off all other checkboxes
				this._$checkboxes.prop('checked', false);

				// send change event
				this._$el.trigger('filterChanged');
			}
		}

	};

})(jQuery, window.Oxx, window.commercestarterkit);