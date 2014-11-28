define([
// Dojo base
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/Deferred",
    "dojo/dom-style",

// Dojox
    "dojox/html/entities",

// EPi CMS
    "epi-cms/ApplicationSettings",

// Language Manager
    "epi-languagemanager/component/command/CommandBase",

// Resource
    "epi/i18n!epi/cms/nls/languagemanager.gadget"

], function (
// Dojo base
    declare,
    lang,
    Deferred,
    domStyle,

// Dojox
    entities,

// EPi CMS
    ApplicationSettings,

// Language Manager
    CommandBase,

// Resource
    res
) {

    return declare([CommandBase], {

        // label: [public] String
        //		The action text of the command to be used in visual elements.
        label: res.deletelanguagebranch,

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
            var languageContext = this.model.context.languageContext;

            if (!item) {
                this.set("canExecute", false);
                this.set("isAvailable", false);
                return;
            }

            var isPage = this.isPage(this.model.context),
                isBlock = this.isBlock(this.model.context),
                isSupportLanguageContent = languageContext.hasTranslationAccess && languageContext.isPreferredLanguageAvailable,
                isActive = (((isPage || isSupportLanguageContent) && item.isActive) || isBlock) && (this.model.context.capabilities.deleteLanguageBranch),    // block always is active, check deleteLanguageBranch capability.
                isWastebasket = this.model.contentData.isWastebasket,
                isRoot = this.model.context.customViewType == "epi.cms.pagedata.root";               

            this.set("canExecute",
                item.canDelete
                && item.isCreated
                && !item.isMaster
                && isActive
                && !isWastebasket
                && !isRoot
                && !ApplicationSettings.disableVersionDeletion
            );
            this.set("isAvailable", item.canDelete && item.isCreated && !item.isMaster && isActive && !ApplicationSettings.disableVersionDeletion);

            this.set("label", lang.replace(res.deletelanguagebranch, item));
        },

        _execute: function () {
            // summary:
            //		Delete a language branch.
            // tags:
            //		protected

            return this.model.deleteLanguageBranch();
        }
    });
});
