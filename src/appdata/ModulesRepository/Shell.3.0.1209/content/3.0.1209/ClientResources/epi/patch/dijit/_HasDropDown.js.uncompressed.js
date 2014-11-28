// Currently only a place holder for the core epi namespace
// Needed to get the amd loading properly defined, but should contain core epi parts

define("epi/patch/dijit/_HasDropDown", [
    "dojo/_base/array",
    "dojo/_base/lang",
    "dijit/_HasDropDown"
], function (array, lang, _HasDropDown) {
	// module:
	//		dijit/patch/form/_FormMixin
	// summary:
	//		Fix issue with null array when setting value to form

    lang.mixin(_HasDropDown.prototype, {
		_onBlur: function(){
			// summary:
			//		Called magically when focus has shifted away from this widget and it's dropdown

			// Don't focus on button if the user has explicitly focused on something else (happens
			// when user clicks another control causing the current popup to close)..
			// But if focus is inside of the drop down then reset focus to me, because IE doesn't like
			// it when you display:none a node with focus.
			var focusMe = focus.curNode && this.dropDown && dom.isDescendant(focus.curNode, this.dropDown.domNode);

            /* THE FIX GOES HERE */
            /* --------------------------------------------------------------------------------------------- */

            // Fire the public onBlur event before calling closeDropDown in which it may try to grab focus and fire onFocus event.
            // That leads to the situation that the next onFocus event happens before the last onBlur event.
			this.inherited(arguments);
			this.closeDropDown(focusMe);

            /* --------------------------------------------------------------------------------------------- */
            /* END FIX */
		},

        destroy: function(){
            /* THE FIX GOES HERE */
            /* --------------------------------------------------------------------------------------------- */

            // Apply dojo bug fix https://bugs.dojotoolkit.org/ticket/17275

            // If dropdown is open, close it, to avoid leaving dijit/focus in a strange state.
            // Put focus back on me to avoid the focused node getting destroyed, which flummoxes IE.
            if(this._opened){
                this.closeDropDown(true);
            }

            /* --------------------------------------------------------------------------------------------- */
            /* END FIX */

            if(this.dropDown){
                // Destroy the drop down, unless it's already been destroyed.  This can happen because
                // the drop down is a direct child of <body> even though it's logically my child.
                if(!this.dropDown._destroyed){
                    this.dropDown.destroyRecursive();
                }
                delete this.dropDown;
            }
            this.inherited(arguments);
        }
    });

    _HasDropDown.prototype._onBlur.nom = "_onBlur";
    _HasDropDown.prototype.destroy.nom = "destroy";
});