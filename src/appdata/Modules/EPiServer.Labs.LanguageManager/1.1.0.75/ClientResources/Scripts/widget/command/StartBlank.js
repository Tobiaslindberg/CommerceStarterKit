define([
// Dojo
    "dojo/_base/declare",
    "dojo/_base/lang",

// EPi Framework
    "epi/shell/command/_Command",

// Resource
    "epi/i18n!epi/cms/nls/languagemanager.gadget"

], function (
// Dojo
    declare,
    lang,

// EPi Framework
    _Command,

// Resource
    res
) {

    // module:
    //      "epi-languagemanager/widget/command/StartBlank"
    // summary:
    //      Used for selecting create language branch options for a page

    return declare([_Command], {

        label: res.createlanguagebranch,

        constructor: function (params) {

            this.set("isAvailable", true);
            this.set("canExecute", true);
        },

        _execute: function () {
            // summary:
            //		Executes this command assuming canExecute has been checked. Subclasses should override this method.
            // tags:
            //		protected

            this.model.startWithBlank();
        }
    });
});