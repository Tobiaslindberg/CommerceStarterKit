define([
    "dojo/_base/declare",
    "epi-cms/contentediting/ContentActionSupport",
    "epi-cms/contentediting/command/_LegacyDialogCommandBase",

// Resource
    "epi/i18n!epi/cms/nls/languagemanager.gadget"

], function (declare, ContentActionSupport, _LegacyDialogCommandBase, res) {


    return declare([_LegacyDialogCommandBase], {

        // label: [public] String
        //		The action text of the command to be used in visual elements.
        label: res.managewebsitelanguages,

        // category: [readonly] String
        //		A category which provides a hint about how the command could be displayed.
        category: "setting",

        dialogPath: "Admin/EditLanguageBranches.aspx",
        raiseCloseEvent: true,

        postscript: function () {
            this.inherited(arguments);
        },

        getDialogParams: function () {
            // summary:
            //		Override to provide title for the dialog.

            return { dialogTitle: res.managewebsitelanguages };
        },

        _onModelChange: function () {
            // summary:
            //		Updates canExecute and isAvailable after the model has been updated.
            // tags:
            //		protected

            this.set("canExecute", this.model.isInAdminRole);
        }
    });
});
