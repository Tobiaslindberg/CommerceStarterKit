define([
// Dojo base
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/Deferred",
    "dojo/dom-style",

// Epi Framework
    "epi/dependency",
    "epi/shell/widget/dialog/Dialog",

// Language manager
    "epi-languagemanager/component/command/CommandBase",

// Resource
    "epi/i18n!epi/cms/nls/languagemanager.gadget"

], function (
// Dojo base
    declare,
    lang,
    Deferred,
    domStyle,

// Epi Framework
    dependency,
    Dialog,

// Language manager
    CommandBase,

// Resource
    res

    ) {


    return declare([CommandBase], {

        // label: [public] String
        //		The action text of the command to be used in visual elements.
        label: "",

        // category: [readonly] String
        //		A category which provides a hint about how the command could be displayed.
        category: "context",

        postscript: function () {
            this.inherited(arguments);

            this.set("canExecute", false);
        },

        _onModelChange: function () {
            // summary:
            //		Updates canExecute and isAvailable after the model has been updated.
            // tags:
            //		protected

            var item = this.model.currentItemData;
            if (!item) {
                this.set("canExecute", false);
                this.set("isAvailable", false);
                return;
            }
            var isPage = this.isPage(this.model.context),
                isWastebasket = this.model.contentData.isWastebasket,
                isRoot = this.model.context.customViewType == "epi.cms.pagedata.root";

            this.set("canExecute",
                item.canActivate
                && isPage
                && !isWastebasket
                && !isRoot
            );
            this.set("isAvailable", item.canActivate);

            var resourceKey = item.isActive ? "disableediting" : "enableediting";
            this.set("label", lang.replace(res[resourceKey], item));
        },

        _execute: function () {
            // summary:
            // tags:
            //		protected

            this.model.toggleLanguageBranchActivation();
        }
    });
});
