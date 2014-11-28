define([
// Dojo base
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/Deferred",
    "dojo/topic",

// Epi Framework
    "epi/dependency",
    "epi/Url",

    "epi-cms/contentediting/ContentActionSupport",
    "epi-cms/ApplicationSettings",

// Language mangager
    "epi-languagemanager/component/command/CommandBase",
    "epi-languagemanager/component/CompareEditing",

// Resource
    "epi/i18n!epi/cms/nls/languagemanager.gadget"

], function (
// Dojo base
    declare,
    lang,
    Deferred,
    topic,

// Epi Framework
    dependency,
    Url,

// Epi CMS
    ContentActionSupport,
    ApplicationSettings,

// Language manager
    CommandBase,
    CompareEditing,
    
// Resource
    res

    ) {


    return declare([CommandBase], {

        // label: [public] String
        //		The action text of the command to be used in visual elements.
        label: "Compare with {language} (Master)",

        iconClass: 'epi-iconCompare',

        constructor: function (params) {
            this.inherited(arguments);

            if (params && params.model) {
                this.set("label", lang.replace(res.comparewithmasterlanguage, { name: params.model.masterLanguage }));
            }


            this.set("isAvailable", true);
            this.set("canExecute", true);
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
                return;
            }
            var isPage = this.isPage(this.model.context),
                isBlock = this.isBlock(this.model.context),
                isSupportLanguageContent = languageContext.hasTranslationAccess && languageContext.isPreferredLanguageAvailable,
                isActive = ((isPage || isSupportLanguageContent) && item.isActive) || isBlock; // block always is active

            this.set("label", lang.replace(res.comparewithmasterlanguage, { name: this.model.masterLanguage }));

            // TODO: check current user has edit access right explicitly
            this.set("isAvailable", !this.category || item.isCreated && item.canCopy && !item.isMaster && isActive);
            this.set("canExecute", item.canCopy && item.isCreated && !item.isMaster && isActive);
        },

        execute: function (/*Mouse event*/evt, /*Boolean?*/isForced) {
            if (isForced) {
                this._execute(isForced);
                return;
            }
            this.inherited(arguments);
        },

        _execute: function (/*Boolean?*/isForced) {
            // summary:
            //		Navigate to Settings Panel
            // tags:
            //		protected

            if (!this.model.currentItemData || !this.model.currentItemData.languageID){
                return;
            }
            // currentItemData.canCopy = false means that the current user has not Edit access right on the language,
            // and he cannot access compare editing
            if (!this.model.currentItemData.canCopy) {
                // set actionExecuted to remove standby widget
                this.model.set("actionExecuted", {});
                return;
            }

            var hashWrapper = dependency.resolve("epi.shell.HashWrapper"),
                hashFragments = hashWrapper.getHash(),
                currentItemLanguage = this.model.currentItemData.languageID,

                key = "languagemanageraction",
                actionName = "compare";

            if (isForced) {
                topic.publish("/epi/shell/action/changeview", "epi-languagemanager/component/CompareEditing", null, { languageManagerAction: "compare" });

                // Make it fullscreen, unpin left and right panel
                topic.publish("/epi/layout/pinnable/navigation/toggle", false);
                topic.publish("/epi/layout/pinnable/tools/toggle", false);

                return;
            }

            var currentContentLanguage = new Url(window.location.href).query.language,
                shouldReload = currentContentLanguage === currentItemLanguage,
                currentUrl = new Url(window.location.href),
                currentUrlPath = currentUrl.scheme + "://" + currentUrl.authority + currentUrl.path,            
                reloadUrl = lang.replace("{0}&{1}={2}", [currentUrlPath + this.model.currentItemData.linkFragment, key, actionName]);

            // window.location.replace(reloadUrl);
            window.location.assign(reloadUrl);
            if (shouldReload) {
                window.location.reload();
            }
        }
    });
});
