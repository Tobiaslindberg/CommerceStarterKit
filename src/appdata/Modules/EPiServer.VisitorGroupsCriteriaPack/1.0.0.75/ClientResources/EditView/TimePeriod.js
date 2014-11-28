(function () {
    // summary:
    //      The new controller for TimePeriod criteria.
    // description:
    //      Set min/max for start/end date and time, when values are changed. Ensure FromDateTime <= ToDateTime
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
                var startDateWidget = registry.byId(namingContainer + "StartDate"),
                    endDateWidget = registry.byId(namingContainer + "EndDate"),
                    startTimeWidget = registry.byId(namingContainer + "StartTime"),
                    endTimeWidget = registry.byId(namingContainer + "EndTime");
                function startDate() { return startDateWidget.get("value"); }
                function endDate() { return endDateWidget.get("value"); }
                function startTime() { return startTimeWidget.get("value"); }
                function endTime() { return endTimeWidget.get("value"); }

                function guaranteeTimePeriodCorrectness() {
                    var
                        currentDateTime = new Date(),
                        currentDate = new Date(
                            currentDateTime.getFullYear(),
                            currentDateTime.getMonth(),
                            currentDateTime.getDate(),
                            0, 0, 0);

                    startDateWidget.constraints.min = currentDate;
                    endDateWidget.constraints.min = currentDate;
                    if (startDate()) {
                        endDateWidget.constraints.min = startDate();
                    }
                    if (endDate()) {
                        startDateWidget.constraints.max = endDate();
                    }
                    else {
                        delete startDateWidget.constraints.max;
                    }


                    if (startDate() && endDate() && startDate().getTime() === endDate().getTime()) {
                        if (startTime()) {
                            endTimeWidget.constraints.min = startTime();
                        }
                        if (endTime()) {
                            startTimeWidget.constraints.max = endTime();
                        }
                        else {
                            delete startTimeWidget.constraints.max;
                        }
                    }
                    else {
                        delete startTimeWidget.constraints.max;
                        delete endTimeWidget.constraints.min;
                    }
                }

                guaranteeTimePeriodCorrectness();

                // Setup events
                aspect.after(startDateWidget, "onChange", function (startTime) {
                    guaranteeTimePeriodCorrectness();
                }, true);

                aspect.after(endDateWidget, "onChange", function (endTime) {
                    guaranteeTimePeriodCorrectness();
                }, true);
                aspect.after(startTimeWidget, "onChange", function (startTime) {
                    guaranteeTimePeriodCorrectness();
                }, true);

                aspect.after(endTimeWidget, "onChange", function (endTime) {
                    guaranteeTimePeriodCorrectness();
                }, true);

            });
        }
    };
})();
