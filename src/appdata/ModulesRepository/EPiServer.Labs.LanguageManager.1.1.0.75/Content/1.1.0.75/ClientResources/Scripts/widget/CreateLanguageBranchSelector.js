define([
// Dojo
    "dojo/_base/declare",
    "dojo/_base/lang",

// Dojox
    "dojox/html/entities",

// Dijit
    "dijit/form/Button",
    "dijit/Menu", // used in template
    "dijit/registry",

// EPi CMS
    "epi-cms/widget/_ItemSelectorBase",

// Resouces
    "dojo/text!./templates/CreateLanguageBranchSelector.html"

], function (
// Dojo
    declare,
    lang,

// Dojox
    htmlEntities,

// Dijit
    Button,
    Menu,
    registry,

// EPi CMS
    _ItemSelectorBase,

// Resouces
    template
) {

    // module:
    //      "epi-languagemanager/widget/CreateLanguageBranchSelector"
    // summary:
    //      Used for selecting create language branch options for a page

    return declare([_ItemSelectorBase], {

        // templateString: [protected] String
        //      Html template for the slector
        templateString: template,

        itemClass: Button,

        itemSettings: {
            _setSelected: function () { }
        },

        postscript: function () {
            this.inherited(arguments);
        },

        postCreate: function () {
            this.inherited(arguments);
        },

        _createMenuItem: function (/*Object*/item) {
            // summary:
            //    Create a menu item.
            //
            // item:
            //    The item to create menu item.
            //
            // tags:
            //    protected override
            
            var menuItem = new this.itemClass(lang.mixin(this.itemSettings, {
                label: htmlEntities.encode(item.label),
                value: item.value,
                onClick: lang.hitch(this, function (evt) {
                    if (registry.getEnclosingWidget(evt.currentTarget) instanceof this.itemClass) {
                        this._onItemSelected(menuItem);
                    }
                })
            }));

            if (item.disabled === 'disabled') {
                menuItem.set('disabled', 'disabled');
            }

            return menuItem;
        },

        _setHeaderAttr: function (value) {
            this.header.innerHTML = value;
        }
    });
});