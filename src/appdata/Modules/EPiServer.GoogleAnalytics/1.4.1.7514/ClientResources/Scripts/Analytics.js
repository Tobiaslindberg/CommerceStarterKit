(function ($) {

    if (typeof epi == "undefined") epi = {};

    // TECHNOTE: IE 8, 9 does not have console object
    if (typeof console == "undefined") {
        console = { log: function () { } };
    }

    epi.googleAnalytics = {};

    epi.googleAnalytics.notSupportContentTypes = [];

    epi.googleAnalytics.resize = function () {
        /// <summary>resize the gadget height to appropriate size, enough to display the chart and the list</summary>

        var gadgetElement = epi.googleAnalytics.gadget.element,
            gadgetContainerWidget = epi.googleAnalytics._getGadgetContainerWidget(gadgetElement);

        if (!gadgetContainerWidget || typeof gadgetContainerWidget.resize !== "function") {
            return;
        }

        gadgetContainerWidget.resize({ h: epi.googleAnalytics._getNewHeight(gadgetElement) });
    };

    epi.googleAnalytics.initContext = function (e, gadget) {
        /// <summary>ClientScriptInitMethod, initialize widget on the EDIT VIEW</summary>

        /// inits workingInDashboard value so that it is ready for reloading 
        epi.googleAnalytics.workingInDashboard = false;

        epi.googleAnalytics.init(e, gadget, function (controller) {

            var gadgetElement = controller.gadget.element,
                $gadgetElement = $(gadgetElement);

            var widgetId = epi.gadget.gadgetIdHash[controller.gadget.id];
            if (!widgetId) {
                return;
            }

            var widget = dijit.byId(widgetId); // epi.shell.widget.GadgetWrapper
            // make the widget own the context change event handler so that the event handler will be un-registered when the widget is disposed
            widget.own(
                dojo.subscribe("/epi/shell/context/changed", function (ctx, sender) {
                    if (ctx.id) {
                        controller.reload();
                    }
                })
            );

            $gadgetElement.bind("graphdatareceived", function (e, data) {
                controller.updateHeading(data.PageName);
            });

            $gadgetElement.bind("listhtmlreceived", function () {
                $(".add-links-form", gadgetElement).submit(function (e) {
                    // enable submit behavior on add-links forms
                    e.preventDefault();

                    controller.gadget.ajax({
                        type: "POST",
                        url: controller.gadget.getActionPath({ action: "AddLinks" }),
                        data: $(this).serialize(),
                        dataType: "json",
                        success: function (data) {
                            if (data.isSuccess) {
                                var contextParameters = { uri: "epi.cms.contentdata:///" + data.savedLink };
                                dojo.publish("/epi/shell/context/request", [contextParameters, { sender: this, forceContextChange: true, trigger: "navigation" }]);
                            }
                        }
                    });
                });

                $(".add-links").each(function () {
                    // select property in drop down to add links to
                    $(this).children(".add-links-options")
                        .epiContextMenu({ attachedTo: $(this).children(".add-links-button"), clickSelector: this })
                        .click(function (e) {
                            $(e.target).closest(".option").each(function () {
                                var form = $(this).closest(".add-links-form");
                                form.find(".add-links-property").val($(this).attr("data-property"));
                                form.find(".add-links-page").val(currentNavigate || getLatestNavigate());
                                form.submit();
                            });
                        });
                }).children(".add-links-button").click(function (e) {
                    e.preventDefault();
                });

                $(".add-link-button").click(function (e) {
                    // add to only available link collection
                    e.preventDefault();

                    var form = $(this).closest(".add-links-form");
                    form.find(".add-links-page").val(currentNavigate || getLatestNavigate());
                    form.submit();
                });
            });
        });
    };

    epi.googleAnalytics.initDashboardGadget = function (e, gadget) {
        /// <summary>ClientScriptInitMethod, initialize global gadget on the dashboard</summary>

        epi.googleAnalytics.workingInDashboard = true;
        epi.googleAnalytics.init(e, gadget);

        var $gadgetElement = $(gadget.element);

        $gadgetElement.bind("graphdatareceived", epi.googleAnalytics.resize);
        $gadgetElement.bind("listhtmlreceived", epi.googleAnalytics.resize);
        $gadgetElement.bind("epigadgetloaded", epi.googleAnalytics.resize);

        epi.googleAnalytics.resize();
    };

    epi.googleAnalytics.init = function (e, gadget, callback) {
        require(["dojo/aspect", "dojo/dom-attr", "dojo/ready"], function (aspect, domAttr, ready) {
            gadget.clearFeedbackMessage = function () { };
            epi.googleAnalytics.gadget = gadget;

            // asynchronously performs initialization on the gadget
            // an object instantiate from epi.googleAnalytics.controller "class" is pass back in the callback of defineController()
            epi.googleAnalytics.defineController(gadget, function (controller) {
                var gadgetElement = controller.gadget.element,
                    $gadgetElement = $(gadgetElement);

                controller.connectSignInSignOut();

                var onGadgetLoaded = function (e) {
                    $gadgetElement.closest(".epi-gadgetInner").attr("title", "");
                    $gadgetElement.closest(".dijitContainer").attr("title", "");

                    // Make sure DOM is ready, then watch selected state of tabs.
                    // If not, tabContainer may be undefined when we try to get by its dom node.
                    ready(function () {
                        $("div.ga-tabbed-graph", gadgetElement).each(function () {
                            var tabContainer = dijit.byNode(this);
                            if (!tabContainer) {
                                return;
                            }

                            // TECHNOTE:
                            //      For special case, the tab name contains XSS code, like: "<script></script>", dojo tab name will not render correctly.
                            //      So that, we should bind tab name manually.
                            //      The tab name will set as text, not inner HTML.
                            var tabList = tabContainer.tablist.getChildren(),
                                tabContentList = tabContainer.getChildren();
                            if (tabContentList instanceof Array && tabContentList.length > 0) {
                                var tabName = "";
                                $.each(tabContentList, function (index, item) {
                                    tabName = domAttr.get(item.domNode, "data-tab-name").toString();
                                    $(tabList[index].domNode).text(tabName);
                                });
                            }

                            // Resize tab container incase the gadget had resize
                            var dijitGadget = epi.googleAnalytics._getGadgetContainerWidget(gadgetElement);
                            if (dijitGadget) {
                                aspect.after(dijitGadget, "layout", function () {
                                    if (tabContainer.domNode) {
                                        tabContainer.resize();
                                    }
                                });
                            }

                            tabContainer.watch("selectedChildWidget", function (before, after) {
                                controller.loadGraphDataForVisibleGraphs(controller.cache.graphData.HashCode);
                            });
                        });
                    });

                    if ($gadgetElement.find(".epi-googleanalytics-configuration").length) {
                        // a config view
                        controller.connectAccountSelection(gadget);
                        $(".dependant", gadgetElement).subfieldBehavior();
                        controller.cache = {};
                        // hiding the prior message if any
                        epi.googleAnalytics._showGadgetContent(gadgetElement);
                    }
                    else {
                        // a graph display view
                        controller.connectSignIn("Index");

                        // fix issue with width being changed when loading
                        $gadgetElement.find(".epi-gadgetContent").addClass("epi-googleanalytics-gadgetContent");

                        controller._initTitlePanes(gadgetElement);

                        controller.initDates();
                        controller.initGraphBy();
                        controller.initChartTabs();
                        controller.segmentsOutDated = true; // set flag to refresh segments

                        $(".epi-googleanalytics-signedin", gadgetElement).each(function () {
                            controller.reload();
                        });
                    }
                };

                if ($(".epi-googleanalytics", gadgetElement).length) {
                    onGadgetLoaded();
                }
                $gadgetElement.bind("epigadgetloaded", onGadgetLoaded);

                if (typeof callback === "function") {
                    callback(controller);
                }
            });
        });
    };

    epi.googleAnalytics.defineController = function (gadget, callback) {
        // loads dojo resources and defines the gadget's client controller
        require([
            // dojo
            "dojo",
            "dojo/_base/lang",
            "dojo/aspect",
            "dojo/dom-geometry",
            "dojo/parser",

            "dijit/layout/ContentPane",
            "dijit/TitlePane",
            "dijit/Tooltip",

            "dojox/charting/action2d/Tooltip",
            "dojox/charting/Chart2D",
            "dojox/charting/Theme",

            // epi
            "epi/shell/widget/layout/ComponentTabContainer",
            "epi/shell/TypeDescriptorManager",
            // res
            "epi/i18n!epi/cms/nls/episerver.googleanalytics"
        ],
        function (
            // dojo
            dojo,
            lang,
            aspect,
            domGeometry,
            parser,

            ContentPane,
            dijitTitlePane,
            dijitTooltip,

            Tooltip,
            Chart2D,
            Theme,

            // epi
            TabContainer,
            TypeDescriptorManager,
            res
        ) {
            var contextService = epi.dependency.resolve("epi.shell.ContextService");

            if (typeof console == "undefined") {
                console = { log: function () { } };
            }

            $.fn.subfieldBehavior = function () {
                this.each(function () {
                    var dependant = $(this).find("input");
                    function setEnabled(enable) {
                        if (enable) {
                            dependant.removeAttr("disabled").parent().removeClass("disabled-input");
                        } else {
                            dependant.attr("disabled", "disabled").parent().addClass("disabled-input");
                        }
                    };
                    var initallyChecked = $(this).siblings().find(":checkbox").click(function () {
                        setEnabled($(this).is(":checked"));
                    }).is(":checked");
                    setEnabled(initallyChecked);
                });
            };

            var ua = navigator.userAgent;
            $.browser.ie11 = ua.indexOf('Trident') != -1 ? true : false;
            $.browser.chrome = /chrom(e|ium)/.test(ua.toLowerCase());
            $.browser.mozilla = $.browser.mozilla && !$.browser.ie11;

            // "epi-ga-signout" event is fired after a signedout request completed.
            // "epi-ga-signin" event is fired after a signedin request (to serverside) completed.
            epi.googleAnalytics.controller = function (gadget) {
                /// <summary> declare epi.googleAnalytics.controller "class"</summary>

                var controller = this;
                controller.gadget = gadget;
                controller.cache = {};

                var gadgetElement = controller.gadget.element,
                    $gadgetElement = $(gadgetElement);

                controller.eventually = (function () {
                    // clears the previous action if a new event is triggered before the timeout
                    var timer = 0;
                    return function (callback, ms) {
                        clearTimeout(timer);
                        timer = setTimeout(callback, ms);
                    }
                })();

                controller.createChart = function (element, chartType) {
                    var chart = new Chart2D(element, {
                        margins: { l: 5, r: 10, t: 10, b: 10 }
                    });
                    chart.setTheme(new Theme({
                        colors: ["#005bad", "#bf1313", "#69bf13", "#13bfbf"],
                        axis: {
                            stroke: { width: .5, color: "#ddd" },
                            tick: { width: 1, color: "#ddd", position: "center", font: "normal normal normal 7pt LucidaGrande", fontColor: "#666" },
                            majorTick: { position: "center", color: "#ccc", width: 1, length: 10, style: "Dot" }
                        },
                        series: { stroke: { width: 4, color: "#005bad" }, outline: { width: .5, color: "#fff" } },
                        marker: { stroke: { width: 1, color: "#005bad" }, outline: { width: .5, color: "#fff" } },
                        seriesThemes: [{ fill: "#3da4fb" }],
                        markerThemes: [{ fill: "#005bad" }],
                    }));
                    chart.addPlot("grid", { type: "Grid", hMajorLines: true, vMajorLines: true });
                    if (chartType == "bar") {
                        chart.addPlot("default", { type: "Columns", markers: true });
                    }
                    else /* "line" */ {
                        chart.addPlot("default", { type: "Areas", markers: true });
                    }
                    chart.chartType = chartType;
                    chart.addAxis("x", {
                        labelFunc: function (l, i, x) { return l; }
                    });
                    chart.addAxis("y", { vertical: true, fixUpper: "major", min: 0 });

                    // add tooltip to the chart
                    var format = "<div class='epi-googeanalytics-tooltip'><div><label>"
                        + $(chart.node).attr("data-date-label")
                        + "</label><span>{date}</span></div>"
                        + "<div><label>"
                        + $(chart.node).attr("data-metric-label")
                        + "</label><span>{metric}</span></div></div>";
                    chart.tip = new Tooltip(chart, "default", {
                        showDelay: 0,
                        text: function (o) {
                            var regexExpression = new RegExp("<[^>]*>"),
                                y = ((o.chart.axes.y.opt.labelFunc)
                                        ? o.chart.axes.y.opt.labelFunc(o.y.toString(), o.y, Math.floor(o.x), true)
                                        : o.y)
                                    .replace(regexExpression, ""),

                            // x actually differs depending on the chart type
                                offset = o.chart.chartType == "bar" ? 0 : -1,
                                x = ((o.chart.axes.y.opt.tooltipFunc)
                                        ? o.chart.axes.y.opt.tooltipFunc(o.y.toString(), o.y, Math.floor(o.x))
                                        : o.chart.axes.x.labels[Math.floor(o.x) + offset].text)
                                    .replace(regexExpression, "");

                            return format.replace(/{date}/, x).replace(/{metric}/, y);
                        }
                    });

                    // Resize the visible chart incase the gadget had resize
                    var dijitGadget = epi.googleAnalytics._getGadgetContainerWidget(gadgetElement);
                    if (dijitGadget) {
                        aspect.after(dijitGadget, "layout", function () {
                            epi.googleAnalytics._resizeChart(gadgetElement, chart);
                        });
                    }

                    // workaround strange IE bug where setting width to offsetWidth causes words to wrap
                    // this fix will be removed after this bug is fixed on next CMS's release.
                    var handler = aspect.after(dijitTooltip, "show", function () {
                        // Run once
                        handler.remove();

                        var context = dijitTooltip._masterTT;
                        if (context) {
                            aspect.around(context, "orient", function (originalMethod) {
                                return function () {
                                    var result = originalMethod.apply(context, arguments);
                                    if ($.browser.msie || $.browser.ie11) {
                                        domGeometry.setMarginBox(this.domNode, {
                                            w: domGeometry.position(this.domNode).w + 2
                                        });
                                    }
                                    return result;
                                }
                            });
                        }
                    });

                    return chart;
                };

                controller.applyDynamicSegment = function (settings) {
                    if (!contextService.currentContext) {
                        return settings;
                    } else {
                        return $.extend({ id: contextService.currentContext.id }, settings);
                    }
                };

                controller.loadChartData = function (input, callback) {
                    var url = this.gadget.getActionPath(controller.applyDynamicSegment({ action: "GraphData" }));
                    this.gadget.ajax({
                        type: "POST",
                        url: url,
                        data: input || {},
                        dataType: "json",
                        success: function (data) {
                            controller.cache.graphData = data;
                            controller.loadGraphDataForVisibleGraphs(data.HashCode);

                            if (typeof callback === "function") {
                                callback();
                            }

                            $gadgetElement.trigger("graphdatareceived", data);
                        },
                        error: function (xhr, errorType, exception) { //Triggered if an error communicating with server or not expected dataType
                            if (errorType == "parsererror" && $(".errorinfo", xhr.response).length) {// Exception on the server is catched and return html result causes this
                                // set flag to refresh segments
                                // calling controller.initSegments here will cause a bad cycling
                                controller.segmentsOutDated = true;
                                epi.googleAnalytics._showFeedbackMessage(xhr.response, gadgetElement);
                                return;
                            }
                        }
                    });
                };

                controller.loadSummary = function (data) {
                    $(".summary", gadgetElement).load(this.gadget.getActionPath(controller.
                        applyDynamicSegment($.extend({ action: "SummaryView", r: Math.random() }, data))),
                        function (responseText, textStatus, xhr) {
                            controller._validateAndShowError(responseText, gadgetElement);
                        });
                };

                controller.initChartTabs = function () {
                    // use the graph containers as source for tabs
                    $gadgetElement.find(".tab").each(function () {
                        var a = $("<a href='#' class='epi-tabView-tab' role='tab' tabindex='0'/>").html($(this).attr("data-tab-text")),
                            li = $("<li class='epi-tabView-navigation-item'/>").append(a);

                        $(this).closest(".epi-gaTabPanelContainer").siblings(".epi-tabView-navigation").append(li);
                    });

                    // select first tabs
                    $gadgetElement.find("ul.epi-tabView-navigation").find("li:first a").each(function () {
                        $(this).attr("aria-selected", true);
                        $(this).parent().removeClass("epi-tabView-navigation-item").addClass("epi-tabView-navigation-item-selected");
                    });

                    $(document).bind("layoutchange", function () {
                        controller.eventually(function () {
                            $gadgetElement.find(".metric:visible").each(function () {
                                var chart = $(this).data("loadedChart");
                                if (chart != null) {
                                    epi.googleAnalytics._resizeChart(gadgetElement, chart);
                                }
                            });
                        }, 100);
                    });

                    $gadgetElement.closest(".epi-gadgetContent").css("clear", "both");

                    // widgetize & listen to changes!
                    try {
                        $gadgetElement
                            .find(".epi-tabView").bind("epitabshow", function (e) {
                                if (controller.cache.graphData) {
                                    controller.loadGraphDataForVisibleGraphs(controller.cache.graphData.HashCode);
                                }
                            }).epiTabView({ tabPanelContainerClass: ".epi-gaTabPanelContainer" });
                    } catch (e) {
                        //TODO uncomment: console.log(e);
                    }
                };

                controller.reloadLabels = function (chart, s) {
                    // BUILD the X Axis
                    chart.removeAxis("x");

                    // transform s.Labels
                    var dataPointLabels;
                    if (s.Dimension === "ga:nthWeek") {
                        dataPointLabels = $.map(s.Tooltips, function (l, i) {
                            return { value: i, text: l };
                        });
                    }
                    else {
                        dataPointLabels = $.map(s.Labels, function (l, i) {
                            return { value: i, text: l };
                        });
                    }

                    xAxisLabels = $.map(s.Labels, function (l, i) {
                        return { value: i, text: l };
                    });
                    chart.addAxis("x", {
                        labels: dataPointLabels,
                        minorLabels: false,
                        labelFunc: function (x, y, z) {
                            var prefix = !($.browser.msie || $.browser.ie11) ? "<br/>" : "", // HACK to render X labels in correct position
                                xAxisLabelItem = xAxisLabels[y - 1];

                            // label for x Axis (under)
                            return prefix + (xAxisLabelItem ? xAxisLabelItem.text : "");
                        }
                    });

                    function formatDecimal(val) {
                        var decimal = Math.floor(100 * (val - Math.floor(val)));
                        if (decimal < 1) {
                            return "00";
                        }
                        else if (decimal < 10) {
                            return "0" + decimal.ToString();
                        }
                        else {
                            return decimal.ToString();
                        }
                    }

                    // BUILD the Y Axis
                    chart.removeAxis("y");
                    if (s.Type == "time") {
                        var tickStep = s.Max < 60 ? 10 : Math.floor(s.Max / 600) * 60 + 60;
                        chart.addAxis("y", {
                            min: 0,
                            fixUpper: "major",
                            vertical: true,
                            minorLabels: false,
                            majorTickStep: tickStep,
                            max: (s.Max < 30) ? 30 : (s.Max < 60 ? 60 : s.Max + 30),
                            labelFunc: function (str, val, x) {
                                var prefix = !($.browser.msie || $.browser.ie11) ? "<br/>" : "", // HACK to render Y labels in correct position
                                    s = val % 60;
                                return prefix + (Math.floor(val / 60) + ":" + ((s < 10) ? ("0" + s) : s));
                            },
                            tooltipFunc: function (str, val, x) {
                                // x actually differs depending on the chart type
                                var offset = chart.chartType == "bar" ? 0 : -1;
                                return s.Tooltips[x + offset];
                            }
                        });
                    }
                    else if (s.Type == "percentage") {
                        chart.addAxis("y", {
                            vertical: true,
                            fixUpper: "major",
                            min: 0,
                            max: (s.Max < 10) ? 10 : ((s.Max + 10) > 100 ? 100 : (s.Max + 10)),
                            labelFunc: function (str, y, x, fromTooltip) {
                                var prefix = (($.browser.mozilla || $.browser.chrome) ? "<br/>" : ""), // HACK to render Y labels in correct position
                                    label = s.DataFormat.replace(/{integer}/, Math.floor(y)).replace(/{decimal}/, formatDecimal(y)) + "%";
                                if (fromTooltip) {
                                    return label;   // label of metric (Exit Rate) in tooltip of datapoint
                                }
                                return prefix + label;  // label for y Axis (on the left)
                            },
                            tooltipFunc: function (str, val, x) {
                                // x actually differs depending on the chart type
                                var offset = chart.chartType == "bar" ? 0 : -1;
                                return s.Tooltips[x + offset];
                            }
                        });
                    }
                    else {
                        // this case is s.Type == "string"
                        chart.addAxis("y", {
                            vertical: true,
                            fixUpper: "major",
                            min: 0,
                            max: (s.Max < 10) ? 10 : s.Max,
                            labelFunc: function (str, y, x, fromTooltip) {
                                var prefix = (($.browser.mozilla || $.browser.chrome) ? "<br/>" : ""), // HACK to render Y labels in correct position
                                    label = s.DataFormat.replace(/{integer}/, Math.floor(y)).replace(/{decimal}/, formatDecimal(y));
                                if (fromTooltip) {
                                    return label;   // label of metric (Visits) in tooltip of datapoint
                                }
                                return prefix + label;  // label for y Axis (on the left)
                            },
                            tooltipFunc: function (str, val, x) {
                                // x actually differs depending on the chart type
                                var offset = chart.chartType == "bar" ? 0 : -1;
                                return s.Tooltips[x + offset];
                            }
                        });
                    }
                };

                controller.loadGraphDataForVisibleGraphs = function (hashCode) {
                    var gd = controller.cache.graphData;
                    if (!gd || !gd.IsAuthenticated) {
                        return;
                    }

                    $gadgetElement.find(".metric").each(function () {
                        var $closestGraph = $(this).closest(".graph");
                        var $notification = $closestGraph.find(".notenoughdata");
                        $closestGraph.removeClass("loading");
                        for (var i in gd.Sets) {
                            var s = gd.Sets[i];
                            if (s.Metric.Name === $(this).attr("data-name")) {
                                var chart = $(this).data("loadedChart");

                                if (!chart) {
                                    // fist time load
                                    chart = controller.createChart(this, gd.ChartType);
                                    $(this).data("loadedChart", chart);

                                    controller.reloadLabels(chart, s);
                                    chart.addSeries("chartData", s.Data, { marker: "m-3.5,0 c0,-5 7,-5 7,0 m-7,0 c0,5 7,5 7,0" });
                                    chart.render();
                                }
                                else if ($(this).data("loadedData") !== hashCode) {
                                    // other data on chart, reload
                                    $(this).data("loadedData", hashCode);

                                    controller.reloadLabels(chart, s);
                                    chart.updateSeries("chartData", s.Data);
                                    chart.render();
                                }
                                else {
                                    // same data already loaded
                                }

                                epi.googleAnalytics._resizeChart(gadgetElement, chart);

                                if (s.Data.length <= 1) {
                                    $notification.text(res.dashboard.analyze.graphs.notenoughdata);
                                    $notification.fadeIn();
                                    $closestGraph.find(".chart").css("visibility", "hidden");
                                } else {
                                    $notification.hide();
                                    $closestGraph.find(".chart").css("visibility", "visible");
                                }

                                return;
                            }
                        }
                        $notification.text(res.shared.message.nodata);
                        $notification.fadeIn();
                        $closestGraph.find(".chart").css("visibility", "hidden");

                        console.log("No data found for: ", gd, this);
                    });

                    $gadgetElement.trigger("graphdataloaded", gd);
                };

                controller.selectAccount = function (accountName) {
                    var wpSelector = $(".webProperty", gadgetElement);

                    if (!accountName) {
                        wpSelector.find("optgroup,option").remove();
                        wpSelector.append("<option value=''>" + wpSelector.attr("data-signintoselect-text") + "</option>");
                        wpSelector.attr("disabled", "disabled");
                        return;
                    }

                    var url = this.gadget.getActionPath({ action: "GetProfiles" });
                    $.post(url, { accountName: accountName }, function (profiles) {
                        wpSelector.removeAttr("disabled");
                        wpSelector.find("optgroup,option").remove();

                        var selected = wpSelector.val();
                        $.each(profiles, function (i, p) {
                            var $og = $("<optgroup/>").attr("label", p.Name);
                            $.each(p.WebProperties, function (i, wp) {
                                var $o = $("<option/>").attr("value", wp.TableId).text(wp.DefaultUrl + " - " + wp.Name);
                                $o.appendTo($og);
                            });
                            $og.appendTo(wpSelector);
                        });
                    }, "json");
                };

                controller.onAccountsChanged = function () {
                    /// <summary>this will be called after a Google account is signed in or signed out</summary>
                    // console.debug('controller.onAccountsChanged');

                    var selectedAccount = $(".account", gadgetElement).val(),
                        personalAuthenticated = $(".authentication", gadgetElement).attr("data-personal-authenticated") == "True";
                    //console.log('selectedAccount', selectedAccount, 'personalAuthenticated', personalAuthenticated);
                    $(".signin,.signout", gadgetElement).hide();

                    if (selectedAccount == "Personal") {
                        if (personalAuthenticated) {
                            $(".signout", gadgetElement).show();
                        } else {
                            $(".signin", gadgetElement).show();
                        }
                    }
                    else if (selectedAccount === undefined) {
                        // console.log('only happen when click SignIn in Dashboard Gadget IndexView');
                        controller.gadget.loadView({ action: "Index" });
                    }
                };

                controller.connectSignInSignOut = function () {
                    $(document).bind("epi-ga-signin", function (e, signedInAccount) {
                        // console.log('epi-ga-signin event handler');

                        $(".epi-googleanalytics-signedout", gadgetElement).each(function () {
                            if ($(this).attr("data-account-shared") === signedInAccount) {
                                var signedOutGadget = epi.gadget.getByElement(e.target);
                                if (signedOutGadget.id !== controller.gadget.id) {
                                    // reload index since another gadget signed in the same account this gadget is using
                                    controller.gadget.loadView({ action: "Index" });
                                }
                            }
                        });

                        $(".authentication", gadgetElement).attr("data-personal-authenticated", "True");
                        controller.onAccountsChanged();
                    });
                    $(document).bind("epi-ga-signout", function (e, signedOutAccount) {
                        // console.log('epi-ga-signout event handler');

                        if (e.target === gadgetElement) {
                            return;
                        }

                        $(".epi-googleanalytics-signedin", gadgetElement).each(function () {
                            if ($(this).attr("data-account-shared") === signedOutAccount) {
                                var otherGadget = epi.gadget.getByElement(e.target);
                                if (otherGadget.id !== controller.gadget.id) {
                                    $(this).find(".signedout").height($(this).height());
                                    $(this).removeClass("epi-googleanalytics-signedin").addClass("epi-googleanalytics-signedout");
                                }
                            }
                        });

                    });
                };

                controller.connectSignIn = function (defaultView) {
                    /// <summary>connect event for signin button</summary>

                    $(".signin", gadgetElement).click(function (e) {
                        e.preventDefault();

                        /// <summary>Setup a function here, to be the callback for the GoogleAuthentication's popup window</summary>
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

                            // only run once destroy immediately after calling
                            window.userAuthenticatedCallback = null;

                            // Do nothing if we have any notification from server side,
                            // and then log the responsed notification
                            if (!data || data.responseResult) {
                                (data && data.responseResult)
                                    && console.log(data.responseResult.technicalMessage);

                                return;
                            }

                            var accountName = data.username;

                            // POST to serverside, to save the accountName to setting
                            controller.gadget.ajax({
                                type: "POST",
                                url: controller.gadget.getActionPath({ action: "SignIn", selectedAccount: accountName }),
                                data: {},
                                dataType: "json",
                                success: function (html) {
                                    $gadgetElement.trigger("epi-ga-signin", accountName);
                                    controller.selectAccount("Personal");
                                }
                            });
                        }

                        var url = controller.gadget.getActionPath({ action: "SignIn" });
                        // Open popup window name=small to authenticate with Google Account
                        window.open(url, "small", "height=600,width=900,status=0,titlebar=0,toolbar=0");
                    });
                };

                controller.connectSignOut = function () {
                    /// <summary>Post to serverside, to clear the current authenticated Google Account</summary>

                    $(".signout", gadgetElement).click(function (e) {
                        e.preventDefault();

                        controller.gadget.ajax({
                            type: "POST",
                            url: controller.gadget.getActionPath({ action: "SignOut" }),
                            data: {},
                            dataType: "json",
                            success: function () {
                                var signedOutAccount = $gadgetElement.find(".account").val();
                                $gadgetElement.trigger("epi-ga-signout", signedOutAccount);
                                controller.gadget.loadView({ action: "ConfigureAccount" });
                            }
                        });
                    });
                };

                controller.connectAccountSelection = function (gadget) {
                    controller.connectSignIn("ConfigureAccount");
                    controller.connectSignOut("ConfigureAccount");

                    $(".account", gadgetElement).each(function () {
                        controller.onAccountsChanged();
                    }).change(function () {
                        controller.onAccountsChanged();
                        $(this).blur();
                        controller.selectAccount($(this).val());
                    });
                };

                controller.connectSegmentFilter = function () {
                    $(".SegmentFilter", gadgetElement).change(function () {
                        var customsegments = $(".custom-segment", gadgetElement),
                            advancedsegments = $(".advanced-segment", gadgetElement),
                            customSegmentText = $("[name='Statistics.CustomSegmentText']", gadgetElement);

                        customSegmentText.removeClass("required");
                        if (this.value === "custom") {
                            advancedsegments.filter(":visible").slideUp("fast");
                            customsegments.slideDown("fast");
                            customSegmentText.addClass("required");
                        } else if (this.value == "advanced") {
                            customsegments.filter(":visible").slideUp("fast");
                            advancedsegments.slideDown("fast");
                        } else {
                            customsegments.filter(":visible").slideUp("fast");
                            advancedsegments.filter(":visible").slideUp("fast");
                        }
                    });
                };

                function parseDate(value) {
                    /// <summary>Util function, convert datestring in "1970.1.1" to Javascript Date</summary>

                    var xpr = /(\d+)[.](\d+)[.](\d+)/,
                        components = xpr.exec(value),
                        date = new Date(components[1], parseInt(components[2]) - 1, components[3]);

                    return date;
                };

                controller.initDates = function () {
                    var queryDateFormat = 'yy.mm.dd';
                    $('.date-range-selector', gadgetElement).each(function () {
                        $(this).daterange({
                            viewDateFormat: '',
                            queryDateFormat: queryDateFormat,
                            queryStartDateKey: 'sd',
                            queryEndDateKey: 'ed',
                            minDate: parseDate($(this).attr("data-minDate")),
                            maxDate: parseDate($(this).attr("data-maxDate")),
                            startDate: parseDate($(this).attr("data-startDate")),
                            endDate: parseDate($(this).attr("data-endDate")),
                            popupAlign: 'left',
                            popupParent: 'body',
                            delimiter: '',
                            attachPopupElement: $('body'),
                            wrongRangeMessage: $(this).attr("data-wrongRangeMessage"),
                            onSelect: function (e) {
                                var gadget = epi.gadget.getByElement(this);
                                var fromDate = $.datepicker.formatDate(queryDateFormat, e.widget._startDatePicker.datepicker('getDate'));
                                var toDate = $.datepicker.formatDate(queryDateFormat, e.widget._endDatePicker.datepicker('getDate'));

                                controller.updateContextSettings({ startDate: fromDate, endDate: toDate }, controller.reload);
                            },
                            onShow: function (e) {
                                $(document).bind('click', closeDateRangeHandler);
                            },
                            onHide: function (e) {
                                $(document).unbind('click', closeDateRangeHandler);
                            },
                            selectText: $(this).attr("data-selectText"),
                            cancelText: $(this).attr("data-cancelText")
                        });
                        $(this).click(function (e) {
                            $(this).daterange("show");
                            e.preventDefault();
                        });
                    });
                    // Closes the date range picker if a click was made anywhere outside of the widget.
                    function closeDateRangeHandler(e) {
                        var targetElement = e.originalTarget || e.srcElement;

                        if ($(targetElement).closest('.date-range-selector,.ui-daterange-popup,.ui-datepicker-calendar,.ui-datepicker-header').length == 0) {
                            // click outside the date range
                            $gadgetElement.find('.date-range-selector').daterange("hide");
                        }
                    }

                    $gadgetElement.bind("graphdataloaded", function (e, data) {
                        if (data.IsAuthenticated) {
                            $(".date-range-description", gadgetElement).html(data.SelectedRange.Description);
                        }
                    });
                };

                controller.initGraphBy = function () {
                    $gadgetElement.bind("graphdataloaded", function (e, data) {
                        if (data.IsAuthenticated) {
                            var $graphBySelector = epi.googleAnalytics._getGraphBySelector(gadgetElement);

                            $graphBySelector.find(".graph-by-options-placeholder").text(
                                $graphBySelector.find(".option").slice(data.SelectedRange.GraphBy, data.SelectedRange.GraphBy + 1).text());
                        }
                    });

                    $(".graph-by a.option", gadgetElement).click(function () {
                        controller.updateContextSettings({ graphBy: $(this).attr("data-graphby-index") }, lang.hitch(controller, controller.loadChartData));
                    });

                    var opener = $(".graph-by-options-opener", gadgetElement);
                    var contextMenu = $(".graph-by-options", gadgetElement);
                    
                    // TECHNOTE: Work around to show the context menu correctly (This bug comes from CMS EPiContextMenu)
                    // If set "autohide" = true, when clicking on the attachedTo element multiple times
                    // the context menu will never display again unless you click outside the attachedTo element and
                    // then click to it again. Here we show/hide the context menu manually for correct behavior.
                    contextMenu.epiContextMenu({ attachedTo: opener, autohide: false });

                    var hideContextMenu = function (e) {
                        if (contextMenu.is(":visible") && ($(e.target).closest("div.graph-by-options-opener").length == 0)) {
                            contextMenu.epiContextMenu("hide");
                            epi.shell.events.unbindFrameClickHandler(hideContextMenu)
                        }
                    }

                    opener.click(function () {
                        if (!contextMenu.is(":visible")) {
                            contextMenu.epiContextMenu("show");
                            epi.shell.events.bindFrameClickHandler(hideContextMenu);
                        }
                    });
                };

                controller.updateContextSettings = function (input, callback) {
                    /// <summary>post changes (date, graphby, segment) to update the analytics settings</summary>
                    var url = controller.gadget.getActionPath(controller.applyDynamicSegment({ action: "UpdateSettings" }));
                    controller.gadget.ajax({
                        type: "POST",
                        url: url,
                        data: input || {},
                        dataType: "json",
                        success: function (result) {

                            if (typeof callback === "function") {
                                callback();
                            }
                        }
                    });
                };

                controller.loadPageSummary = function (input, callback) {
                    // nothing to do if is in Dashboard
                    if (epi.googleAnalytics.workingInDashboard) {
                        return;
                    }
                    var url = controller.gadget.getActionPath(controller.applyDynamicSegment({ action: "PageSummaryView" }));
                    controller.gadget.ajax({
                        type: "POST",
                        url: url,
                        data: input || {},
                        dataType: "html",
                        success: function (html) {
                            if (controller._validateAndShowError(html, gadgetElement)) {
                                return;
                            }

                            $(".page-summary", gadgetElement).html(html);

                            if (typeof callback === "function") {
                                callback();
                            }

                            $gadgetElement.trigger("fivedatareceived", { html: html });
                        }
                    });
                };

                controller.initSegments = function (input, callback) {
                    $(".segment-filter", gadgetElement).each(function () {
                        var $segmentContainer = $(this),
                            url = controller.gadget.getActionPath(controller.applyDynamicSegment({ action: "FilterBy" }));
                        $segmentContainer.removeData("segmentData");

                        controller.gadget.ajax({
                            type: "POST",
                            url: url,
                            data: input || {},
                            dataType: "html",
                            success: function (html) {
                                $segmentContainer.innerHtml = "";

                                var $segmentArea = $(html);
                                $segmentArea.appendTo($segmentContainer);

                                // the data is available after loading from server, keep it
                                $segmentContainer.data("segmentData", controller._getSegmentFilterData($segmentArea));

                                controller.connectSegmentFilter();

                                if (controller._segmentTitlePane && controller._segmentTitlePane.domNode) {
                                    controller._segmentTitlePane.set("content", $segmentArea);
                                }
                                controller._updateSegmentTitlePane();


                                $("form.epi-gadgetform", gadgetElement).validate({
                                    rules: {
                                        "Statistics.CustomSegmentText": {
                                            required: function () { return $(".custom-segment:visible", gadgetElement).length > 0; },
                                            minlength: 3
                                        }
                                    },
                                    submitHandler: function (form) {
                                        var segmentData = controller._getSegmentFilterData(gadgetElement);
                                        // keep the changed data
                                        $segmentContainer.data("segmentData", segmentData);

                                        controller.updateContextSettings({
                                            segment: segmentData.segment, segmentDimension: segmentData.segmentDimension,
                                            segmentOperator: segmentData.segmentOperator, segmentText: segmentData.segmentText
                                        }, controller.reload);

                                        controller._updateSegmentTitlePane();
                                        controller._collapseSegmentTitlePane();
                                        return false;
                                    }
                                });

                                // re-bind the old value and collapse
                                $("input.epi-button-child-item[type='button']", gadgetElement).attr('onclick', '').unbind('click').bind("click", function () {
                                    controller._rebindSegmentFilterData($segmentContainer.data("segmentData"));
                                    controller._collapseSegmentTitlePane();

                                    return false;
                                });

                                if (typeof callback === "function") {
                                    callback();
                                }

                                return false;
                            }
                        });
                    });
                };

                controller._initTitlePanes = function (/*DOM*/gadgetElement) {
                    // summary:
                    //      Setups the dijit/TitlePane widgets for the gadget
                    // gadgetElement: [DOM]
                    //      The current gadget element (DOM)
                    // tags:
                    //      private

                    // Segment filter area title pane
                    controller._segmentTitlePane = new dijitTitlePane({
                        title: res.shared.section.segmentfilter.title,
                        open: false // Collapsed by default
                    });

                    var $segmentContainer = $(".segment-filter", gadgetElement);
                    $segmentContainer.append(controller._segmentTitlePane.domNode);
                    controller._segmentTitlePane.startup();

                    // Render the acquisition segment only its data existing
                    if ($(".lists", gadgetElement).length > 0) {
                        // Acquisition title pane (3 tables)
                        controller._acquisitionTitlePane = new dijitTitlePane({
                            title: res.shared.section.tables.title,
                            open: false // Collapsed by default
                        });

                        var $listsContainer = $(".lists-container", gadgetElement);
                        $listsContainer.append(controller._acquisitionTitlePane.domNode);

                        controller._acquisitionTitlePane.startup();
                    }
                };

                controller._getSegmentFilterData = function (container) {
                    var segmentVal = $(".SegmentFilter", container).val(),
                        segmentDim = $("select[name='Statistics.CustomSegmentDimension']", container).val(),
                        segmentOpe = $("select[name='Statistics.CustomSegmentOperator']", container).val(),
                        segmentText = $("input[name='Statistics.CustomSegmentText']", container).val();

                    return { segment: segmentVal, segmentDimension: segmentDim, segmentOperator: segmentOpe, segmentText: segmentText };
                };

                controller._rebindSegmentFilterData = function (segmentData) {
                    if (!segmentData || (segmentData.segment != "custom" & segmentData.segment == $(".SegmentFilter", gadgetElement).val())) {
                        return;
                    }

                    segmentData.segmentText && $("input[name='Statistics.CustomSegmentText']", gadgetElement).val(segmentData.segmentText);
                    segmentData.segmentOperator && $("select[name='Statistics.CustomSegmentOperator']", gadgetElement).val(segmentData.segmentOperator);
                    segmentData.segmentDimension && $("select[name='Statistics.CustomSegmentDimension']", gadgetElement).val(segmentData.segmentDimension);
                    segmentData.segment && $(".SegmentFilter", gadgetElement).val(segmentData.segment).trigger("change");
                };

                controller._updateSegmentTitlePane = function () {
                    if (!controller._segmentTitlePane) {
                        return;
                    }
                    var $segmentFilter = $(".SegmentFilter option:selected", gadgetElement);
                    controller._segmentTitlePane.set("title",
                        res.shared.section.segmentfilter.title + $segmentFilter.html());
                };

                controller._collapseSegmentTitlePane = function () {
                    controller._segmentTitlePane && controller._segmentTitlePane.open
                        && controller._segmentTitlePane.toggle();
                };

                controller._validateAndShowError = function (html, container) {
                    // exception occurs at server side sending error info
                    var $errorMessage = $(".errorinfo", html);
                    if ($errorMessage && $errorMessage.length > 0) {
                        // set flag to refresh segments
                        // calling controller.initSegments here will cause a bad cycling
                        controller.segmentsOutDated = true;
                        epi.googleAnalytics._showFeedbackMessage($errorMessage.closest(".error-area").html(), container);
                        return true;
                    }
                    return false;
                };

                controller.loadLists = function (input, callback) {
                    $(".lists", gadgetElement).each(function () {
                        var listsWrapper = this,
                            url = controller.gadget.getActionPath(controller.applyDynamicSegment({ action: "ListsView" }));

                        controller.gadget.ajax({
                            type: "POST",
                            url: url,
                            data: input || {},
                            dataType: "html",
                            success: function (html) {
                                if (controller._validateAndShowError(html, gadgetElement)) {
                                    return;
                                }

                                var $listsWrapper = $(listsWrapper);
                                // We don't want the redundant spaces under the collapsible pane (acquisition pane).
                                // So that, be default, hide the content that will be places inside the acquisition pane.
                                $listsWrapper.css("display", "none");
                                $listsWrapper.html(html);

                                if (typeof callback === "function") {
                                    callback();
                                }

                                $gadgetElement.trigger("listhtmlreceived", { html: html });

                                // Place all the list's content inside a dijit TitlePane (called Metrics)
                                if (controller._acquisitionTitlePane && controller._acquisitionTitlePane.domNode) {
                                    var $containerNode = $(controller._acquisitionTitlePane.containerNode);
                                    $containerNode.empty();
                                    $containerNode.append($listsWrapper);

                                    if (controller._acquisitionTitlePane.open) {
                                        $listsWrapper.css("display", "");
                                    }

                                    // Toogle the display of the content inside the acquisition pane based on
                                    // the toggle state of the acquisition pane
                                    aspect.after(controller._acquisitionTitlePane, "toggle", function () {
                                        $listsWrapper.css("display", this.open ? "" : "none");
                                    });
                                }
                            }
                        });
                    });
                };

                controller._isNotSupportContentType = function (dataType) {
                    if (typeof epi.googleAnalytics.notSupportContentTypes !== 'undefined'
                            && epi.googleAnalytics.notSupportContentTypes.length > 0) {
                        var isMatch = false;
                        $.each(epi.googleAnalytics.notSupportContentTypes, function (index, value) {
                            isMatch = TypeDescriptorManager.isBaseTypeIdentifier(dataType, value);
                            if (isMatch) {
                                return false;
                            }
                        });

                        return isMatch;
                    }

                    return false;
                };

                controller.updateHeading = function (pageName) {
                    var heading = $("h2.gadget-heading", gadgetElement),
                        text = heading.attr("data-text-template");

                    heading.text(text + ' "' + pageName + '"');

                    if (pageName) {
                        heading.show();
                    } else {
                        heading.hide();
                    }
                };

                controller.reload = function () {
                    // If no google analytics profile, just show feedback message and do nothing
                    if (epi.googleAnalytics._setFeedbackMessage(gadgetElement)) {
                        return;
                    }

                    epi.googleAnalytics._showGadgetContent(gadgetElement);

                    controller.segmentsOutDated && controller.initSegments();
                    controller.segmentsOutDated = false;

                    if (epi.googleAnalytics.workingInDashboard) {
                        controller.loadSummary();
                    }
                    else {
                        // re-bind the segment filter with stored segmentData
                        controller._rebindSegmentFilterData($(".segment-filter", gadgetElement).data("segmentData"));
                        controller.loadPageSummary();
                    }
                    controller.loadChartData();
                    controller.loadLists();
                };
            };   // end declare epi.googleAnalytics.controller "class"

            epi.googleAnalytics._setFeedbackMessage = function (/*DOM*/gadgetElement) {
                // summary:
                //      Verifies and then show the gadget's feedback if applicable
                //      If no google analytics profile, just show feedback message and do nothing
                // gadgetElement: [DOM]
                //      The given gadget element that used as a limited context to find the correct indicated element
                // tags:
                //      private

                if (!epi.googleAnalytics.isAuthenticated) {
                    return false;
                }

                var currentContext = contextService.currentContext,
                    $gadgetContent = $(".epi-gadgetContent", gadgetElement),
                    $gadgetFeedback = $(".epi-gadgetFeedback", gadgetElement);

                var isShowFeedbackMsg = false;

                // Support only for content having PublicURL and 
                //   has BaseTypeIdentifier equal "episerver.core.pagedata"
                // Examples:
                //    - The content type does not has Public Url as like: Block Type, Root Page, Container Page Type
                //    - The content type has public Url but base type identifier is not "episerver.core.pagedata": Media, Folder
                if (!epi.googleAnalytics.workingInDashboard && (!currentContext.publicUrl || controller._isNotSupportContentType(currentContext.dataType))) {
                    epi.googleAnalytics._showFeedbackMessage(res.context.invalidcontenttype, gadgetElement);
                    isShowFeedbackMsg = true;
                }
                else if (!epi.googleAnalytics.selectedProfile) {
                    epi.googleAnalytics._showFeedbackMessage(res.shared.message.novalidgaaccount, gadgetElement);
                    isShowFeedbackMsg = true;
                }

                !isShowFeedbackMsg && epi.googleAnalytics._showGadgetContent(gadgetElement);
                //!isShowFeedbackMsg && $gadgetContent.removeClass("hidden") && $gadgetFeedback.empty().hide();

                return isShowFeedbackMsg;
            };

            epi.googleAnalytics._showFeedbackMessage = function (/*String*/message, gadgetElement) {
                // summary:
                //      Setups feedback message for the gadget
                // message: [String]
                //      The message be show in the gadged's feedback message area
                // tags:
                //      private

                $gadgetContent = $(".epi-gadgetContent", gadgetElement),
                $gadgetFeedback = $(".epi-gadgetFeedback", gadgetElement);

                // DO NOT "hide" the $gadgetContent because it may cause dijit elements hidden recursively,
                // then we cannot select element:visible anymore.
                $gadgetContent.addClass("hidden");
                $gadgetFeedback.html(message);
                $gadgetFeedback.css({
                    "display": "block",
                    "padding-left": "8px",
                    "float": "none",
                    "overflow": "visible"
                });
            };

            epi.googleAnalytics._showGadgetContent = function (gadgetElement) {
                // summary:
                //      Clear all gadget's feedback message and then show the gadget's content area
                // tags:
                //      private

                var $gadgetContent = $(".epi-gadgetContent", gadgetElement),
                $gadgetFeedback = $(".epi-gadgetFeedback", gadgetElement);

                $gadgetFeedback.empty().hide();
                $gadgetFeedback.removeAttr("style");
                $gadgetContent.removeClass("hidden");
            };

            // instantiate an object of controller, and give back to the callback()
            var controller = new epi.googleAnalytics.controller(gadget);
            callback(controller);
        });   // end dojo scope()

        epi.googleAnalytics._resizeChart = function (gadgetElement, /*Object*/chart) {
            // summary:
            //      Resize the dojox Chart
            // gadgetElement: [DOM]
            //      The current gadget element (DOM)
            // chart: [Object]
            //      An instance of the dojox/charting/Chart2D
            // tags:
            //      private

            var chartNode = chart.node;
            if (!chartNode) {
                return;
            }

            var $chartNodeParent = $(chartNode).parent();
            if (!$chartNodeParent || $chartNodeParent.length === 0) {
                return;
            }

            function resizeClipPath(/* object */chart, /*object*/adjustProperties) {
                if (!chart || !adjustProperties) {
                    console.log("resizeClipPath: Invalid input parameters");
                    return;
                }

                // NOTE: We must select clippath element by id instead of tag name because of a known bug for webkit browsers.
                // Bug 83438 - querySelectorAll unable to find SVG camelCase elements imported into HTML:
                // https://bugs.webkit.org/show_bug.cgi?id=83438
                var shape = $('[id^="gfx_clip"]', chart.node).find('rect')[0];

                $.each(adjustProperties, function (key, value) {
                    shape.setAttribute(key, parseFloat(shape.getAttribute(key)) + value);
                });
            }

            var newWidth = $chartNodeParent.width(),
                newHeight = $chartNodeParent.height() - (epi.googleAnalytics._getGraphBySelector(gadgetElement).outerHeight(true) || 0);
            if (newWidth <= 0 || newHeight <= 0) {
                return;
            }

            chart.resize(newWidth, newHeight);

            // NOTE: Work around to display the marker correctly for max value points.
            // Bug #16802: Markers for first and last data point are cut in half:
            // https://bugs.dojotoolkit.org/ticket/16802
            resizeClipPath(chart, { "height": 10, "y": -10 });
        };

        epi.googleAnalytics._getGraphBySelector = function (/*DOM*/gadgetElement) {
            // summary:
            //      Gets the graph-by selector
            // gadgetElement: [DOM]
            //      The given gadget element that used as a limited context to find the correct indicated element
            // tags:
            //      private

            return $(".graph-by", gadgetElement);
        };

        epi.googleAnalytics._getGadgetContainerWidget = function (/*DOM*/gadgetElement) {
            // summary:
            //      Gets the closest dijit gadget container object
            // gadgetElement: [DOM]
            //      The given gadget element that used as a limited context to find the correct indicated element
            // tags:
            //      private

            return dijit.byNode($(gadgetElement).closest(".epi-gadgetContainer")[0]);
        };

        epi.googleAnalytics._getNewHeight = function (/*DOM*/gadgetElement) {
            // summary:
            //      Gets the gadget content height
            // gadgetElement: [DOM]
            //      The given gadget element that used as a limited context to find the correct indicated element
            // tags:
            //      private

            return Math.max(120 + $(".epi-gadgetContent", gadgetElement).outerHeight(true), 360);
        };
    };   // end epi.googleAnalytics.defineController()

})(epiJQuery);