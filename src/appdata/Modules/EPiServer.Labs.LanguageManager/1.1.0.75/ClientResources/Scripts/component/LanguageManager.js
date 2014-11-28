define([
// Dojo
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
    "dojo/aspect",
    "dojo/when",
    "dojo/topic",
    "dojo/json",
    "dojo/dom-construct",
    "dojo/dom-style",
    "dojo/dom-class",
    "dojo/dom-geometry",
    "dojo/dom-attr",
    "dojo/html",
    "dojo/store/Memory",
    "dojo/store/Observable",
    "dojo/on",

// Dijit
    "dijit/_TemplatedMixin",
    "dijit/_WidgetBase",
    "dijit/_Container",
    "dijit/layout/_LayoutWidget",
    "dgrid/OnDemandGrid",
    "dgrid/Selection",

// Dojox
    "dojox/widget/Standby",

// EPi Framework
    "epi/dependency",
    "epi/Url",
    "epi/shell/widget/_ModelBindingMixin",
    "epi/shell/command/_WidgetCommandProviderMixin",
    "epi/shell/dgrid/util/misc",
    "epi/shell/widget/_FocusableMixin",
    "epi/shell/widget/dialog/Dialog",
    "epi/shell/widget/dialog/Alert",
    "epi/datetime",
    "epi/shell/command/withConfirmation",
    "epi/shell/widget/ContextMenu",
    "epi/shell/TypeDescriptorManager",

// CMS
    "epi-cms/_ContentContextMixin",
    "epi-cms/core/ContentReference",
    "epi-cms/component/command/DeleteLanguageBranch",

// Language Manager
    "epi-languagemanager/component/viewmodel/LanguageManagerViewModel",

// Resources
    "epi/i18n!epi/cms/nls/languagemanager.gadget"

], function (
// Dojo
    declare,
    lang,
    array,
    aspect,
    when,
    topic,
    json,
    domConstruct,
    domStyle,
    domClass,
    domGeometry,
    domAttr,
    html,
    Memory,
    Observable,
    on,

// Dijit
    _TemplatedMixin,
    _WidgetBase,
    _Container,
    _LayoutWidget,
    OnDemandGrid,
    Selection,

// Dojox
    Standby,

// EPi Framework
    dependency,
    Url,

    _ModelBindingMixin,
    _WidgetCommandProviderMixin,
    GridMiscUtil,
    _FocusableMixin,
    Dialog,
    Alert,
    epiDate,
    withConfirmation,
    ContextMenu,
    TypeDescriptorManager,

// CMS
    _ContentContextMixin,
    ContentReference,
    DeleteLanguageBranch,

// Language Manager
    LanguageManagerViewModel,

// Resources
    res
) {

    return declare([_Container, _LayoutWidget, _TemplatedMixin, _ModelBindingMixin, _WidgetCommandProviderMixin, _FocusableMixin], {

        modulePath: null,
        moduleClientResourcesPath: null,
        isTranslatingSystemAvailable: null,     // Determines whether current TranslatingService available

        model: null,
        context: null,
        contentData: null,
        store: null,

        grid: null,
        standby: null,

        commandProvider: null,
        commandProviderInitialized: null,

        // _hashWrapper: epi.shell.HashWrapper
        //    The hash wrapper instance.
        _hashWrapper: null,

        _inactiveCssClass: "inactive",
        _currentLanguageCssClass: "currentLanguage",
        _disabledCssClass: "disabled",
        _preferLanguageCssClass: "preferLanguage",
        _mouseOverCssClass: 'epi-dgrid-mouseover',

        modelBindingMap: {
            "context": ["context"],
            "contentData": ["contentData"],
            "store": ["store"],
            "actionExecuted": ["actionExecuted"],
            "actionExecuting": ["actionExecuting"]
        },

        templateString: '<div class="epi-languagemanager"></div>',

        postscript: function () {
            // summary:
            //      Initialize view model, setting up the grid and register events as well.
            // tags:
            //      protected

            this.inherited(arguments);

            this._hashWrapper = dependency.resolve("epi.shell.HashWrapper");

            var customGridClass = declare([OnDemandGrid, Selection]);
            this.grid = new customGridClass({
                columns: {
                    name: {
                        renderCell: lang.hitch(this, this._setupLanguageCollumn)
                    },
                    contentStatus: {
                        renderCell: lang.hitch(this, this._setupContentStatusCollumn)
                    },
                    existence: {
                        renderCell: lang.hitch(this, this._setupExistenceCollumn)
                    },
                    contextmenu: {
                        renderCell: function (item, value, node, options) {
                            var format = "<div class='epi-languageManagerEditAction' title='{0}'>\
                                            <span class='dijitInline dijitIcon epi-iconContextMenu epi-floatRight' title ='{1}'>&nbsp;</span>\
                                          <div>";
                            // TODO: make localization for tooltip
                            node.innerHTML = lang.replace(format, ["Context Menu", "Context Menu"]);
                        }
                    }
                },
                selectionMode: "single",
                showHeader: false
            }, this.domNode);

            // init model for the widget
            this.set("model", this.model || new LanguageManagerViewModel({ modulePath: this.modulePath }));
            this._setupEvents();
            this._createContextMenu();
        },

        startup: function () {

            this.inherited(arguments);

            this.standby = new Standby({ target: "applicationContainer" });
            document.body.appendChild(this.standby.domNode);
            this.standby.startup();
                     
        },

        _langIDFromUrl: new Url(window.location.href).query.language,

        _aroundInsertRow: function (/*Object*/original) {
            // summary:
            //      Called 'around' the insertRow method to fix the grids less than perfect selection.
            // tags:
            //      private

            return lang.hitch(this, function (object, parent, beforeNode, i, options) {

                // Call original method
                var row = original.apply(this.grid, arguments);
                var isPage = this.context.capabilities.isPage;
                var isBlock = this._isBlock(this.context);
                var languageContext = this.context.languageContext;
                var isSupportLanguageContent = languageContext.hasTranslationAccess;

                var isActive = ((isPage || isSupportLanguageContent) && object.isActive) || isBlock;    // block always is active

                if (object.languageID === this._langIDFromUrl) {
                    this.grid.clearSelection();
                    this.grid.select(object);
                    this.model.onItemSelected(object);
                }

                if (!isActive) {
                    domClass.add(row, this._inactiveCssClass);  // add the inactive class to the whole row
                }
                if (languageContext && object.languageID === languageContext.preferredLanguage) {
                    domClass.add(row, this._preferLanguageCssClass);
                }

                row.title =
                    isActive && (languageContext && object.languageID !== languageContext.preferredLanguage)
                    && ((isPage && this.model.contentData.capabilities.languageSettings) || isBlock)      // block always can switch
                    ? res.clicktoswitchlanguage : !isActive ? res.editingforlanguageisdisabled : ''
                ;

                return row;
            });
        },

        _isBlock: function (context) {
            // summary:
            //      Determine if the input context is block or not.
            // tags:
            //      private

            return (context && TypeDescriptorManager.isBaseTypeIdentifier(context.dataType, "episerver.core.blockdata"));
        },

        _setContextAttr: function (context) {
            if (context) {
                this._set("context", context);
                if (!this._langIDFromUrl) {
                    this._langIDFromUrl = context.language;
                }
            }

        },

        _setContentDataAttr: function (contentData) {
            if (!contentData) {
                return;
            }

            if (!this.commandProviderInitialized) {
                // Re-initiliaze command provider
                this.commandProviderInitialized = true;

                this.commandProvider = this.model;
                this._consumer = this.getConsumer();

                this._consumer.addProvider(this.commandProvider);
                this.contextMenu.addProvider(this.commandProvider);
                this.contextMenu.startup();

                this.own(
                    this.commandProvider,
                    aspect.around(this._consumer.toolbar, "_getCurrentPosition", function (originalMethod) {
                        return function (currentCategory) {
                            if (currentCategory === "default") {
                                return 0;
                            }
                            return originalMethod(currentCategory);
                        }
                    })
                );
            } else {
                this._consumer.removeProvider(this.commandProvider);
                this.contextMenu.removeProvider(this.commandProvider);

                this.defer(lang.hitch(this, function () {
                    this._consumer.addProvider(this.commandProvider);
                    this.contextMenu.addProvider(this.commandProvider);
                }), 1);
            }

            this.grid.set("query", { "contentLink": contentData.contentLink });
        },

        _setActionExecutingAttr: function (value) {
            if (!value) {
                return;
            }
            this.standby.show();
        },

        _setActionExecutedAttr: function (result) {
            if (!result) {
                return;
            }
            this.grid.refresh();
            this.standby.hide();
        },

        _setStoreAttr: function (/*Object*/store) {
            // summary:
            //      Set the store associatated with the grid.
            // parameters:
            //      store: Rest store object.
            // tags:
            //      private

            this._set("store", store);
            this.grid.set("store", store);
        },

        _setupLanguageCollumn: function (item, value, node, options) {
            // summary:
            //      Setup for the Language collumn of dgrid.
            // param:
            //  item: LanguageInfo item of LanguageManager Rest Store
            //  node: grid cell element

            var isPage = this.context.capabilities.isPage;
            var isBlock = this._isBlock(this.context);

            var languageContext = this.context.languageContext;
            var isSupportLanguageContent = languageContext.hasTranslationAccess;
            
            var isActive = (isPage && item.isActive) || isBlock || isSupportLanguageContent;    // block always is active
            var canActivateOrDeactivate = (isPage && item.canActivate && this.model.contentData.capabilities.languageSettings);
            

            var systemIconString = lang.replace("<img class='systemIcon' src='{systemIconPath}' />",
                {
                    systemIconPath: item.systemIconPath || this.moduleClientResourcesPath + "/Images/emptyFlag.png"
                });
            var languageName = lang.replace("<span class='languageName rowTextElement'>{languageName}{suffix}</span>",
                {
                    languageName: value,
                    suffix: item.isMaster ? ' (' + res.master + ')' : ''
                })
            ;
            node.innerHTML = systemIconString + languageName;

            if (item.isCurrent) {
                domClass.add(node, this._currentLanguageCssClass);
            }

            if (!isActive) {
                domClass.add(node, this._inactiveCssClass);
            }

            if (!canActivateOrDeactivate) {
                domClass.add(node, this._disabledCssClass);
            }
        },

        _setupContentStatusCollumn: function (item, value, node, options) {
            // summary:
            // param:
            //  item: LanguageInfo item of LanguageManager Rest Store
            //  node: grid cell element

            var isPage = this.context.capabilities.isPage;
            var isBlock = this._isBlock(this.context);
            var isActive = (isPage && item.isActive) || isBlock;    // block always is active
            
            var contentStatus = item.isCreated ? (item.isPublished ? res.published : res.draft) : res.notcreatedyet;
            var contentHtml = lang.replace("<span class='rowTextElement' >{0}</span>", [contentStatus]);
            node.innerHTML = contentHtml;
        },

        _setupExistenceCollumn: function (item, value, node, options) {
            // summary:
            // param:
            //  item: LanguageInfo item of LanguageManager Rest Store
            //  node: grid cell element

            var isPage = this.context.capabilities.isPage;
            var isBlock = this._isBlock(this.context);
            var isActive = (isPage && item.isActive) || isBlock;    // block always is active

            var contentHtml = '';

            if (isActive && item.canCreate && !item.isCreated) {
                contentHtml = lang.replace("<a class='lmLanguageBranchAction createLanguageBranch rowTextElement epi-visibleLink' >{0}</a>", [res.create]);
            } else if (item.isCreated) {
                contentHtml = lang.replace("<span class='lmDateTime rowTextElement' >{0}</span>", [epiDate.toUserFriendlyString(item.publishedDateTime ? item.publishedDateTime : item.savedDateTime)]);
            }

            node.innerHTML = contentHtml;
        },

        _onCreateLanguageBranch: function (e) {
            // summary:
            //    Create a language branch.
            //
            // tags:
            //    private
            e.stopImmediatePropagation();

            this.model.set("target", e);

            // TODO: Select row that was clicked
            this.model.onItemSelected(this.grid.row(e).data);
            var command = this.model.getCommand("create");
            command.execute();
        },

        _onSwitchLanguage: function (evt) {
            // summary:
            //    Handle event for switching between languages.
            //
            // tags:
            //    private

            var cell = this.grid.cell(evt);
            var row = this.grid.row(evt);
            // Do not redirect to the language branch if it is not active,
            // or it is prefer language
            if (domClass.contains(row.element, this._inactiveCssClass)
                || domClass.contains(cell.element, this._preferLanguageCssClass)) {
                return;
            }
            var hash = this._hashWrapper.getHash(),
                currentUrl = new Url(window.location.href),
                currentUrlPath = currentUrl.scheme + "://" + currentUrl.authority + currentUrl.path;
            // TECHNOTE: consider to use topic.publish changerequest.
            window.location.replace(currentUrlPath + row.data.linkFragment);
            
            // force to refresh page if url is the same language, but different hash (of view setting)
            if (hash && hash.viewsetting) {
                if (new Url(window.location.href).query.language === row.data.languageID) {
                    window.location.reload();
                }
            }
        },

        _createContextMenu: function () {
            // summary:
            //      Creates the context menu for the grid.
            // tags:
            //      private

            this.contextMenu = new ContextMenu({ category: "contextMenu" });
            this.own(this.contextMenu);
        },

        _setupEvents: function () {
            // summary:
            //      Initialization of events on the grid.
            // tags:
            //      protected

            var self = this;
            this.own(
                this.grid.on(".createLanguageBranch:click", lang.hitch(this, "_onCreateLanguageBranch")),
                this.grid.on(".epi-iconContextMenu:click", lang.hitch(this, "_onContextMenuClick")),

                //When mouse is over a row, show the context menu node
                this.grid.on(".dgrid-row:mouseover", function (event) {
                    domClass.add(this, self._mouseOverCssClass);
                }),
                //When mouse is out of an unselected row, hide the context menu node
                this.grid.on(".dgrid-row:mouseout", function (event) {
                    domClass.remove(this, self._mouseOverCssClass);
                }),

                /* click on row (except buttons, contextmenu) will switch */
                this.grid.on(".dgrid-row:click", lang.hitch(this, "_onSwitchLanguage")),

                //When mouse is out of an unselected row, hide the context menu node
                this.grid.on("dgrid-select", lang.hitch(this, function (event) {
                    this.model.onItemSelected(event.rows[0].data);
                })),

                // update the grid when user close language setting dialog, or manage website language dialog
                topic.subscribe("/epi/cms/contentdata/updated", function () {
                    self.grid.refresh();
                }),

                // Overide base method from grid to set selected item
                aspect.around(this.grid, "insertRow", lang.hitch(this, this._aroundInsertRow)),

                this.model.watch("currentItemData", lang.hitch(self, self._onItemSelected))
             
            );
        },

        _onContextMenuClick: function (e) {
            // summary:
            //    Handle event click of context menu node.
            //
            // tags:
            //    private

            e.stopImmediatePropagation();

            var availableCommands = this.model.getAvailableCommands("contextMenu");
            if (availableCommands && availableCommands instanceof Array && availableCommands.length > 0) {
                this.contextMenu.scheduleOpen(this, null, {
                    x: e.pageX,
                    y: e.pageY
                });
            }
        },

        _onItemSelected: function () {
            // summary:
            //    Call after user choose a language branch from the grid.
            //
            // tags:
            //    private
            
            // HACK: get Auto translate menu item and set it enabled/disabled depend on isTranslatingSystemAvailable

            if (!this._consumer.toolbar || !this.model.currentItemData.isCreated) {
                return;
            }
            if (this._consumer.toolbar.getChildren()[1].dropDown.getChildren()[2] && this._consumer.toolbar.getChildren()[1].dropDown.getChildren()[2].popup) {
                var autoTranslateMenuItemInToolbarMoreOptionsMenu = this._consumer.toolbar.getChildren()[1].dropDown.getChildren()[2].popup.getChildren()[0];
                autoTranslateMenuItemInToolbarMoreOptionsMenu.set("disabled", this.model.isTranslatingSystemAvailable !== true);
            }

            if (this.contextMenu.getChildren()[2] && this.contextMenu.getChildren()[2].popup) {
                var autoTranslateMenuItemInContextMenu = this.contextMenu.getChildren()[2].popup.getChildren()[0];
                autoTranslateMenuItemInContextMenu.set("disabled", this.model.isTranslatingSystemAvailable !== true);
            }
        }
    });
});