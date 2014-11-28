define([
// Dojo
    "dojo/_base/declare",
    "dojo/_base/lang",

    "dojo/Deferred",
    "dojo/aspect",
    "dojo/promise/all",
    "dojo/query",
    "dojo/topic",
    "dojo/when",
    "dojo/on",

// Dijit
    "dijit/_TemplatedMixin",
    "dijit/_Widget",
    "dijit/_WidgetsInTemplateMixin",
    "dijit/layout/BorderContainer",     // used in template
    "dijit/layout/ContentPane",         // used in template
    "dijit/registry",
    "dijit/form/DropDownButton",        // used in template
    "dijit/DropDownMenu",               // used in template
    "dijit/MenuItem",                   // used in template

// Dojox
    "dojox/widget/Standby",

// EPi Framework
    "epi/dependency",
    "epi/shell/widget/_ModelBindingMixin",
    "epi/shell/command/withConfirmation",

// CMS
    "epi-cms/contentediting/ContentViewModel",
    "epi-cms/contentediting/EditToolbar",
    "epi-cms/contentediting/FormEditing",
    "epi-cms/contentediting/SideBySideEditorWrapper",
    "epi-cms/core/ContentReference",
    "epi-cms/contentediting/NotificationBar",
    "epi-cms/contentediting/command/RevertToPublished",

// Language Manager
    "epi-languagemanager/component/viewmodel/CompareEditingViewModel",

// Template
    "dojo/text!./templates/CompareEditing.html",

// Resources
    "epi/i18n!epi/cms/nls/languagemanager.compareediting"
], function (
// Dojo
    declare,
    lang,

    Deferred,
    aspect,
    all,
    query,
    topic,
    when,
    on,

// Dijit
    _TemplatedMixin,
    _Widget,
    _WidgetsInTemplateMixin,
    BorderContainer,
    ContentPane,
    registry,
    DropDownButton,
    DropDownMenu,
    MenuItem,

// Dojox
    Standby,

// EPi Framework
    dependency,
    _ModelBindingMixin,
    withConfirmation,

// CMS
    ContentViewModel,
    EditToolbar,
    FormEditing,
    SideBySideEditorWrapper,
    ContentReference,
    NotificationBar,
    RevertToPublishedCommand,

// Language Manager
    CompareEditingViewModel,

// Template
    template,

// Resources
    res
) {
    // summary:
    //      The compare editing controller. That display 2 form editing include:
    //      - Master language content (in readonly mode)
    //      - Current content (can be edited as normal)
    return declare([_Widget, _TemplatedMixin, _WidgetsInTemplateMixin, _ModelBindingMixin], {
        // baseClass: [public] String
        //      The base style class for this widget
        baseClass: "epi-compareEditing",

        // templateString: [pubic] String
        //      The display template for this widget.
        templateString: template,
        
        // res: Json object
        //      Language resource
        res: res,

        // contextTypeName: String
        //      Type name for the context we can handle
        contextTypeName: "epi.cms.contentdata",

        // currentFormEditing: [pubilc] Object
        //      The instance of viewClass for current content
        currentFormEditing: null,

        // masterFormEditing: [pubilc] Object
        //      The instance of viewClass for master content
        masterFormEditing: null,

        // viewModel: [public] CompareEditingViewModel instance
        //      The view model of this widget.
        viewModel: null,

        // viewParams: [public] Object
        //      The parameters to create view.
        viewParams: null,

        // standby: [protected] Object
        //      Standby widget.
        standby: null,

        // viewClass: [public] Class constructor
        //      The view class.
        viewClass: FormEditing,

        // _autoSaveButton: [private] Reference to AutoSaveButton instance
        //      Used to inject AutoSaveButton in our editing toolbar.
        _autoSaveButton: null,

        // _masterLangElement: [private] Object
        //      Reference to current master active element to copy or translate.
        _masterLangElement: null,

        // _destinationLangElement: [private] Object
        //      Reference to current destination active element to copy or translate.
        _destinationLangElement: null,

        // [private] array Object
        //      Cache element of Tabs and Properties on both side, so later we can find them faster
        cacheTabHandle: null,
        cacheSynchableProperties: null,

        postCreate: function () {
            this.inherited(arguments);

            this.standby = new Standby({ target: "applicationContainer" });
            document.body.appendChild(this.standby.domNode);

            this.viewParams = this.viewParams || {
                toolbar: this.toolbar,
                contextTypeName: this.contextTypeName,
                selectFormOnCreation: false,
                // Fake object to pass throught the harded code session from EditingBase
                iframeWithOverlay: { overlay: { set: function () { } } },
                // Override default removeForm method
                removeForm: function () { return true; }
            };

            var viewModel = new CompareEditingViewModel();
            this.set("model", viewModel);

            this._viewSettingsManager = dependency.resolve("epi.viewsettingsmanager");
            this.own(
                // HACK: make url always different so Iframe should reload
                aspect.around(this._viewSettingsManager, "get", lang.hitch(this, function (originalMethod) {
                    return lang.hitch(this, function (attributeName) {
                        var result = originalMethod.apply(this._viewSettingsManager, [attributeName]);
                        if (attributeName === "previewParams" && this._forceIframeReload) {
                            result.epiLanguageManagerForceReload = !result.epiLanguageManagerForceReload;
                            this._forceIframeReload = false;
                        }
                        return result;
                    });
                })),

                // Reload when content is published
                topic.subscribe("/epi/shell/context/request", lang.hitch(this, function (contextParameters, callerData) {
                    this._forceIframeReload = true;

                    var key = "languagemanageraction",
                        hashWrapper = dependency.resolve("epi.shell.HashWrapper"),
                        hashFragments = hashWrapper.getHash(),
                        actionName = hashFragments[key];
                    // if the sender is not compare edit view, we remove the languagemanageraction hash from URL
                    if (actionName && callerData.sender != this && callerData.sender != this.model) {
                        // we need to set the hashFragments context to new context uri
                        hashFragments["context"] = contextParameters.uri;
                        delete hashFragments[key];
                        hashWrapper.setHash(hashFragments);
                    }
                }))
            );
        },

        startup: function () {
            this.inherited(arguments);

            this.standby.startup();
        },

        resize: function (size) {
            // summary:
            //    Call this to resize a widget, or after its size has changed.
            // tags:
            //    public

            this.inherited(arguments);

            this.layoutContainer.resize(size);
            this.container.resize();
        },

        updateView: function (callerData, ctx, additionalParams) {
            // summary:
            //      Interface method that called from WidgetSwitcher to update view into main region.
            // tags:
            //      public virtual

            // Create compare view under our command only
            if (!callerData || (callerData.languageManagerAction != "compare")) {
                return;
            }

            this._removeAndDestroyForm();

            this.standby.show();

            when(this._setupForms(), lang.hitch(this, function () {
                //console.debug("setup form finished");

                this._setupSyncFocusingEvent();

                // Hide standby
                this.standby.hide();
                // Resize layout
                this.resize();
            }));

            this._setupEvents();
        },

        onPropertyTextEditorSyncClick: function () {
            /// <summary>Call after sync-click to editors on both sides</summary>
        },

        onTabSyncClick: function ($tabMaster, $tabSlave, triggerByLanguageManager) {
            /// <summary>Call after sync-click to tabs on both sides</summary>
        },


        _copyFieldValue: function () {
            /// <summary>Copy field value from master content to current editing language branch</summary>

            if (!this._masterLangElement || !this._destinationLangElement)
                return;

            var sourceEditorWrapper = this._findEditorWrapper(this._masterLangElement[0]),
                valueToCopy = sourceEditorWrapper.editorWidget.get("value"),
                targetEditorWrapper = this._findEditorWrapper(this._destinationLangElement[0]);

            // Do nothing when editor is readonly
            if (!targetEditorWrapper.editorWidget || targetEditorWrapper.editorWidget.readOnly) {
                return;
            }

            // TECHNOTE: we might do not need standby layer show up here because copying is a very fast action
            //this.standby.show();
            this._setEditorWrapperValue(targetEditorWrapper, valueToCopy);
            //this.standby.hide();
        },

        _copyAndTranslateFieldValue: function () {
            /// <summary>Translate field value from master to current language branch</summary>

            if (!this._masterLangElement || !this._destinationLangElement)
                return;

            var sourceEditorWrapper = this._findEditorWrapper(this._masterLangElement[0]),
                valueToTranslate = sourceEditorWrapper.editorWidget.get("value"),
                targetEditorWrapper = this._findEditorWrapper(this._destinationLangElement[0]),
                specialHandling = null;

            // Do nothing when editor is readonly
            if (!targetEditorWrapper.editorWidget || targetEditorWrapper.editorWidget.readOnly) {
                return;
            }
                
            /*=====special handling for StringList property which has value represented as an array of string===*/
            if (lang.isArray(valueToTranslate)) {
                valueToTranslate = valueToTranslate.join("$$\r");
                specialHandling = "StringList";
            }
            /*==================================================================================================*/

            if (valueToTranslate) {
                this.standby.show();
                when(this.model.languageManagerStore.executeMethod("TranslateText", null,
                                            {
                                                "text": valueToTranslate,
                                                "fromLanguageID": this.masterFormEditing.viewModel.languageContext.language,
                                                "toLanguageID": this.currentFormEditing.viewModel.languageContext.language,
                                                "specialHandling": specialHandling
                                            }),
                    lang.hitch(this, function (result) {
                        this._setEditorWrapperValue(targetEditorWrapper, result.message);
                        this.standby.hide();
                    })
                );
            }
            else {
                this._setEditorWrapperValue(targetEditorWrapper, valueToTranslate);
            }
        },

        _findEditorWrapper: function (element) {
            /// <summary>Find the SideBySideEdiotrWrapper contains input dom element</summary>

            var widget = dijit.getEnclosingWidget(element);
            while (widget.isInstanceOf(SideBySideEditorWrapper) == false) {
                widget = widget.parent || widget.getParent();
            }
            return widget;
        },

        _setEditorWrapperValue: function (wrapper, value) {
            /// <summary>Set value for an editor widget which conained in editor wrapper</summary>

            // do nothing if the editor widget is read only
            if (wrapper.editorWidget.readOnly === true) {
                return;
            }
            // set _grabFocusOnStartEdit to [false] to prevent the current view scroll,
            // because we have already sync scroll of left and right editors
            wrapper._grabFocusOnStartEdit = false;

            wrapper.startEdit();
            wrapper.editorWidget.set("value", value);
            wrapper.tryToStopEditing(true);
        },

        _setupSyncFocusingEvent: function () {
            /// <summary>determine the index of the element on one side (LEFT), find the appropriate element on the other side (RIGHT) to perform action (tab click, highlight)</summary>

            var self = this;
            var $appRoot = $('#applicationContainer');

            //#region            /////////// SYNC TAB browsing
            var deferTabBothSideReady = new Deferred();
            self._TryToDoFuncRepeatly(function () {
                var tabSelector = '.compareEditingPane[data-dojo-attach-point={0}] div.dijitTab[widgetid][data-dojo-attach-point*=tabContent]';
                self.cacheTabHandle = {
                    leftForm: $appRoot.find(lang.replace(tabSelector, ['leftForm'])),
                    rightForm: $appRoot.find(lang.replace(tabSelector, ['rightForm']))
                };
                return self.cacheTabHandle.leftForm.length === self.cacheTabHandle.rightForm.length;
            }, self, 400, 10, deferTabBothSideReady);

            when(deferTabBothSideReady, function () {
                var tabBothSideSelector = '.compareEditingPane div[widgetid][data-dojo-attach-point*=tabContent]';
                $appRoot.find(tabBothSideSelector).bind('click', function (event, triggerByLanguageManager) {
                    if (triggerByLanguageManager) {
                        return; // manually trigger by ourselves
                    }

                    var masterSideId = $(event.currentTarget).parents('div.compareEditingPane[data-dojo-attach-point]:eq(0)').attr('data-dojo-attach-point');
                    var slaveSideId = masterSideId === 'rightForm' ? 'leftForm' : 'rightForm';

                    var indexMaster = self.cacheTabHandle[masterSideId].index(event.currentTarget);
                    var $tabSlave = self.cacheTabHandle[slaveSideId].eq(indexMaster);

                    $tabSlave.trigger('click', [true]);     // manually trigger a click to the slave tab

                    self.onTabSyncClick($(event.currentTarget), $tabSlave, triggerByLanguageManager);
                });
            });

            //#endregion            /////////// SYNC TAB browsing


            //#region Wait for tinyMCE editors ready on bothside to bind click event
            /// TECHNOTE: this section try to wait for tinyMCE object because tinyMCE is not ready when this form finish setup/ready.
            /// we try to wait 10 times, after a long waiting, if no tinyMCE object exists, we assume that both side of compareEditing does not have TinyMCE
            /// after having the tinyMCE object, we hook into the onclick of iframe.contentDocument.body to sync click of both side.
            var deferTinyMCEReady = new Deferred();

            // tinymce.editors includes tinymce in form editing mode, so we need to wait 2 seconds for init tinymce in compare view
            self._TryToDoFuncRepeatly(function () {
                try {
                    // if tinymce object exists and number of tinyMCE editor must be an even number, it's OK
                    // after long waiting, this content is assumed doesnot have tinyMCE Editor
                    var OK = tinymce.editors.length > 0;
                    return OK;
                }
                catch (e) { }

            }, self, 2000, 1, deferTinyMCEReady);

            when(deferTinyMCEReady, function foundTinyMCEOnComparingPanel() {
                var iframes = $.map(tinymce.editors, function (editor, index) {
                    var parents = $(editor.container).parents(".epi-compareEditing");
                    // need to check parent of tinymce is compare editing because tinymce.editors includes tinymce for whole page (on page edit, form edit)
                    if (parents && parents.length > 0) {
                        editor.onClick.add(function onTinyMCEEditorContentDocumentClick(editor, e) {
                            var tinyMCETextArea = $('#' + editor.editorId);
                            languageManagerSyncClick(tinyMCETextArea);
                        });
                    }
                });
            });

            //#endregion Wait for tinyMCE editors ready on bothside to bind click event


            //#region Wait for Properties ready from both side, to bind click event
            /// TECHNOTE: sync property browing, find the onclick element (master), find the index of "master", 
            /// then we find the "slave" by masterIndex, highlight the "slave" element

            var deferPropertiesBothSideReady = new Deferred();

            self._TryToDoFuncRepeatly(function () {
                var selector = 'div.compareEditingPane[data-dojo-attach-point={0}] [data-dojo-attach-point*=textbox], div.compareEditingPane[data-dojo-attach-point={0}] div.dijitTabPane .epiTinyMCEEditor [data-dojo-attach-point*=editorFrame]';
                self.cacheSynchableProperties = {
                    leftForm: $appRoot.find(lang.replace(selector, ['leftForm'])),
                    rightForm: $appRoot.find(lang.replace(selector, ['rightForm']))
                };
                return (self.cacheSynchableProperties.leftForm.length === self.cacheSynchableProperties.rightForm.length);

            }, self, 400, 10, deferPropertiesBothSideReady);

            when(deferPropertiesBothSideReady, function () {
                var propBothSideSelector = 'div.compareEditingPane [data-dojo-attach-point*=textbox]';
                $appRoot.find(propBothSideSelector)
                    //.bind('blur', function (event) {
                    //    self._updateToolbarCompareButtonsState();
                    //    // BLUR is raised before CLICK
                    //})
                    .bind('click', function (event) {
                        languageManagerSyncClick(event.currentTarget);
                    });
            });


            function languageManagerSyncClick(element) {

                var $element = $(element);

                // clear syncFocusing of both side
                $('.syncFocusing').not(element) // ignore the currentTarget (already has Focused class)
                    .removeClass('syncFocusing dijitFocused dijitTextBoxFocused dijitTextAreaFocused epiTinyMCEEditorFocused');

                var masterSideId = $element.parents('div.compareEditingPane[data-dojo-attach-point]:eq(0)').attr('data-dojo-attach-point');
                var slaveSideId = masterSideId === 'rightForm' ? 'leftForm' : 'rightForm';

                var indexMaster = self.cacheSynchableProperties[masterSideId].index(element);
                var $propSlave = self.cacheSynchableProperties[slaveSideId].eq(indexMaster);

                // console.debug("indexmaster", indexMaster, $propSlave, $propSlave.position());

                // re-add syncFocusing for both side
                markSyncFocus($element);    // no need to mark focus for currentTarget
                markSyncFocus($propSlave);

                if (masterSideId === 'leftForm') {
                    self._masterLangElement = $element;
                    self._destinationLangElement = $propSlave;
                }
                else {
                    self._masterLangElement = $propSlave;
                    self._destinationLangElement = $element;
                }

                // Sync scroll position as good as possible
                var masterTop = $element.position().top;
                var slaveTop = $propSlave.position().top;
                var delta = slaveTop - masterTop;   // delta of all sum("above" properties)
                var masterPanelSrollTop = $(lang.replace('div.compareEditingPane[data-dojo-attach-point={0}] div.dijitTabContainer[role=tabpanel]', [masterSideId])).scrollTop();

                // scroll the slave panel to the master panel, then adjust a little with delta
                $(lang.replace('div.compareEditingPane[data-dojo-attach-point={0}] div.dijitTabContainer[role=tabpanel]', [slaveSideId])).scrollTop(masterPanelSrollTop + delta);

                self.onPropertyTextEditorSyncClick($element, $propSlave); // raise our own event

                // Toggle Enabled/Disabled state for menu items
                self.duplicateContentMenuItem.set("disabled", element.readOnly);
                self.autoTranslateMenuItem.set("disabled", element.readOnly);
            };

            // SYNC property browsing focus effect
            function markSyncFocus($element) {
                // tinyMCE editor
                var $elementMarkerToApplyFocus = $element.parent('.epiTinyMCEEditor:eq(0)');
                if ($elementMarkerToApplyFocus.length) {
                    // handle for TinyMCE richtextbox
                    $elementMarkerToApplyFocus.addClass('syncFocusing dijitFocused epiTinyMCEEditorFocused');
                    return;
                }

                // inline textbox
                $elementMarkerToApplyFocus = $element.parents('.dijitTextBox:eq(0)');
                if ($elementMarkerToApplyFocus.length) {
                    $elementMarkerToApplyFocus.addClass('syncFocusing dijitFocused dijitTextBoxFocused');
                    return;
                }

                // text area
                $element.addClass('syncFocusing dijitFocused dijitTextAreaFocused');
            };

            //#endregion Wait for Properties ready from both side, to bind click event
        },

        _removeAndDestroyForm: function () {
            // summary:
            //		Check if a form exists so remove from edit layout and destroy it.
            // tags:
            //		private

            // Destroy view (left & right)
            if (this.currentFormEditing) {
                this.currentFormEditing.destroy();
                this.currentFormEditing = null;
            }
            if (this.masterFormEditing) {
                this.masterFormEditing.destroy();
                this.masterFormEditing = null;
            }
        },

        _setupForms: function () {
            // summary:
            //      Setup 2 forms (left and right)
            // tags:
            //      private

            var deferred = new Deferred();

            var deferredList = [this.model.getCurrentContentViewModel(), this.model.getMasterContentViewModel()],
                self = this;
            all(deferredList).then(lang.hitch(this, function () {

                // Setup view common method
                function setupView(container, model, setupEditModeCompleteCallback) {
                    // Initialize current content editing with: ViewModel, Toolbar, layoutContainer
                    var formEditing = new self.viewClass(lang.mixin({
                        editLayoutContainer: container
                    }, self.viewParams));

                    self.own(
                        aspect.after(formEditing, "onGroupCreated", function (groupName, widget) {
                            if (groupName === "ePiServerCMS_SettingsPanel") {
                                // Register an callback method to disable tools button after form is created.
                                self._disableToolButtons(widget);
                            }
                        }, true)
                    );
                    if (setupEditModeCompleteCallback) {
                        self.own(
                            aspect.after(formEditing, "onSetupEditModeComplete", setupEditModeCompleteCallback)
                        );
                    }

                    formEditing.startup();
                    formEditing.onPreviewReady(model);

                    return formEditing;
                };

                // Setup toolbar
                this._setupToolbar();

                // Setup current view
                this.currentFormEditing = setupView(this.rightForm, this.model.currentContentViewModel);
                // Setup master view
                var callback = lang.hitch(this, this._onSetupEditModeComplete);
                this.masterFormEditing = setupView(this.leftForm, this.model.masterContentViewModel, function () {

                    callback.apply(self, arguments);
                    deferred.resolve();
                });
            }));

            return deferred;
        },

        _setupEvents: function () {
            // summary:
            //      Setup all events
            // tags:
            //      private

            // Listen undo/redo changes
            var parent = this.getParent(),
                self = this;
            this.own(
                aspect.after(parent, "layout", function () {
                    self.container.resize();
                }, true),

                self.model.watch("contentLinkSyncChange", function () {
                    self.toolbar.set("contentViewModel", self.model.currentContentViewModel);
                    self.toolbar._revertToPublished.set("model", self.model.currentContentViewModel);
                })
            );
        },


        _setupToolbar: function () {
            // summary:
            //      Setup toolbar including PublishActionMenu, AutoSave button, Back button
            // tags:
            //      private

            var viewModel = this.model.currentContentViewModel;

            // Workaround to ignore broadcast undo/redo global event
            var self = this;
            this.toolbar.setItemProperty("undo", "onClick", function () {
                self.currentFormEditing._undo();
            });
            this.toolbar.setItemProperty("redo", "onClick", function () {
                self.currentFormEditing._redo();
            });

            // We need to init RevertToPublishedCommand ourself because the EditingCommands.revertToPublished already wrapped
            // with confirmation dialog and it will display dublicate dialog when execute the command
            this.toolbar._revertToPublished = withConfirmation(new RevertToPublishedCommand(), null, {
                title: res.reverttopublishconfirmation.dialogtitle,
                heading: res.reverttopublishconfirmation.confirmquestion,
                description: res.reverttopublishconfirmation.description
            });
            this.toolbar._revertToPublished.set("model", viewModel);

            // Enable/disable RevertToPublished button based on canExecute attribute
            this.toolbar._revertToPublished.watch("canExecute", lang.hitch(this, function () {
                this.toolbar.setItemProperty(this.toolbar._revertToPublished.name, "disabled", !this.toolbar._revertToPublished.canExecute);
            }));

            lang.mixin(this.toolbar, {
                viewConfigurations: {
                    availableViews: []
                }
            });
            this.toolbar.set("contentViewModel", viewModel);

            this.textContainer.innerHTML = lang.replace(res.comparing,
                {
                    "masterLanguageName": this.model.masterContentViewModel.contentData.currentLanguageBranch.name,
                    "languageName": this.model.currentContentViewModel.contentData.currentLanguageBranch.name
                });

            this.duplicateContentMenuItem.set("label", lang.replace(res.duplicatecontent, { name: this.model.masterContentViewModel.contentData.currentLanguageBranch.name }));
            this.autoTranslateMenuItem.set("label", lang.replace(res.autotranslate, { name: this.model.masterContentViewModel.contentData.currentLanguageBranch.name }));

            //aspect.after(this, "onPropertyTextEditorSyncClick", function () {
            //    this._updateToolbarCompareButtonsState();
            //}, true);

            //aspect.after(this, "onTabSyncClick", function () {
            //    this._masterLangElement = null;
            //    this._destinationLangElement = null;
            //    this._updateToolbarCompareButtonsState();
            //}, true);

            
            this._updateToolbarCompareButtonsState();
        },

        _updateToolbarCompareButtonsState: function () {
            /// <summary>update the enable-state of buttons base on current selected property</summary>

            // we are focusing on property text editor
            // this.buttonCopy.set("disabled", false);

            // Disable menu items if no translation service is available or no focused property
            this.duplicateContentMenuItem.set("disabled", !this._masterLangElement);
            this.autoTranslateMenuItem.set("disabled", this.model.isTranslatingServiceAvailable !== true || !this._masterLangElement);
        },


        _onCancel: function () {
            // summary:
            //      Back to on page edit view
            // tags:
            //      private

            // Set flag to force Iframe reload data
            this._forceIframeReload = true;

            var contentLink = new ContentReference(this.model.currentContentViewModel.contentLink).createVersionUnspecificReference().toString();           
            
            var key = "languagemanageraction",
                hashWrapper = dependency.resolve("epi.shell.HashWrapper"),
                hashFragments = hashWrapper.getHash(),
                actionName = hashFragments[key];
            if (actionName) {
                // Clear hash
                delete hashFragments[key];
                hashWrapper.setHash(hashFragments);
            } else {
                topic.publish(
                    "/epi/shell/context/request",
                    { uri: "epi.cms.contentdata:///" + contentLink },
                    { sender: this, forceContextChange: true }
                );
            }

            // destroy compare view recursively to make system display correctly notification messages
            setTimeout(lang.hitch(this, function () {
                this.destroyRecursive();
            }), 500);
        },

        _onSetupEditModeComplete: function (form) {
            // summary:
            //		Triggerred when the master content editing view is created.
            // tags:
            //		public, callback

            // Run callback methods after view is created
            // For example: Disable tools dropdown buttons.
            if (this._setupEditModeCompleteCallback) {
                var item;
                while (item = this._setupEditModeCompleteCallback.pop()) {
                    var params = item.params,
                        method = item.func,
                        context = item.context;

                    method.apply(context, params);
                };
            }

            this.resize();
        },

        _disableToolButtons: function (widget) {
            // summary:
            //      Disable the Tools dropdown button on form editing
            // tags:
            //      private

            if (!this._setupEditModeCompleteCallback) {
                this._setupEditModeCompleteCallback = [];
            }

            this._setupEditModeCompleteCallback.push({
                context: this,
                params: [widget],
                func: function (widget) {
                    var toolButtonDOM = query(".dijitDropDownButton", widget.contentDetails.domNode)[0],
                        toolButtons = registry.getEnclosingWidget(toolButtonDOM);

                    toolButtons.set("disabled", true);
                }
            });

        },


        _TryToDoFuncRepeatly: function (func, contextForThis, interval, maxLoopCount, defer) {
            /// <summary>
            /// Util function.
            /// try to exec <param>func</param>. func should return true if its executing is successful
            /// contextForThis: context for func's "this" object
            /// interval: between a loop
            /// maxLoopCount: max count to try executing func
            /// defer: on executing func successfully, this defer will be resolved.
            /// </summary>

            var flagToStop = false;
            var count = 0;

            var loopHandler = setInterval(function () {
                count++;

                if (flagToStop || count > maxLoopCount) {
                    clearInterval(loopHandler);
                    if (defer)
                        defer.reject();
                    return;
                }

                var ret = func.apply(contextForThis);
                if (ret) {
                    clearInterval(loopHandler);
                    flagToStop = true;
                    if (defer)
                        defer.resolve();
                }

            }, interval);
        }
    });
});