define([
// Dojo base
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/Deferred",
    "dojo/topic",

// Epi Framework
    "epi/dependency",
    "epi/Url",
    "epi/shell/command/_Command",
    "epi/shell/TypeDescriptorManager",

// EPi CMS
    "epi-cms/contentediting/ContentActionSupport",
    "epi-cms/ApplicationSettings"

], function (
// Dojo base
    declare,
    lang,
    Deferred,
    topic,

// Epi Framework
    dependency,
    Url,
    _Command,
    TypeDescriptorManager,

// Epi CMS
    ContentActionSupport,
    ApplicationSettings

    ) {


    return declare([_Command], {

        isBlock: function (context) {
            // summary:
            //		Check the input context is Block or not.
            // tags:
            //		protected
            
            return (context && TypeDescriptorManager.isBaseTypeIdentifier(context.dataType, "episerver.core.blockdata"));
        },

        isPage: function (context) {
            // summary:
            //		Check the input context is Page or not.
            // tags:
            //		protected

            return context && context.capabilities.isPage;
        }
    });
});
