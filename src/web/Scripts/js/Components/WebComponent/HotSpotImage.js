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
	var defaultOptions = {};

//********************************************************************************
//*CONSTRUCTOR********************************************************************
//********************************************************************************

	/**
	 *
	 * Events:
	 * 		click - Click on HotSpot, sends productId as second parameter
	 *
	 * @param {jQuery} $el
	 * @param {object} options
	 * @constructor
	 */
	commercestarterkit.WebComponent.HotSpotImage = function($el, options) {

		/** @type {Object} */
		this._settings = {};

		/** @type {Object} */
		this._settings.options = $.extend({}, defaultOptions, options);

		/** @type {jQuery} */
		this._$el = $el;

		/** @type {jQuery} */
		this._$hotSpots = this._$el.find('.hotspot');

		/** @type {jQuery} */
		this._$areaContainer = this._$el.find('map');

		/** @type {jQuery} */
		this._$image = this._$el.find('img');

		/** @type {number} */
		this._timerWindowResize = undefined;

		/** @type {boolean} */
		this._isMobile = this._$el.data('mobile') === 'True';

		// wait for image
		if(this._$image.height() > 0) {
			// give it some time anyway
			setTimeout($.proxy(this._onImageLoaded, this), 200);
			//this._onImageLoaded();
		} else {
			this._$image.on('load', $.proxy(this._onImageLoaded, this));
		}
	};

//********************************************************************************

	/**
	 * Convert percent position to pixel value
	 *
	 * @param {string} position - top or left position in percent
	 * @param {int} imageSize - width or height of the background image
	 * @return {int} - top or left position in pixels
	 */
	commercestarterkit.WebComponent.HotSpotImage.percentToPixels = function (position, imageSize) {
		var number = parseFloat(position.replace('%', ''));
		return Math.round(number * imageSize / 100);
	};

//********************************************************************************
//*PROTOTYPE/PUBLIC FUNCTIONS*****************************************************
//********************************************************************************
	commercestarterkit.WebComponent.HotSpotImage.prototype = {

		constructor: commercestarterkit.WebComponent.HotSpotImage,

		/**
		 * Init the hot spot image
		 */
		init: function() {
			// the image map coordinates are based on the original image size
			if(this._$image.width() <= 0 || this._$image.height() <= 0) {
				this._$image.on('load', $.proxy(this._lockImageSize, this));
			} else {
				this._lockImageSize();
			}

			this._$hotSpots.each($.proxy(this._iteratorInitEachHotSpot,this));

			this._$image.on('touchmove', $.proxy(this._onImageTouchMove, this));


			// Listen to the window resize event
			$(window).on('resize', $.proxy(this._onWindowResize, this));
		},

//********************************************************************************
//*PRIVATE OBJECT METHODS ********************************************************
//********************************************************************************

		/**
		 * Lock the size of the image
		 *
		 * @private
		 */
		_lockImageSize: function() {
			this._$image.css('width', this._$image.width() + 'px')
				.css('height', this._$image.height() + 'px');
		},

//********************************************************************************
//*CALLBACK METHODS **************************************************************
//********************************************************************************

		/**
		 * Create the HotSpot object
		 *
		 * @param index
		 * @param item
		 * @private
		 */
		_iteratorInitEachHotSpot: function(index, item) {
			var $item = $(item);
			$item.attr('id', 'hotspot' + index);

			item.HotSpot = new commercestarterkit.WebComponent.HotSpot($item, this._$image, this._$areaContainer);

			$(item.HotSpot).on('click', $.proxy(this._onHotSpotClick, this));
		},

//********************************************************************************

		/**
		 * Redraw the HotSpot
		 *
		 * @param index
		 * @param item
		 * @private
		 */
		_iteratorRedrawEachHotSpot: function(index, item) {
			item.HotSpot.redraw();
		},

//********************************************************************************

		/**
		 * Window has changed size, update positions based on the new image size
		 *
		 * @private
		 */
		_callbackWindowResize: function() {

			this._lockImageSize();
			this._$hotSpots.each($.proxy(this._iteratorRedrawEachHotSpot, this));

		},

//********************************************************************************
//*EVENT METHODS******************************************************************
//********************************************************************************

		/**
		 * Wait with init until the image is loaded
		 *
		 * @param event
		 * @private
		 */
		_onImageLoaded: function(event) {
			this.init();
		},

//********************************************************************************


		/**
		 *
		 * @param event
		 * @private
		 */
		_onHotSpotClick: function(event) {
			var hotSpot = event.target;

			$(this).trigger('click', hotSpot.getProductId());
		},

//********************************************************************************

		/**
		 *
		 * @private
		 */
		_onImageTouchMove: function() {
			this._$el.addClass('visible');

		},


//********************************************************************************

		/**
		 * Event for when the window resize is making the Image change size
		 *
		 * @param event
		 */
		_onWindowResize: function(event) {
			if (this._timerWindowResize) {
				clearTimeout(this._timerWindowResize);
			}
			this._$image.css('width', 'auto').css('height', 'auto');

			this._timerWindowResize = setTimeout($.proxy(this._callbackWindowResize, this), 300);
		}


	};

})(jQuery, window.Oxx, window.commercestarterkit);