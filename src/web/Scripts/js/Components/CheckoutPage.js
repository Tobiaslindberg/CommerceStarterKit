/* jshint -W099 */
/* global jQuery:false */

(function($, Oxx, commercestarterkit){

	"use strict";

//********************************************************************************
//*NAMESPACES ********************************************************************
//********************************************************************************
	commercestarterkit = window.commercestarterkit = (!commercestarterkit) ? {} : commercestarterkit;

//********************************************************************************
//*CLASS VARIABLES****************************************************************
//********************************************************************************

//********************************************************************************
//*CONSTRUCTOR********************************************************************
//********************************************************************************
	commercestarterkit.CheckoutPage = {


//********************************************************************************
//*PROTOTYPE/PUBLIC FUNCTIONS*****************************************************
//********************************************************************************

		/**
		 * Init the checkout page view
		 * @param {string} id
		 */
		init: function(id) {

			/** @var {jQuery} */
			this._$el = $((id || '#checkoutpage'));

			if(this._$el.length === 0) {
				return;
			}

			/** @type {jQuery} */
			this._$submitButton = this._$el.find('input[type=submit]');

			/** @type {jQuery} */
			this._$termsCheckBox = this._$el.find('.terms input[type=checkbox]');

			/** @type {jQuery} */
			this._$termsError = this._$el.find('.terms-error');

			/** @type {jQuery} */
			this._$termsLink = this._$el.find('.terms a[data-reference]');


			this._initTermsLink();


			this._$submitButton.on('click', $.proxy(this._onSubmit, this));
			this._$termsCheckBox.on('change', $.proxy(this._onTermsCheckBoxChange, this));
		},

//********************************************************************************
//*PRIVATE OBJECT METHODS ********************************************************
//********************************************************************************

		/**
		 * Set up the link on the terms and conditions
		 * @private
		 */
		_initTermsLink: function() {
			if(this._$termsLink.length > 0) {
				this._$termsLink.on('click', $.proxy(this._onTermsClick, this));
			}
		},

//********************************************************************************
//*CALLBACK METHODS **************************************************************
//********************************************************************************


//********************************************************************************
//*EVENT METHODS******************************************************************
//********************************************************************************

		/**
		 * Click on the terms link
		 *
		 * @param event
		 * @private
		 */
		_onTermsClick: function(event) {
			var $btn = $(event.currentTarget);
			commercestarterkit.openArticleReference($btn);
		},

//********************************************************************************

		/**
		 *
		 * @param event
		 * @private
		 */
		_onTermsCheckBoxChange: function(event) {
			this._$termsError.toggle(!this._$termsCheckBox.is(':checked'));
		},

//********************************************************************************

		/**
		 *
		 * @param event
		 * @private
		 */
		_onSubmit: function(event) {

			if(!this._$termsCheckBox.is(':checked')) {
				event.preventDefault();
				event.stopPropagation();
				this._$termsError.show();
				return false;
			}

		}

	};


})(jQuery, window.Oxx, window.commercestarterkit);

