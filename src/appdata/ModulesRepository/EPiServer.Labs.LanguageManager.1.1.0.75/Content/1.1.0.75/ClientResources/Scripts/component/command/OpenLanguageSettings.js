define([
    "dojo/_base/declare",
    "epi-cms/contentediting/ContentActionSupport",
    "epi-cms/contentediting/command/_LegacyDialogCommandBase",

// Resource
    "epi/i18n!epi/cms/nls/languagemanager.gadget"
],

function (declare, ContentActionSupport, _LegacyDialogCommandBase, res) {

    // TODO: listen close dialog event to refresh the gadget.

    return declare([_LegacyDialogCommandBase], {

        // label: [public] String
        //		The action text of the command to be used in visual elements.
        label: res.languagesettings,
        title: res.languagesettings,

        // category: [readonly] String
        //		A category which provides a hint about how the command could be displayed.
        category: "context",
        

        dialogPath: "Edit/LanguageSettings.aspx",
        raiseCloseEvent: true,

        postscript: function () {
            this.inherited(arguments);
        },

        _onModelChange: function () {
            // summary:
            //		Updates canExecute and isAvailable after the model has been updated.
            // tags:
            //		protected

            var contentData = this.model.contentData,
                hasAdminAccess = ContentActionSupport.hasAccess(contentData.accessMask, ContentActionSupport.accessLevel.Administer),
                hasEditAccess = ContentActionSupport.hasAccess(contentData.accessMask, ContentActionSupport.accessLevel.Edit),
                isWastebasket = this.model.contentData.isWastebasket,
                isRoot = this.model.context.customViewType == "epi.cms.pagedata.root";

            this.set("canExecute",
                contentData.capabilities.languageSettings
                && hasAdminAccess
                && hasEditAccess
                && !isWastebasket
                && !isRoot
            );
        }
    });
});
