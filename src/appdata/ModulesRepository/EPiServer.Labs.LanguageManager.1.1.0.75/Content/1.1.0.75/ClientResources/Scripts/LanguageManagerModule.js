define([
    "epi/_Module",
    "epi/dependency",
    "epi/routes",
    "epi/shell/HashWrapper"

], function (
    _Module,
    dependency,
    routes,
    HashWrapper

) {

    return dojo.declare([_Module], {

        // _settings: [private] Object
        //      Information which sent by LanguageManager module (in module.config file). We can read helpPath, moduleDependencies, routes, ... from here
        _settings: null,

        constructor: function (settings) {
            this._settings = settings;
        },

        initialize: function () {
            // summary:
            //		Initialize module
            //
            // description:
            //      Dependencies registered by this module are: 'LanguageManage application'

            this.inherited(arguments);

            // Initialize stores
            var registry = this.resolveDependency("epi.storeregistry");

            var route = this._getRestPath("language");

            registry.create("epi-languagemanager.language", route, { idProperty: "id" });
        },

        _getRestPath: function (name) {
            // summary:
            //      Get the rest path to a specified store.
            // prameters:
            //      name: The name of the store to get.
            // tags:
            //      private

            return routes.getRestPath({ moduleArea: "EPiServer.Labs.LanguageManager", storeName: name });
        }

    });
});
