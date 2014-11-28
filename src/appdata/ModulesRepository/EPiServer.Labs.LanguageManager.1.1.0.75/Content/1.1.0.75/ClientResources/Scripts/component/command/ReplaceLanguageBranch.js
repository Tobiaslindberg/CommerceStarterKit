define([
// Dojo base
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
    "dojo/aspect",

// Dijit
    "dijit/popup",
    "dijit/focus",
    "dijit/Destroyable",

// Epi CMS
    "epi-cms/contentediting/ContentActionSupport",

// Language manager
    "epi-languagemanager/widget/command/DuplicateContent",
    "epi-languagemanager/widget/command/StartBlank",
    "epi-languagemanager/widget/command/AutoTranslate",
    "epi-languagemanager/component/command/CommandBase",

// Resource
    "epi/i18n!epi/cms/nls/languagemanager.gadget"

], function (
// Dojo base
    declare,
    lang,
    array,
    aspect,

// Dijit
    pm,
    focusUtil,
    Destroyable,

// Epi CMS
    ContentActionSupport,

// Language manager
    DuplicateContent,
    StartBlank,
    AutoTranslate,
    CommandBase,

// Resource
    res
) {


    return declare([CommandBase, Destroyable], {

        // label: [public] String
        //		The action text of the command to be used in visual elements.
        label: res.replacecontent,

        category: "context",

        selected: false,

        constructor: function (params) {
           
            if (params) {
                this._setupOptions(params.model);
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

            // Rebuild options commands
            this._setupOptions(this.model);

            var isPage = this.isPage(this.model.context),
                isBlock = this.isBlock(this.model.context),
                isSupportLanguageContent = languageContext.hasTranslationAccess && languageContext.isPreferredLanguageAvailable,
                isActive = ((isPage || isSupportLanguageContent) && item.isActive) || isBlock,    // block always is active
                hasEditAccess = ContentActionSupport.hasAccess(this.model.contentData.accessMask, ContentActionSupport.accessLevel.Edit),
                isWastebasket = this.model.contentData.isWastebasket,
                isRoot = this.model.context.customViewType == "epi.cms.pagedata.root",
                isReadonlyVersion = item.versionStatus === ContentActionSupport.versionStatus.CheckedIn
                                    || item.versionStatus === ContentActionSupport.versionStatus.DelayedPublish;

            this.set("isAvailable",
                item.isCreated
                && !item.isMaster
                && isActive
                && hasEditAccess
                && !isWastebasket
                && !isRoot
                && !isReadonlyVersion
            );
            this.set("canExecute", item.isCreated && !item.isMaster && isActive && hasEditAccess && !isReadonlyVersion);
        },

        _setupOptions: function (model) {
            var autoTranslate = new AutoTranslate({ model: model }),
                duplicateContent = new DuplicateContent({ model: model }),

                languageBranchCommands = [
                    {
                        "label": autoTranslate.label,
                        "value": autoTranslate
                    },
                    {
                        "label": duplicateContent.label,
                        "value": duplicateContent
                    }
                ];

            this.set("options", languageBranchCommands);
        },

        _selectedSetter: function (command, sender) {
            if (!sender) {
                command.execute();
                // Workaround to mark option always is unchecked.
                this.set("selected", false, this);
            }
        }
    });
});
