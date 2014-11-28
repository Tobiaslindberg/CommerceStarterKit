define([
// Dojo base
    "dojo/_base/declare",
    "dojo/_base/array",
    "dojo/_base/lang",
    "dojo/aspect",
    "dojo/dom", // dom.byId dom.isDescendant
    "dojo/dom-geometry",

// Dijit
    "dijit/popup",
    "dijit/focus",
    "dijit/Destroyable",

// Language manager
    "epi-languagemanager/component/command/CommandBase",
    "epi-languagemanager/widget/CreateLanguageBranchSelector",

    "epi-languagemanager/widget/command/DuplicateContent",
    "epi-languagemanager/widget/command/StartBlank",
    "epi-languagemanager/widget/command/AutoTranslate",

    // Resources
    "epi/i18n!epi/cms/nls/languagemanager.gadget"

], function (
// Dojo base
    declare,
    array,
    lang,
    aspect,
    dom,
    domGeometry,

// Dijit
    popupManager,
    focusUtil,
    Destroyable,

// Language manager
    CommandBase,
    CreateLanguageBranchSelector,

    DuplicateContent,
    StartBlank,
    AutoTranslate,

    res
) {


    return declare([CommandBase, Destroyable], {

        // label: [public] String
        //		The action text of the command to be used in visual elements.
        label: res.create,

        category: "context",

        // For contextMenu
        popup: null,
        popupClass: CreateLanguageBranchSelector,

        // For create link
        _internalPopup: null,

        constructor: function (params) {
            var model = params ? params.model : null;

            // init popup with emty items then setup items later on model changed
            this.set("isAvailable", true);
            this.set("canExecute", true);
        },

        _setupPopup: function (/*Object*/popup) {
            // summary:
            //      Initialize popup sub items and do something else.
            // tags:
            //      private

            // Create sub items
            this._setupCreateMenuSubItems(this.model, popup);
            // Set new header label
            popup.set("header", lang.replace(res.createpagein, this.model.currentItemData));
        },

        _setupEvent: function (/*Object*/popup) {
            // summary:
            //      Bind event's listeners on the popup
            // tags:
            //      private

            this.own(
                aspect.after(popup, "onItemSelected", lang.hitch(this, function (command) {
                    var parentMenu = popup.parentMenu;
                    if (parentMenu) {
                        parentMenu._cleanUp();
                        popupManager.close(parentMenu);
                    } else {
                        popupManager.close(popup);
                    }

                    this.onItemSelected(command);
                }), true)
            );
        },

        _setupInternalPopup: function() {
            // summary:
            //      Create an independent popup that used to display everywhere
            // tags:
            //      private

            if (!this._internalPopup) {
                this._internalPopup = new this.popupClass();
                this._setupEvent(this._internalPopup);
            }
            this._setupPopup(this._internalPopup);
        },

        _popupSetter: function (/*Object*/popup) {
            // summary:
            //      Customize set method to setting up on the popup
            // tags:
            //      public override

            this.popup = popup;
            this._setupPopup(this.popup);
            this._setupEvent(this.popup);
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
                isActive = ((isPage || isSupportLanguageContent) && item.isActive) || isBlock;    // block always is active

            this.set("isAvailable", isActive && item.canCreate && !item.isCreated);
            this.set("canExecute", isActive && item.canCreate && !item.isCreated);

            this._setupPopup(this.popup);
            this._setupInternalPopup();
        },

        execute: function (evt) {
            this._execute(evt);
        },

        _setupCreateMenuSubItems: function (model, popup) {
            // summary:
            //		Setup sub menu items when user click to create language branch.
            // tags:
            //		private

            var isTranslatingSystemAvailable = this.model.isTranslatingSystemAvailable;
            
            var autoTranslate = new AutoTranslate({ model: model }),
                duplicateContent = new DuplicateContent({ model: model }),
                startWithBlank = new StartBlank({ model: model }),
                createLanguageBranchCommands = [
                    {
                        "label": autoTranslate.label,
                        "value": autoTranslate,
                        "disabled": isTranslatingSystemAvailable == false ? "disabled" : false
                    },
                    {
                        "label": duplicateContent.label,
                        "value": duplicateContent,
                        "disabled": false
                    },
                    {
                        "label": startWithBlank.label,
                        "value": startWithBlank,
                        "disabled": false
                    }
                ];

            popup.set("items", createLanguageBranchCommands);
        },

        _execute: function (evt) {
            // summary:
            //		Executes this command assuming canExecute has been checked. Subclasses should override this method.
            // tags:
            //		protected

            var self = this,
                target = this.model.get("target"),
                mouseEvent = evt || target;

            if (!mouseEvent) {
                return;
            }

            var popup = this._internalPopup,
                prevFocusNode = focusUtil.get("prevNode"),
                curFocusNode = focusUtil.get("curNode"),
                focusNode = popup.domNode,
                savedFocusNode = !curFocusNode || (dom.isDescendant(curFocusNode, focusNode)) ? prevFocusNode : curFocusNode;

            var popupBlurHanlder = aspect.after(popup, "onBlur", function () {
                popupBlurHanlder.remove();

                self.model.set("target", null);
                popupManager.close(popup);
                if (savedFocusNode) {
                    savedFocusNode.focus();
                }
            });

            focusUtil.focus(focusNode);

            popupManager.open({
                popup: popup,
                x: mouseEvent.pageX,
                y: mouseEvent.pageY - 156,
                onCancel: function () {
                    popupManager.close(popup);
                }
            });
        },

        onItemSelected: function (command) {
            command.execute(true);
        }
    });
});
