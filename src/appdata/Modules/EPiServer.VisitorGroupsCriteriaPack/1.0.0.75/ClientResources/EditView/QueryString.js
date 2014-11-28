(function () {
    // summary:
    //      The new editor for QueryString criteria.
    // module:
    //      "ClientResources/Criteria/QueryStringParameter"
    // description:
    //      That supports: Show / hide value row when condition's value is changed.
    // tags:
    //    public
    return {
        uiCreated: function (namingContainer) {
            this._updateUI(namingContainer);
        },

        createUI: function (namingContainer, container, settings) {
            this.prototype.createUI.apply(this, arguments);
        },

        _updateUI: function (/*String*/namingContainer) {
            // summary:
            //      Update UI after dom and dojo widgets are created.
            // tags:
            //      private

            require([
                // Dojo
                "dojo/aspect",
                "dojo/query",
                "dojo/NodeList-traverse",

                // Dijit
                "dijit/registry"
            ], function (
                // Dojo
                aspect,
                query,
                NodeListTraverse,

                // Dijit
                registry
            ) {
                var conditionWidget = registry.byId(namingContainer + "Condition"),
                    conditionValue = conditionWidget.get("value"),
                    valueWidget = registry.byId(namingContainer + "Value"),
                    valueRowNodeList = query(valueWidget.domNode).parents(".epi-critera-block");

                function updateUIElements(value) {
                    // Update UI when condition is changed.
                    if (value == "0") {
                        valueRowNodeList.style("display", "none");
                        valueWidget.set("value", "");
                    } else {
                        valueRowNodeList.style("display", "");
                    }
                }

                // Initialize UI
                updateUIElements(conditionValue);

                // Setup events
                aspect.after(conditionWidget, "onChange", updateUIElements, true);
            });
        }
    };
})();
