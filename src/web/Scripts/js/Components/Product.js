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
	var defaultOptions = {

	};


//********************************************************************************
//*CONSTRUCTOR********************************************************************
//********************************************************************************
	/**
	 *
	 * @param containerId
	 * @param options
	 * @constructor
	 */
	commercestarterkit.Product = function(containerId, options) {

		/** @type {jQuery} */
		this._$el = $(containerId);

		/** @type {Object} */
		this._settings = {};

		/** @type {Object} */
		this._settings.options = Oxx.ObjectUtils.createOptionsObject(defaultOptions, options);

		/** @type {jQuery} */
		this._$sizes = this._$el.find('.sizes select');

		/** @type {jQuery} */
		this._$mainProductImageSlider = this._$el.find('.main-product-image-slider');

		/** @type {jQuery} */
		this._$mainProductView = this._$el.find('.main-product-view');

		/** @type {jQuery} */
		this._$mainProductFullScreen = this._$el.find('.full-screen');

		/** @type {jQuery} */
		this._$wearItWithProducts = this._$el.find('.wear-it-with-products');

		/** @type {jQuery} */
		this._$lightbox = $('#lightbox');

		/** @type {jQuery} */
		this._$sizeguide = this._$el.find('.sizeguide');

		/** @type {string} */
		this.wearItWithProducts = 'vertical';

		/** @type {int} */
		this.wearItBreakPoint = 992;

		/** @type {boolean} */
		this.showZoomFlyout = false;

		this._init();
	};
