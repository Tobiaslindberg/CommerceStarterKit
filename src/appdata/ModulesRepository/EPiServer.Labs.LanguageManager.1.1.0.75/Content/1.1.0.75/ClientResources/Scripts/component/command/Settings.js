define([
// Dojo base
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/Deferred",

// Epi Framework
    "epi/dependency",

// Language manager
    "epi-languagemanager/component/command/CommandBase",

// Resource
    "epi/i18n!epi/cms/nls/languagemanager.gadget"

], function (
// Dojo base
    declare,
    lang,
    Deferred,

// Epi Framework
    dependency,

// Language manager
    CommandBase,

// Resource
    res

    ) {


    return declare([CommandBase], {

        // label: [public] String
        //		The action text of the command to be used in visual elements.
        label: res.settings,

        // category: [readonly] String
        //		A category which provides a hint about how the command could be displayed.
        category: "setting",

        postscript: function () {
            this.inherited(arguments);

            this.set("canExecute", false);
        },

        _onModelChange: function () {
            // summary:
            //		Updates canExecute and isAvailable after the model has been updated.
            // tags:
            //		protected

            this.set("canExecute", this.model.isInAdminRole);
        },

        _execute: function () {
            // summary:
            //		Navigate to Settings Panel
            // tags:
            //		protected

            window.location = this.model.modulePath + "Setting";
        }
    });
});
