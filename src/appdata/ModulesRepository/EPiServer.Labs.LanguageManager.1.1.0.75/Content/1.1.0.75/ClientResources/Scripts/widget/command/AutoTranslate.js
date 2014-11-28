define([
// Dojo
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/dom-style",

// Dojox
    "dojox/html/entities",

// EPi Framework
    "epi/shell/command/_Command",
    "epi/shell/widget/dialog/Dialog",

// Resource
    "epi/i18n!epi/cms/nls/languagemanager.gadget"

], function (
// Dojo
    declare,
    lang,
    domStyle,

// Dojox
    entities,

// EPi Framework
    _Command,
    Dialog,

// Resource
    res

) {

    // module:
    //      "epi-languagemanager/widget/command/AutoTranslate"
    // summary:
    //      Used for selecting create language branch options for a page

    return declare([_Command], {

        label: "Auto-translate from {language}",

        constructor: function (params) {
            if (params && params.model) {
                this.label = lang.replace(res.translateandcopyfrommaster, { name: params.model.masterLanguage });
            }
            this.set("isAvailable", true);
            this.set("canExecute", true);
        },

        _onModelChange: function () {

            this.label = lang.replace(res.translateandcopyfrommaster, { name: this.model.masterLanguage });
        },

        execute: function (isCreatingNew) {
            this._execute(isCreatingNew);
        },

        _execute: function (isCreatingNew) {
            // summary:
            //		Executes this command assuming canExecute has been checked. Subclasses should override this method.
            // tags:
            //		protected

            // creating a new language branch with auto translate from master
            if (isCreatingNew === true) {
                this.model.autoTranslate();
                return;
            }

            // return if there are no existing language branches, thus there is not the master language branch
            var existingLanguageBranches = this.model.contentData.existingLanguageBranches;
            if (existingLanguageBranches.length == 0)
            {
                return;
            }

            // replace existing language branch with auto translate from master
            var masterLanguageName = existingLanguageBranches.filter(function (languageBranch) {
                return languageBranch.isMasterLanguage;
            });

            // Workaround to get the master language.
            if (masterLanguageName.length == 0) {
                masterLanguageName = existingLanguageBranches;
            }

            var dialog = new Dialog({
                destroyOnHide: true,
                dialogClass: "epi-dialog-confirm",
                title: res.replacecontent,
                description: lang.replace(res.autotranslateconfirmation,
                    {
                        pageName: entities.encode(this.model.contentData.name),
                        toLanguage: this.model.currentItemData.name,
                        fromLanguage: masterLanguageName[0].name
                    }),
                confirmActionText: res.replacecontent
            });
            domStyle.set(dialog.domNode, { width: "450px" });

            dialog.connect(dialog, "onExecute", lang.hitch(this, function () {
                this.model.autoTranslate();
            }));

            dialog.show();
        }
    });
});