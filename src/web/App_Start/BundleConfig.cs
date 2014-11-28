/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System.Web.Optimization;

namespace OxxCommerceStarterKit.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/libraries/jquery-2.1.0.min.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
						"~/Scripts/libraries/jquery-ui-1.10.4.custom.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
						"~/Scripts/libraries/jquery.validate.min.js",
						"~/Scripts/libraries/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/libraries/jquery.unobtrusive-ajax.min.js"
						));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			//bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
			//			"~/Scripts/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
				"~/Scripts/libraries/bootstrap.min.js",
				"~/Scripts/libraries/bootstrap-lightbox.js"));

			bundles.Add(new ScriptBundle("~/bundles/frontpage").Include(
				"~/Scripts/js/components/FrontPage.js"));

			bundles.Add(new ScriptBundle("~/bundles/checkoutpage").Include(
				"~/Scripts/js/components/CheckoutPage.js",
				"~/Scripts/js/components/Registration.js"));

			bundles.Add(new ScriptBundle("~/bundles/registration").Include(
				"~/Scripts/js/components/Registration.js"));

			bundles.Add(new ScriptBundle("~/bundles/general").Include(
				"~/Scripts/libraries/easyzoom.js",
				"~/Scripts/libraries/jquery.flexslider-min.js",
				"~/Scripts/libraries/jquery.touchSwipe.min.js",
                "~/Scripts/libraries/accounting.min.js", // http://openexchangerates.github.io/accounting.js
				"~/Scripts/js/Oxx/ObjectUtils.js",
				"~/Scripts/js/Oxx/AjaxUtils.js",
				"~/Scripts/js/components.js",
                "~/Scripts/js/starterkit.js",
				"~/Scripts/js/components/Product.js",
				"~/Scripts/js/components/WebComponent/HotSpotImage.js",
				"~/Scripts/js/components/WebComponent/HotSpot.js",
				"~/Scripts/js/components/ProductDialog.js",
				"~/Scripts/js/components/SizeGuideDialog.js",
				"~/Scripts/js/components/HelpDialog.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
				"~/Content/css/jqueryui/jquery-ui-1.10.4.custom.min.css",
				"~/Content/bootstrap.min.css",
				"~/Content/easyzoom.css",
				"~/Content/css/flexslider/flexslider.css",
                "~/Content/css/commerce-starter-kit.css"));

            bundles.Add(new ScriptBundle("~/bundles/angular_app").IncludeDirectory(
                "~/Scripts/app/", "*.js",true
                ));
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
               "~/Scripts/libraries/angular.js",
               "~/Scripts/libraries/angular-resource.js",
               "~/Scripts/libraries/ui-bootstrap-tpls-0.10.0.js"
               ));
        }
    }
}
