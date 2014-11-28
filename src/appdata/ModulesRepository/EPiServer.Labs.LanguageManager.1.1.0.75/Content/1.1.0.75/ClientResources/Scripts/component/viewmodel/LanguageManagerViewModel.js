define([
// Dojo
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
    "dojo/_base/Deferred",

    "dojo/aspect",
    "dojo/promise/all",
    "dojo/when",
    "dojo/Stateful",
    "dojo/string",
    "dojo/_base/xhr",
    "dojo/topic",

    "dojox/html/entities",

//EPi
    "epi/epi",
    "epi/datetime",
    "epi/dependency",
    "epi/shell/_StatefulGetterSetterMixin",
    "epi/shell/command/withConfirmation",
    "epi/shell/command/_CommandProviderMixin",
    "epi/shell/TypeDescriptorManager",

//CMS
    "epi-cms/_ContentContextMixin",
    "epi-cms/contentediting/ContentActionSupport",
    "epi-cms/core/ContentReference",

// Language Manager commands
    "epi-languagemanager/component/command/CompareWithMasterLanguage",
    "epi-languagemanager/component/command/DeleteLanguageBranch",
    "epi-languagemanager/component/command/ManageWebsiteLanguages",

    "epi-languagemanager/component/command/OpenLanguageSettings",
    "epi-languagemanager/component/command/Settings",
    "epi-languagemanager/component/command/ToggleEditingLanguageBranch",
    "epi-languagemanager/component/command/CreateLanguageBranch",
    "epi-languagemanager/component/command/ReplaceLanguageBranch",
    
// Resources
   "epi/i18n!epi/cms/nls/languagemanager.gadget"
],

function (
// Dojo
    declare,
    lang,
    array,
    Deferred,

    aspect,
    all,
    when,
    Stateful,
    dojoString,
    xhr,
    topic,

    entities,

// EPi
    epi,
    epiDatetime,
    dependency,
    _StatefulGetterSetterMixin,
    withConfirmation,
    _CommandProviderMixin,
    TypeDescriptorManager,

//CMS
    _ContentContextMixin,
    ContentActionSupport,
    ContentReference,

// Language Manager commands
    CompareWithMasterLanguageCommand,
    DeleteLanguageBranchCommand,
    ManageWebsiteLanguagesCommand,

    OpenLanguageSettingsCommand,
    SettingsCommand,
    ToggleEditingLanguageBranchCommand,
    CreateLanguageBranchCommand,
    ReplaceLanguageBranch,

// Resources
    res

    ) {

    // summary:
    //      View model object for the LanguageManager widget.
    return declare([Stateful, _StatefulGetterSetterMixin, _CommandProviderMixin, _ContentContextMixin], {

        // store: [protected] Object
        //      Rest store for manipulate model data.
        store: null,

        res: null,

        _commandRegistry: null,

        _hashWrapper: null,

        _commandsReady: null,

        _currentItemDataReady: null,

        currentItemData: null,

        target: null,

        // deleteLanguageBranchSettings: Object
        //      Settings for DeleteLanguageBranch command
        _deleteLanguageBranchSettings: null,

        postscript: function () {
            // summary:
            //      Initialize store and data displayed for page.
            // tags:
            //      protected

            this.inherited(arguments);

            this.res = this.res || res;
            this._hashWrapper = dependency.resolve("epi.shell.HashWrapper");            
            this._commandsReady = new Deferred();
            this._currentItemDataReady = new Deferred();

            //resolve _viewSettingsManager
            this._viewSettingsManager = dependency.resolve("epi.viewsettingsmanager");

            var key = "languagemanageraction",
                hashFragments = this._hashWrapper.getHash(),
                actionName = hashFragments[key];
            if (actionName) {
                // Assign custom view into current context to replace the default view.
                this._currentContext.customViewType = "epi-languagemanager/component/CompareEditing";

                // we need to wait for commands and the currentDataItem are setup,
                // then get associated command
                all([this._commandsReady, this._currentItemDataReady]).then(lang.hitch(this, function (results) {
                    // Execute command
                    var command = this.getCommand(actionName);
                    command && command.execute(null, true);
                }));
            }

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
                }))
            );
            
            var registry = dependency.resolve("epi.storeregistry");
            this.set("store", this.store || registry.get("epi-languagemanager.language"));

            all({
                "currentContext": this.getCurrentContext(),
                "currentContent": this.getCurrentContent(),
                "isInAdminRole": this.store.executeMethod("IsCurrentUserInAdminRole", null, null),
                "isTranslatingSystemAvailable": this.store.executeMethod("IsTranslatingServiceAvailable", null, null)
            }).then(lang.hitch(this, function (result) {
                var context = result.currentContext,
                    contentData = result.currentContent,
                    isInAdminRole = result.isInAdminRole,
                    isTranslatingSystemAvailable = result.isTranslatingSystemAvailable;

                this.set("isTranslatingSystemAvailable", isTranslatingSystemAvailable);
                this.set("context", context);
                this._setupMasterLanguage(contentData);
                this._setupCommands();
                this.set("contentData", contentData);
                this.set("isInAdminRole", isInAdminRole);
                
                this.updateCommandModel(this);
                this._commandsReady.resolve();
            }));
        },

        contentContextChanged: function (context, callerData) {
            // summary:
            //      Content context change event.
            // tags:
            //      event

            this.set("context", context);
            all({
                "currentContent": this.getCurrentContent()
            }).then(lang.hitch(this, function (result) {
                var contentData = result.currentContent;
                this._setupMasterLanguage(contentData);
                this.set("contentData", contentData);
                this.updateCommandModel(this);
            }));
        },

        getCommand: function (commandName) {
            // summary:
            //      Gets a command by command name
            // tags:
            //      protected

            return this._commandRegistry && this._commandRegistry[commandName] ? this._commandRegistry[commandName].command : null;
        },

        getAvailableCommands: function (category) {
            // summary:
            //      Gets available commands by category
            // tags:
            //      public

            var commands = this.get("commands");
            return commands.filter(function (cmd) {
                return cmd.category === category && cmd.get("isAvailable") == true;
            }, this);
        },

        onItemSelected: function (itemData) {
            // summary:
            //      Call when user choose a row on the grid.
            // tags:
            //      public

            this.set("currentItemData", itemData);
            this.updateCommandModel(this);

            // Update text contents of the confirmation dialog for DeleteLanguageBranch command
            var content = this.contentData;
            if (itemData && content) {
                var heading = lang.replace(res.deletelanguagebranch, itemData),
                    description = TypeDescriptorManager.getResourceValue(content.typeIdentifier, "deletelanguagebranchdescription");

                lang.mixin(this._deleteLanguageBranchSettings, {
                    confirmActionText: heading,
                    description: lang.replace(description, [entities.encode(content.name), itemData.name]),
                    title: heading
                });
            }

            if (this._currentItemDataReady) {
                this._currentItemDataReady.resolve();
                this._currentItemDataReady = null;
            }
        },

        autoTranslate: function () {
            // summary:
            //      Replace content by translating from master language branch.
            // tags:
            //      pubic

            this._executeAction("TranslateAndCopyDataFromMasterBranch",
                    {
                        twoLetterISOLanguageName: this.currentItemData.twoLetterISOLanguageName
                    },
                    this._fireContextRequest
                );
        },

        duplicateMasterContent: function () {
            // summary:
            //      Copy and replace content from master language branch.
            // tags:
            //      pubic

            this._executeAction("CopyDataFromMasterBranch", null, this._fireContextRequest);
        },

        startWithBlank: function () {
            // summary:
            //      Create a language branch with empty value for all properties.
            // tags:
            //      pubic

            this._executeAction("CreateLanguageBranch", null, this._fireContextRequest);
        },


        deleteLanguageBranch: function () {
            // summary:
            //      Delete a language branch.
            // tags:
            //      pubic

            var onComplete = function () {
                var contentReference = new ContentReference(this.contentData.contentLink);
                topic.publish("/epi/shell/context/request", { uri: "epi.cms.contentdata:///" + contentReference.createVersionUnspecificReference() }, { sender: this, forceContextChange: true });
            }
            this._executeAction("DeleteLanguageBranch", null, onComplete);
        },

        toggleLanguageBranchActivation: function () {
            // summary:
            //      Activate/Deactivate a language branch.
            // tags:
            //      pubic

            var onComplete = function () {
                var contentReference = new ContentReference(this.contentData.contentLink);
                topic.publish(
                    "/epi/shell/context/request",
                    { uri: "epi.cms.contentdata:///" + contentReference.createVersionUnspecificReference() },
                    { sender: this, forceContextChange: true }
                );

                // update context, this section is similar to the CloseButton of LanguageSetting legacy dialog
                // publish this event, so when we use LanguageManager's context menu to enable, disable a language, 
                // SiteGadget can be changed to refresh the available LanguageList
                topic.publish("/epi/cms/contentdata/updated", {
                    contentLink: contentReference.createVersionUnspecificReference(),
                    recursive: true
                });
                
            }

            this._executeAction("ToggleLanguageBranchActivation",
                {
                    twoLetterISOLanguageName: this.currentItemData.twoLetterISOLanguageName
                }, onComplete);
        },

        _executeAction: function (actionName, params, onComplete) {
            // summary:
            //      Execute an action.
            // tags:
            //      private

            this.set("actionExecuting", true);

            var defaultParams = {
                contentLink: this.contentData.contentLink,
                languageID: this.currentItemData.languageID
            }
            when(this.store.executeMethod(actionName, null, lang.mixin(defaultParams, params)),
                lang.hitch(this, function (result) {
                    this.set("actionExecuted", result);
                    if (onComplete && result.isSuccess == true) {
                        onComplete.call(this);
                    }
                }));
        },

        _fireContextRequest: function () {
            // summary:
            //      public event to force context change.
            // tags:
            //      private

            // Set flag to force Iframe reload data
            this._forceIframeReload = true;

            var id = new ContentReference(this.contentData.contentLink).createVersionUnspecificReference().toString();
            topic.publish("/epi/shell/context/request", {
                uri: "epi.cms.contentdata:///" + id
            }, {
                sender: this,
                forceContextChange: true
            });
        },

        _setupMasterLanguage: function (contentData) {
            // summary:
            //      Set masterLanguage property when context change. 
            // tags:
            //      private

            if (contentData && contentData.existingLanguageBranches.length > 0) {
                var masterLanguage = contentData.existingLanguageBranches.filter(function (languageBranch) {
                    return languageBranch.isMasterLanguage == true;
                }, this);

                // TODO: Need to get the correct master language.
                // Workaround to get the master language.
                if (masterLanguage.length == 0) {
                    masterLanguage = contentData.existingLanguageBranches;
                }

                this.set("masterLanguage", masterLanguage[0].name);
            }
        },

        _setupCommands: function () {
            // summary:
            //      Setup commands for the widget.
            // tags:
            //      private

            this._deleteLanguageBranchSettings = {
                cancelActionText: epi.resources.action.cancel,
                setFocusOnConfirmButton: false
            };

            var commands = {
                /* Commands for context menu */
                createContextMenu: {
                    command: new CreateLanguageBranchCommand({ category: "contextMenu", model: this }),
                    order: 10
                },
                compareContextMenu: {
                    command: new CompareWithMasterLanguageCommand({ category: "contextMenu", iconClass: null }),
                    order: 20
                },
                replaceContextMenu: {
                    command: new ReplaceLanguageBranch({ category: "contextMenu", model: this }),
                    order: 30
                },
                toggleContextMenu: {
                    command: new ToggleEditingLanguageBranchCommand({ category: "contextMenu" }),
                    order: 40
                },
                deleteContextMenu: {
                    command: withConfirmation(new DeleteLanguageBranchCommand({ category: "contextMenu" }), null, this._deleteLanguageBranchSettings),
                    order: 50
                },

                /* Commands for toolbar default position */
                compare: {
                    command: new CompareWithMasterLanguageCommand({ model: this })
                },

                /* Commands for toolbar context position */
                open: {
                    command: new OpenLanguageSettingsCommand(),
                    order: 10
                },
                create: {
                    command: new CreateLanguageBranchCommand({ model: this }),
                    order: 20
                },
                replace: {
                    command: new ReplaceLanguageBranch({ model: this }),
                    order: 30
                },
                toggle: {
                    command: new ToggleEditingLanguageBranchCommand(),
                    order: 40
                },
                "delete": {
                    command: withConfirmation(new DeleteLanguageBranchCommand(), null, this._deleteLanguageBranchSettings),
                    order: 50
                },

                /* Commands for toolbar settings position */
                manage: {
                    command: new ManageWebsiteLanguagesCommand()
                },
                setting: {
                    command: new SettingsCommand()
                },
                
                sort: function () {
                    var commands = [];
                    for (var key in this) {
                        if (key !== "toArray" && key !== "sort" && this.hasOwnProperty(key)) {
                            var index = this[key].order;
                            if (!index) {
                                index = 100;
                            }
                            commands.push([index, this[key].command]);
                        }
                    }

                    commands.sort(function (a, b) {
                        return a[0] - b[0];
                    });

                    return commands;
                },
                toArray: function () {
                    var sortedCommand = this.sort();
                    var commands = [];
                    array.forEach(sortedCommand, function (key) {
                        commands.push(key[1]);
                    });

                    return commands;
                }
            };

            this._commandRegistry = lang.mixin(this._commandRegistry, commands);
            //this.commands = this._commandRegistry.toArray();
            this.set("commands", this._commandRegistry.toArray());
        }
    });
});