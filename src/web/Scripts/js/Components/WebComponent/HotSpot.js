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
	 * 		click - click on a product link
	 *
	 * @param {jQuery} $el
	 * @param {jQuery} $image
	 * @param {jQuery} $areaContainer
	 * @param {object} [options]
	 * @constructor
	 */
	commercestarterkit.WebComponent.HotSpot = function($el, $image, $areaContainer, options) {
		/** @type {Object} */
		this._settings = {
			hotSpot: {
				top: 0,
				left: 0,
				width: 0,
				height: 0
			},
			area: {
				top: 0,
				left: 0,
				width: 0,
				height: 0
			}
		};

		/** @type {Object} */
		this._settings.options = $.extend({}, defaultOptions, options);

		/** @type {jQuery} */
		this._$el = $el;

		/** @type {jQuery} */
		this._$areaContainer = $areaContainer;

		/** @type {jQuery} */
		this._$image = $image;

		/** @type {string} */
		this._id = this._$el.attr('id');

		/** @type {string} */
		this._productId = this._$el.data('product-id');

		this.init();
	};



//********************************************************************************
//*PROTOTYPE/PUBLIC FUNCTIONS*****************************************************
//********************************************************************************
	commercestarterkit.WebComponent.HotSpot.prototype = {

		constructor: commercestarterkit.WebComponent.HotSpot,
        
		/**
		 * Init the hot spot
		 */
		init: function () {
			this._readConfig();

			this._createAreaDOM();

			this.redraw();

			this._attachEvents();


		},

//********************************************************************************

		/**
		 * Update the HotSpot coordinates
		 */
		redraw: function() {
			this._updateHotSpotCoordinates();
			this._updateAreaCoordinates();
		},

//********************************************************************************

		/**
		 * Return the product id on the link
		 */
		getProductId: function() {
			return this._productId;
		},

//********************************************************************************
//*PRIVATE OBJECT METHODS ********************************************************
//********************************************************************************

		/**
		* Attach events
		 *
		* @private
		*/
		_attachEvents: function() {
			var $container = this._$el.parents('.hotspotimage'),
				targets = 'area#a' + this._id + ', .hotspot#' + this._id;


			$container.on('mouseenter', targets,
				$.proxy(this._onMouseEnterHotSpotArea, this));

			$container.on('mouseleave', targets,
				$.proxy(this._onMouseLeaveHotSpotArea, this));


			if(this._productId) {
				this._$el.on('click', $.proxy(this._onLinkClick, this));
			}

		},

//********************************************************************************

		/**
		 * Read the configuration from the data attributes
		 * @private
		 */
		_readConfig: function() {
			this._settings.hotSpot = {
				top: parseFloat(this._$el.data('top')),
				left: parseFloat(this._$el.data('left')),
				width: parseInt(this._$el.data('width'), 10),
				height: parseInt(this._$el.data('height'), 10)
			};
			this._settings.area = {
				top: parseFloat(this._$el.data('area-top')),
				left: parseFloat(this._$el.data('area-left')),
				width: parseFloat(this._$el.data('area-width')),
				height: parseFloat(this._$el.data('area-height'))
			};
		},

//********************************************************************************

		/**
		 * Update the position of the HotSpot
		 *
		 * @private
		 */
		_updateHotSpotCoordinates: function() {
			var percentToPixels = commercestarterkit.WebComponent.HotSpotImage.percentToPixels,
				top = this._settings.hotSpot.top,
				offsetTop = parseInt(this._settings.hotSpot.height / 2, 10),
				left = this._settings.hotSpot.left,
				offsetLeft = parseInt(this._settings.hotSpot.width / 2, 10),
				imageWidth = this._$image.width(),
				imageHeight = this._$image.height();


			this._$el.css('top', (percentToPixels(top+'', imageHeight) - offsetTop) + 'px');
			this._$el.css('left', (percentToPixels(left+'', imageWidth) - offsetLeft) + 'px');
		},

//********************************************************************************

		/**
		 * Create the DOM element for the area
		 *
		 * @private
		 */
		_createAreaDOM: function () {
			this._$area = $('<area shape="rect" alt="" href="javascript:;" coords="1,1,1,1" />');
			this._$areaContainer.append(this._$area);
			this._$area.attr('id', 'a' + this._id);
			this._updateAreaCoordinates();
		},

//********************************************************************************

		/**
		 * Update the position of the area
		 *
		 * @private
		 */
		_updateAreaCoordinates: function() {
			//window.console.log(this._settings.area);
			var percentToPixels = commercestarterkit.WebComponent.HotSpotImage.percentToPixels,
				y1 = this._settings.area.top,
				y2 = y1 + this._settings.area.height,
				x1 = this._settings.area.left,
				x2 = x1 + this._settings.area.width,
				imageWidth = this._$image.width(),
				imageHeight = this._$image.height();

			this._$area.attr('coords',
				percentToPixels(x1+'', imageWidth) + ',' +
				percentToPixels(y1+'', imageHeight) + ',' +
				percentToPixels(x2+'', imageWidth) + ',' +
				percentToPixels(y2+'', imageHeight));
		},

//********************************************************************************
//*CALLBACK METHODS **************************************************************
//********************************************************************************


//********************************************************************************
//*EVENT METHODS******************************************************************
//********************************************************************************

		/**
		 * Click on a product link
		 *
		 * @param event
		 * @private
		 */
		_onLinkClick: function(event) {
			event.preventDefault();
			event.stopPropagation();

			$(this).trigger('click');
		},

//********************************************************************************

		/**
		 * Event for when the mouse enters the area or the HotSpot
		 *
		 * @param event
		 */
		_onMouseEnterHotSpotArea: function(event) {
			if (this._activeSpotTimer) {
				clearTimeout(this._activeSpotTimer);
			}
			this._$el.show();
		},

//********************************************************************************

		/**
		 * Event for when the mouse leaves the area or the HotSpot
		 *
		 * @param event
		 */
		_onMouseLeaveHotSpotArea: function(event) {
			var $el = this._$el;
			this._activeSpotTimer = setTimeout(function () {
				$el.fadeOut(200);
			}, 300);
		}


};

})(jQuery, window.Oxx, window.commercestarterkit);