//********************************************************************************
//*PROTOTYPE/PUBLIC FUNCTIONS*****************************************************
//********************************************************************************
	commercestarterkit.Product.prototype = {
		constructor: commercestarterkit.Product,

		/**
		 * Init the product view
		 *
		 * @private
		 */
		_init: function() {

			this._initMainProductImage();
			setTimeout($.proxy(this._initProductImages, this), 300);
			setTimeout($.proxy(this._initWearItWithProducts, this), 300);


			// attach events
			this._$sizes.on('change', $.proxy(this._onSizeChanged, this));
			this._$mainProductFullScreen.on('click', $.proxy(this._onMainProductFullScreenClick, this));
			this._$lightbox.find('a.close').on('click', $.proxy(this._onLightBoxCloseClick, this));
			$(window).on('resize', $.proxy(this._onWindowResized, this));


			this._$sizeguide.on('click', $.proxy(this._onSizeGuideClick, this));
		},


		/**
		 * Get the viewport width and height
		 * (better than $(window).width() because of the scrollbar
		 *
		 * @returns {{width: *, height: *}}
		 */
		viewport: function() {
			var e = window, a = 'inner';
			if (!('innerWidth' in window )) {
				a = 'client';
				e = document.documentElement || document.body;
			}
			return { width : e[ a+'Width' ] , height : e[ a+'Height' ] };
		},

//********************************************************************************
//*PRIVATE OBJECT METHODS ********************************************************
//********************************************************************************



	    /**
		 * Set up the main product image with slider, zoom and full screen
		 *
		 * @private
		 */
		_initMainProductImage: function() {
			var $slider = this._$mainProductImageSlider.find('.slider');
			$slider.flexslider({
				//animation: 'slide',
				controlNav: false,
				slideshow: false,
				prevText: '',
				nextText: '',
				animationLoop: false,
				start: $.proxy(this._mainProductImageFlexSliderStart, this)
			});

			$slider.find('img').css('max-width', '100%');

			$('.easyzoom').easyZoom();
		},

//********************************************************************************

		_mainProductImageFlexSliderStart: function() {
			//commercestarterkit.equalHeight('#' + $slider.attr('id') + ' .slides li');
		},

//********************************************************************************

		/**
		 * Set up the main product image thumbnails with a slider
		 *
		 * @private
		 */
		_initProductImages: function() {
			this._$el.find('.product-image-slider .slider').flexslider({
				animation: 'slide',
				controlNav: false,
				slideshow: false,
				prevText: '',
				nextText: '',
				itemWidth: 85,
				maxItems: 5
			});
			this._$el.find('.product-image-slider .slides li:not(.clone) a').on('click', $.proxy(this._onSlideImageClicked, this));
		},

		/**
		 *
		 * @private
		 */
		_updateProductImagesHeight: function() {

			this._$mainProductImageSlider.find('.slides li').each(function(index, item) {
				var $li = $(item),
					$img = $li.find('img');

				$li.css('height', ($img.height() + 41) + 'px');
			});
		},

//********************************************************************************

		/**
		 * Set up the related products and the "wear it with" product sliders
		 *
		 * @private
		 */
		_initWearItWithProducts: function() {

			this._setWearItWithOrientation();

			this._reCreateWearItWithSlider(true);

		},

//********************************************************************************

		/**
		 * Based on the window size, guess the slider height
		 *
		 * @private
		 */
		_setWearItWithOrientation: function() {
			var width = this.viewport().width;

			if(width >= this.wearItBreakPoint) {
				this.wearItWithProducts = 'vertical';
			} else {
				this.wearItWithProducts = 'horizontal';
			}
		},

//********************************************************************************

		/**
		 * Set the height of the slider to be the same as the product images
		 *
		 * @private
		 */
		_setWearItWithHeight: function() {
			var mainProductImageContainer = this._$mainProductImageSlider.parent(),
				wearItWithProductsBox = this._$wearItWithProducts.find('.box');

				setTimeout(function() {
					var mainProductImageContainerHeight = mainProductImageContainer.height() - 20;
					if(wearItWithProductsBox.height() < mainProductImageContainerHeight) {
						wearItWithProductsBox.css('height', mainProductImageContainerHeight + 'px');
					}
				}, 200);
		},

//********************************************************************************

		/**
		 * Init the slider
		 *
		 * @param {bool} [forceIt]
		 * @private
		 */
		_reCreateWearItWithSlider: function(forceIt) {
			var width = this.viewport().width,
				$slider = this._$wearItWithProducts.find('.slider');

			if(width >= this.wearItBreakPoint && (this.wearItWithProducts === 'horizontal' || forceIt)) {

				this.wearItWithProducts = 'vertical';
				this._replaceSlider($slider, {
					animation: 'slide',
					direction: 'vertical',
					controlNav: false,
					slideshow: false,
					prevText: '',
					nextText: '',
					maxItems: 3,
					animationLoop: false
				});

			} else if(width < this.wearItBreakPoint && (this.wearItWithProducts === 'vertical' || forceIt)) {

				this.wearItWithProducts = 'horizontal';
				this._replaceSlider($slider, {
					animation: 'slide',
					controlNav: false,
					slideshow: false,
					prevText: '',
					nextText: '',
					itemWidth: 120,
					maxItems: 6,
					animationLoop: false
				});
			}

			if(this.wearItWithProducts === 'vertical') {
				if(this._setWearItWithHeightTimer) {
					clearTimeout(this._setWearItWithHeightTimer);
				}
				this._setWearItWithHeightTimer = setTimeout($.proxy(this._setWearItWithHeight, this), 200);
			} else {
				this._$wearItWithProducts.find('.box').css('height', 'auto');
			}
		},

//********************************************************************************

		/**
		 * Recreate the slider element with different options
		 *
		 * @param {jQuery} $element
		 * @param {object} options
		 * @private
		 */
		_replaceSlider: function($element, options) {
			// create new container
			$element.before('<div class="slider"><ul class="slides"></ul></div>');
			var $newElement = $element.prev(),
				$slides = $newElement.find('.slides');

			// find the images and move to new container
			var $products = $element.find('.slides li:not(.clone) .product');

			if(options.direction === 'vertical') {
				// group 3 and 3 images
				var $groupLi = $('<li>'),
					isGrouped = false;
				for(var i = 0; i < $products.length; i++) {
					isGrouped = false;
					var $product = $($products[i]);
					$product.detach().appendTo($groupLi);
					if(i % options.maxItems === (options.maxItems - 1)) {
						$slides.append($groupLi);
						$groupLi = $('<li>');
						isGrouped = true;
					}
				}
				// handle last group
				if(!isGrouped) {
					$slides.append($groupLi);
				}
			} else {
				$products.each(function() {
					var $product = $(this),
						$li = $('<li>');
					$product.appendTo($li);
					$slides.append($li);
				});
			}


			// remove old container and slider
			$element.remove();

			// create new slider
			$newElement.flexslider(options);
		},

//********************************************************************************

		/**
		 * Open the size guide dialog
		 *
		 * @param {int} reference
		 * @private
		 */
		_openSizeGuide: function(reference) {
			var $sizeGuideDialog = new commercestarterkit.SizeGuideDialog(reference);
		},

//********************************************************************************
//*EVENT METHODS******************************************************************
//********************************************************************************

		/**
		 * When user has changed product size
		 *
		 * @param event
		 * @private
		 */
		_onSizeChanged: function(event) {
			this._$sizes.parents('form').trigger('submit');
		},


//********************************************************************************

		/**
		 * When size guide button is clicked
		 *
		 * @param event
		 * @private
		 */
		_onSizeGuideClick: function(event) {
			event.preventDefault();
			event.stopPropagation();

			var $btn = $(event.currentTarget),
				reference = $btn.data('reference');

			if(reference > 0) {
				this._openSizeGuide(reference);
			}
		},

//********************************************************************************

		/**
		 *
		 * @param event
		 * @private
		 */
		_onSliderSwipeLeft: function(event) {
			var $slider = $(event.target).parents('.slider');
			$slider.flexslider('prev');

		},

//********************************************************************************

		/**
		 *
		 * @param event
		 * @private
		 */
		_onSliderSwipeRight: function(event) {
			var $slider = $(event.target).parents('.slider');
			$slider.flexslider('next');
		},

//********************************************************************************

		/**
		 * Opens main product image in full screen
		 *
		 * @private
		 */
		_onMainProductFullScreenClick: function() {
			var imageUrl = this._$mainProductImageSlider.find('.flex-active-slide a').attr('href');
			this._$lightbox.find('img').attr('src', imageUrl).css('max-width', '100%');
		    this._$lightbox.css('z-index', '1041');
			this._$lightbox.lightbox();//.removeClass('hide');


		},

//********************************************************************************

		/**
		 * Sets the main product image slide index
		 *
		 * @param event
		 * @private
		 */
		_onSlideImageClicked: function(event) {
			var $slide = $(event.target).parents('li'),
				index = $slide.index();

			this._$mainProductImageSlider.find('.slider').flexslider(index);
		},

//********************************************************************************

		/**
		 * Change the wear it with product slider when the window size change
		 *
		 * @param event
		 * @private
		 */
		_onWindowResized: function(event) {
			this._reCreateWearItWithSlider();
			this._updateProductImagesHeight();
		},


//********************************************************************************
		/**
		 * Event for closing the fullscreen image lightbox
		 *
		 * @private
		 */
		_onLightBoxCloseClick: function(event) {
			event.preventDefault();
			event.stopPropagation();
			this._$lightbox.lightbox('hide');
		}

//********************************************************************************

	};


})(jQuery, window.Oxx, window.commercestarterkit);

