epiJQuery(function ($) {

    // TECHNOTE: IE 8, 9 does not have console object
    if (typeof console == "undefined") {
        console = { log: function () { } };
    }

    $.fn.subfieldBehavior = function () {
        this.each(function () {
            var dependant = $(this).find("input");
            function setEnabled(enable) {
                if (enable)
                    dependant.removeAttr("disabled");
                else
                    dependant.attr("disabled", "disabled");
            };
            var initallyChecked = $(this).siblings().find(":checkbox").click(function () {
                setEnabled($(this).is(":checked"));
            }).is(":checked");
            setEnabled(initallyChecked);
        });
    }

    var controller = {
        initSubfieldBehavior: function () {
            $(".dependant").subfieldBehavior();
        }
    };

    var reduce = function (arr, valueInitial, fnReduce) {
        jQuery.each(arr, function (i, value) {
            valueInitial = fnReduce.apply(value, [valueInitial, i, value]);
        });
        return valueInitial;
    }

    var controller = {
        initSubfieldBehavior: function () {
            $(".dependant").each(function () {
                var dependant = $(this).find("input");
                function setEnabled(enable) {
                    if (enable)
                        dependant.removeAttr("disabled");
                    else
                        dependant.attr("disabled", "disabled");
                };
                var initallyChecked = $(this).siblings().find(":checkbox").click(function () {
                    setEnabled($(this).is(":checked"));
                }).is(":checked");
                setEnabled(initallyChecked);
            });
        }
    };

    var init = {
        shared: function () {
            $("a[target=small]").click(function (e) {
                e.preventDefault();
                // Open popup window name=small to authenticate with Google Account
                window.open(this.href, "small", "height=600,width=900,status=0,titlebar=0,toolbar=0");
            });
            try {
                $("#tabs").epiTabView({ tabPanelContainerClass: ".epi-topTabPanelContainer" });
            } catch (e) {
            }
        },
        tracking: function () {
            $(".epi-tabView").each(function () {
                try {
                    $(this).epiTabView({ tabPanelContainerClass: ".epi-settingsTabPanelContainer" });
                } catch (e) {
                }
            });

            var $trackingPanel = $("#trackingPanel");
            if ($trackingPanel.attr("data-shared") === "True") {
                $trackingPanel.addClass("ShareTracking");
            } else {
                $trackingPanel.addClass("DifferentTracking");
            }

            var showhideDomainsInput = function () {
                $(".domains").each(function () {
                    $this = $(this);
                    var $scriptOptionsDiv = $this.closest("div.auto-script").siblings("div.script-options");
                    var scriptOptionVal = $("input:checked", $scriptOptionsDiv).val();
                    var domainOptionVal = $(".radiooption input:checked", $this).val();

                    $("div.domains-input", $this).css("display", scriptOptionVal == "Universal" && domainOptionVal == "multidomains" ? "block" : "none");
                }
			)};

            $(".script-settings").each(function () {
                $this = $(this);
                switch ($this.attr("script-option")) {
                    case "Classic":
                        $this.addClass("ga-script");
                        break;
                    case "Universal":
                        $this.addClass("ua-script ua-selected");
                        break;
                    default:
                        $this.addClass("CustomScript");
                        break;
                }
            })

            showhideDomainsInput();

            $(".Sharing input").click(function () {
                var $shareDiv = $("#shared");
                var $differentDiv = $("#different");
                if (this.value === "True") {
                    if (!$shareDiv.is(":visible")) {
                        $shareDiv.slideDown();
                        $differentDiv.slideUp();
                    }
                } else {
                    if (!$differentDiv.is(":visible")) {
                        $differentDiv.slideDown();
                        $shareDiv.slideUp();
                    }
                }
                showhideDomainsInput();
            });

            $(".script-options input").click(function () {
                var $parentSettingsPanel = $(this).closest(".script-settings");
                var $customScriptDiv = $(".custom-script", $parentSettingsPanel);
                var $autoScriptDiv = $(".auto-script", $parentSettingsPanel)
                if (this.value == "Custom") {
                    if (!$customScriptDiv.is(":visible")) {
                        $customScriptDiv.slideDown();
                        $autoScriptDiv.slideUp();
                    }
                } else {
                    if (!$autoScriptDiv.is(":visible")) {
                        $autoScriptDiv.slideDown();
                        $customScriptDiv.slideUp();
                    }

                    var selectedScriptOption = this.value;
                    $(".domains .radiooption input:checked", $autoScriptDiv).each(function () {
                        if (this.value == "multidomains") {
                            // Show/hide the textbox for inputting cross domains. 
                            var $parentOptionDiv = $(this).closest(".option");
                            $("div.domains-input", $parentOptionDiv).css("display", selectedScriptOption == "Universal" ? "block" : "none");
                        }
                    })

                    if (selectedScriptOption == "Universal") {
                        $parentSettingsPanel.addClass("ua-selected");
                    }
                    else {
                        $parentSettingsPanel.removeClass("ua-selected");
                    }
                }
            });

            // Show domains input when in UA mode and multiple domains options selected, otherwise hide it
            $(".domains .radiooption input").click(function () {
                $this = $(this);
                var $parentSettingsPanel = $this.closest(".script-settings");
                if ($parentSettingsPanel.hasClass("ua-selected")) {
                    var $parent = $this.closest(".domains");
                    var $domainsinput = $(".domains-input", $parent);
                    $domainsinput.css("display", this.value == "multidomains" ? "block" : "none");
                }
            })

            controller.initSubfieldBehavior();
        },
        analytics: function () {
            // on connect callback
            window.userAuthenticatedCallback = function (/*Object*/data) {
                // response: [json]
                //  Possible response object structure:
                //      {
                //          username: [string],
                //          isShared: [boolean],
                //          responseResult: {
                //              status: [string],
                //              code: [string],
                //              message: [string],
                //              technicalMessage: [string]
                //          }
                //      }

                // Do nothing if we have any notification from server side,
                // and then log the responsed notification
                if (!data || data.responseResult) {
                    (data && data.responseResult)
                        && console.log(data.responseResult.technicalMessage);

                    return;
                }

                $("#disconnected").slideUp();
                $("#connected").slideDown();
                $("#GadgetSetup").removeClass("missing").addClass("ok");
            }

            // disconnect handling
            $("#signout").click(function (e) {
                e.preventDefault();
                $.post(this.href, {}, function (data) {
                    if (data.success) {
                        $("#connected").slideUp();
                        $("#disconnected").slideDown();
                        $("#GadgetSetup").removeClass("ok").addClass("missing");
                    }
                }, "json");
            });

            $("input.role").click(function () {
                var newValue = reduce($("input.role"), "", function (prev, i, box) {
                    if (!box.checked) {
                        return prev;
                    }

                    if (prev.length) {
                        return prev + ";" + box.name;
                    }

                    return box.name;
                });
                $("#SharedWith").val(newValue);
            });
            //// save form
            //$("#SaveForm").submit(function (e) {
            //    e.preventDefault();

            //    var $form = $(this);
            //    $form.addClass("saving");

            //    var postData = {
            //        "Authentication.SharedWith": reduce($form.find("input.role"), "", function (prev, i, box) {
            //            if (!box.checked)
            //                return prev;
            //            if (prev.length)
            //                return prev + ";" + box.name;
            //            return box.name;
            //        })
            //    };
            //    $.post($form.attr("action"), postData, function (data) {
            //        if (data.success) {
            //            $form.removeClass("saving");
            //        }
            //    }, "json");
            //});
        }
    };

    init.shared();
    init.tracking();
    init.analytics();

});