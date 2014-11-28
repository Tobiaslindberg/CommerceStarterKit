define("epi/patch/dijit/form/NumberSpinner", [
// Dojo
    "dojo/_base/lang",

// Dijit
    "dijit/form/NumberSpinner"
], function (
// Dojo
    lang,

// Dijit
    NumberSpinner
    ) {

    // summary:
    //    The EPiServer NumberSpinner widget patch.
    //
    // description:
    //    We need patch dijit.form.NumberSpinner to customize it's default behaviour
    //    when it's adjust value and current value is Not a Number. See bug 85501 for more details.
    //

    lang.mixin(NumberSpinner.prototype, {

        adjust: function (/*Object*/val, /*Number*/delta) {
            // summary:
            //		Change Number val by the given amount
            // tags:
            //		protected

            var tc = this.constraints,
                v = isNaN(val),
                gotMax = !isNaN(tc.max),
                gotMin = !isNaN(tc.min);

            if (v && delta != 0) { // blank or invalid value and they want to spin, so create defaults
                /* THE FIX GOES HERE */
                /* --------------------------------------------------------------------------------------------- */
                val = delta;
                /* --------------------------------------------------------------------------------------------- */
                /* END FIX */
            }
            var newval = val + delta;
            if (v || isNaN(newval)) { return val; }
            if (gotMax && (newval > tc.max)) {
                newval = tc.max;
            }
            if (gotMin && (newval < tc.min)) {
                newval = tc.min;
            }
            return newval;
        }
    });

    NumberSpinner.prototype.adjust.nom = "adjust";
});
