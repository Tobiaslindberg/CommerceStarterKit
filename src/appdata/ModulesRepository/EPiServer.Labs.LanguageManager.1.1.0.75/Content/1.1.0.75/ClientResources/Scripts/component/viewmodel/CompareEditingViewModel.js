define([
// Dojo
    "dojo/_base/declare",
    "dojo/_base/Deferred",
    "dojo/_base/lang",
    "dojo/_base/connect",
    "dojo/promise/all",
    "dojo/when",
    "dojo/Stateful",

//EPi Framework
    "epi/dependency",
    "epi/shell/_StatefulGetterSetterMixin",

//CMS
    "epi-cms/_ContentContextMixin",
    "epi-cms/core/ContentReference",
    "epi-cms/contentediting/ContentViewModel"
],

function (
// Dojo
    declare,
    Deferred,
    lang,
    connect,
    all,
    when,
    Stateful,

// EPi
    dependency,
    _StatefulGetterSetterMixin,

//CMS
    _ContentContextMixin,
    ContentReference,
    ContentViewModel
) {

    // summary:
    //      View model object for the CompareEditing controller.
    return declare([Stateful, _StatefulGetterSetterMixin, _ContentContextMixin], {

        // contentDataStore: [protected] Object
        //      Rest store for manipulate model data.
        contentDataStore: null,

        // contextStore: [protected] JsonRestStore
        //      Rest store for get content context.
        contextStore: null,

        // contentVersionStore: [protected] Object
        //      Rest store for get content versions.
        contentVersionStore: null,

        // languageManagerStore: [protected] JsonRestStore
        //      Rest store for translating property.
        languageManagerStore: null,

        // currentContentViewModel: [public] ContentViewModel
        //      The content view model of current content.
        currentContentViewModel: null,

        // masterContentViewModel: [public] ContentViewModel
        //      The content view model of master content.
        masterContentViewModel: null,

        // contentLinkSyncChange: [public] Boolean
        //      The flag to indicate that the current content link is changed.
        contentLinkSyncChange: false,

        // isTranslatingServiceAvailable: [public] Boolean
        //      The flag to indicate that the translating service is available or not.
        isTranslatingServiceAvailable: false,

        postscript: function () {
            // summary:
            //      Initialize stores
            // tags:
            //      protected

            this.inherited(arguments);

            var registry = dependency.resolve("epi.storeregistry");
            this.contentDataStore = this.contentDataStore || registry.get("epi.cms.contentdata");
            this.contextStore = this.contextStore || registry.get("epi.shell.context");
            this.contentVersionStore = this.contentVersionStore || registry.get("epi.cms.contentversion");
            this.languageManagerStore = this.languageManagerStore || registry.get("epi-languagemanager.language");

            when(this.languageManagerStore.executeMethod("IsTranslatingServiceAvailable", null, null),
                lang.hitch(this, function (result) {
                    this.set("isTranslatingServiceAvailable", result);
                })
            );
        },

        _contentLinkChanged: function (/*String*/oldContentLink, /*String*/newContentLink, /*ContentViewModel*/viewModel) {
            // summary:
            //      Patch content view model _contentLinkChanged method
            //      to avoid some un-wanted behaviour.
            // tags:
            //      private

            // store the latest newContentLink, could have changed during sync, ie when first changing a property
            viewModel.contentLink = newContentLink;

            viewModel.syncService.contentLink = newContentLink;

            // propagate new contentlink
            viewModel.validator.setContextId(newContentLink);

            ////////////////////////////////////////////////////////////////////////
            // Begin patch from EPi-CMS::ContentViewModel._contentLinkChanged method
            ////////////////////////////////////////////////////////////////////////
            var contentLink = newContentLink,
                newContextParams = { uri: "epi.cms.contentdata:///" + contentLink };
            all({
                currentContent: this.contentDataStore.get(contentLink),
                currentContext: this.contextStore.query(newContextParams),
            }).then(lang.hitch(this, function (result) {
                var currentContent = result.currentContent,
                    currentContext = result.currentContext;

                viewModel.set("contentData", currentContent);
                viewModel.set("languageContext", currentContext.languageContext);
                this.set("contentLinkSyncChange", true);

                // note that our view component, OPE or form must check if we're the sender?
                // or just skip when a content link changed
                var contextParameters = { uri: "epi.cms.contentdata:///" + newContentLink };
                connect.publish("/epi/shell/context/request",
                    [contextParameters, {
                        sender: this,
                        contentLinkSyncChange: true,
                        trigger: "internal",

                        // skip update view to avoid OPE reload when publish content
                        skipUpdateView: true,
                        viewType: "epi-languagemanager/component/CompareEditing"
                    }]);
            }));
            ////////////////////////////////////////////////////////////////////////
            // End patch from EPi-CMS::ContentViewModel
            ////////////////////////////////////////////////////////////////////////
        },

        _createViewModel: function (contentData, ctx) {
            // summary:
            //      Create new content view model instance.
            //      mixin _contentLinkChanged method.
            // tags:
            //      private


            // Create new content view model
            var viewModel = new ContentViewModel({
                contentLink: contentData.contentLink,
                contextTypeName: "epi.cms.contentdata",
                // TECHNOTE:
                //  + Commerce use this prop to check current content language or not.
                //      See "CatalogContentDetails.js" line 72
                //  + CMS doesn't (CMS use isCurrentLanguage that is returned from server (contentdata)
                //      See "ContentDetails.js" line 81
                currentContentLanguage: ctx.languageContext.language 
            }),
                self = this;

            // Update content view model
            viewModel.set("contentData", contentData);
            viewModel.set("languageContext", ctx.languageContext);

            // Mixin method
            lang.mixin(viewModel, {
                _contentLinkChanged: function (oldContentLink, newContentLink) {
                    self._contentLinkChanged(oldContentLink, newContentLink, viewModel);
                }
            });

            return viewModel;
        },

        _getAndUpdateViewModel: function (content, context) {
            // summary:
            //      Create and update content view model metadata.
            // tags:
            //      private

            var dfd = new Deferred();

            var viewModel = this._createViewModel(content, context);
            viewModel.getContentModelAndMetadata().then(lang.hitch(this, function () {
                dfd.resolve(viewModel);
            }));

            return dfd;
        },

        getCurrentContentViewModel: function () {
            // summary:
            //      Initializer content view model of current content.
            // tags:
            //      public virtual

            var dfd = new Deferred(),
                self = this;

            // Update model then return imerdiately when already have.
            if (this.currentContentViewModel) {
                this.currentContentViewModel.getMetadataThenUpdateModel().then(function () {
                    dfd.resolve(self.currentContentViewModel);
                });
                return;
            }

            all({
                currentContent: this.getCurrentContent(),
                currentContext: this.getCurrentContext()
            }).then(function (result) {
                var currentContent = result.currentContent,
                    currentContext = result.currentContext;

                self._getAndUpdateViewModel(currentContent, currentContext).then(function (viewModel) {
                    self.currentContentViewModel = viewModel;
                    dfd.resolve(viewModel);
                });
            });

            return dfd;
        },

        getMasterContentViewModel: function () {
            // summary:
            //      Initializer content view model of master content.
            // tags:
            //      public virtual

            var dfd = new Deferred(),
                self = this;

            // Update model then return imerdiately when already have.
            if (this.masterContentViewModel) {
                this.masterContentViewModel.getMetadataThenUpdateModel().then(function () {
                    dfd.resolve(self.masterContentViewModel);
                });
                return;
            }

            when(this.getCurrentContent(), function (currentContent) {
                var masterLanguageId = currentContent.existingLanguageBranches.filter(function (languageBranch) {
                    return languageBranch.isMasterLanguage;
                });

                // Workaround to get the master language.
                if (masterLanguageId.length == 0) {
                    masterLanguageId = currentContent.existingLanguageBranches;
                }

                when(self.contentVersionStore.query({
                    query: "getpublishedversion",
                    contentLink: masterLanguageId[0].commonDraftLink,
                    language: masterLanguageId[0].languageId
                }), function (publishedVersion) {

                    var contentLink = publishedVersion ? publishedVersion.contentLink : masterLanguageId[0].commonDraftLink;
                    var masterContextParams = { uri: "epi.cms.contentdata:///" + contentLink };

                    all({
                        masterContent: self.contentDataStore.refresh(contentLink),
                        masterContext: self.contextStore.query(masterContextParams),
                    }).then(function (result) {
                        var masterContent = result.masterContent,
                            masterContext = result.masterContext;

                        self._getAndUpdateViewModel(masterContent, masterContext).then(function (viewModel) {
                            lang.mixin(viewModel, {
                                canChangeContent: function () { return false; }
                            });
                            self.masterContentViewModel = viewModel;
                            dfd.resolve(viewModel);
                        });
                    });

                });
            });

            return dfd;
        },

        contextChanged: function (ctx, callerData) {
            // summary:
            //      Listen on context changed to update model
            // tags:
            //      protected override

            this.inherited(arguments);

            var self = this;
            this.getCurrentContent().then(function (currentContent) {
                self.currentContentViewModel.set("contentData", currentContent);
                self.currentContentViewModel.set("languageContext", ctx.languageContext);
                self.currentContentViewModel.set("contentLinkSyncChange", true);
            });;
        },
    });
});