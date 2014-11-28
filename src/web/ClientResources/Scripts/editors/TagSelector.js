define([
    "dojo/_base/connect",
    "dojo/_base/declare",
    "dijit/_CssStateMixin",
    "dijit/_Widget",
    "dijit/_TemplatedMixin",
    "dijit/_WidgetsInTemplateMixin",
    "epi/dependency",
    "epi/epi",
    "epi/shell/widget/_ValueRequiredMixin",
    "epi-cms/widget/_HasChildDialogMixin",
    "epi/shell/_ContextMixin"
],
function (
    connect,
    declare,
    _CssStateMixin,
    _Widget,
    _TemplatedMixin,
    _WidgetsInTemplateMixin,
    dependency,
    epi,
    _ValueRequiredMixin,
    _HasChildDialogMixin,
    _ContextMixin

    // tagSelector
) {
    return declare("app.editors.TagSelector",
        [_Widget, _TemplatedMixin, _WidgetsInTemplateMixin, _CssStateMixin, _ValueRequiredMixin, _HasChildDialogMixin, _ContextMixin], {
            templateString:
		         "<div class='dijitInline'>\
                        <div data-dojo-attach-point='stateNode, tooltipNode'>\
                                <input type='text' data-dojo-attach-point='containerNode' />\
                        </div>\
                 </div>",
            intermediateChanges: false,
            value: null,
            store: null,
            onChange: function (value) {
                // Event that tells EPiServer when the widget's value has changed.
            },
            initUpdate: function (valueString) {
                var array = valueString.split(',');
                this._setValue(array, false);
            },
            postCreate: function () {
                this.inherited(arguments);
               
                var input = this.containerNode;
                var self = this;
                this._input = input;
                var context = this.getCurrentContext();
                var language = "no";
                if (context != null) {
                    language = context.language;
                }
                var apiUrlToUse = "";
                if (this.apiUrl) {
                    apiUrlToUse = "/" + language+this.apiUrl;
                    console.log(apiUrlToUse);
                }
                this.$select = $(this._input).select2({
                    width: '195px',
                    allowClear: true,
                    multiple: true,
                    id: function (e) {
                        return e.text;
                    },
                    ajax: {
                        url: apiUrlToUse,
                        data: function (term, page) {
                            return {
                                q: term
                            };
                        },
                        results: function (data) { // parse the results into the format expected by Select2.
                            // since we are using custom formatting functions we do not need to alter remote JSON data
                            var res = [];
                            $.each(data, function (ind, elem) {
                                res.push({
                                    text: elem.text
                                });
                            });
                            var result = { results: res };
                            return result;
                        }
                    }
                });
                if (this.value && this.value.length) {
                    var array = [];
                    $.each(this.value, function (ind, elem) {
                        array.push({
                            text: elem
                        });
                    });
                    this.$select.select2("data", array);
                }
                
                this.$select.on('select2-opening', function () {
                        self.isShowingChildDialog = true;
                });
                this.$select.on('change', function () {
                    self.initUpdate($(this).val());
                    self.isShowingChildDialog = false;
                });

            },
            isValid: function () {
                // summary:
                //    Check if widget's value is valid.
                // tags:
                //    protected, override
                return !this.required || this.value.length > 0;
            },

            // Setter for value property
            _setValueAttr: function (value) {
                this._setValue(value, true);
            },

            _onTextChanged: function (value) {
                this._setValue(value, false);
            },
            _setValue: function (value, updateWidget) {
                // set value to this widget (and notify observers)
                this._set("value", value);
                // set value to control
                if (this._started && this.validate()) {
                    // Trigger change event
                    this.onChange(value);
                }
            }
    });
});