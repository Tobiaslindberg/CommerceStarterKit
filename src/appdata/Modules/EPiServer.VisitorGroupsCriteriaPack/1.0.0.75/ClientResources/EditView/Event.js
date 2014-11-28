(function () {
    // summary:
    //      The new controller for Event criteria.
    // description:
    //      That supports: Set min/max for start/end date when value is changed.
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

                // Dijit
                "dijit/registry"
            ], function (
                // Dojo
                aspect,

                // Dijit
                registry
            ) {
                var startTimeWidget = registry.byId(namingContainer + "StartTime"),
                    startTimeValue = startTimeWidget.get("value"),
                    endTimeWidget = registry.byId(namingContainer + "EndTime"),
                    endTimeValue = endTimeWidget.get("value"),
                    repeatTypeWidget = registry.byId(namingContainer + "RepeatType");
                function startTime()    { return startTimeWidget.get("value");  }
                function endTime()      { return endTimeWidget.get("value");    }
                function repeatType()   { return repeatTypeWidget.get("value"); }

                function guaranteeTimePeriodCorrectness() {
                    var
                        currentDateTime = new Date(),
                        currentDate = new Date(
                            currentDateTime.getFullYear(),
                            currentDateTime.getMonth(),
                            currentDateTime.getDate(),
                            0, 0, 0);

                    startTimeWidget.constraints.min = currentDate;
                    endTimeWidget.constraints.min = currentDate;

                    if (startTime()) {
                        endTimeWidget.constraints.min = startTime();
                    }
                    if (endTime()) {
                        startTimeWidget.constraints.max = endTime();
                    }
                    else {
                        delete startTimeWidget.constraints.max;
                    }

                    if (repeatType() == '0') {  // never repeat
                        endTimeWidget.set("disabled", false);
                    }
                    else {
                        endTimeWidget.set("disabled", true);
                        endTimeWidget.set("value", startTime());
                        delete startTimeWidget.constraints.max;
                    }
                }

                guaranteeTimePeriodCorrectness();

                // Setup events
                aspect.after(startTimeWidget, "onChange", function (startTime) {
                    guaranteeTimePeriodCorrectness();
                }, true);

                aspect.after(endTimeWidget, "onChange", function (endTime) {
                    guaranteeTimePeriodCorrectness();
                }, true);

                aspect.after(repeatTypeWidget, "onChange", function (repeatType) {
                    guaranteeTimePeriodCorrectness();
                }, true);
            });
        }
    };
})();
