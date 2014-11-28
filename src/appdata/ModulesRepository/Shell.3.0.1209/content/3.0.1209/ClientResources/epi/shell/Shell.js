(function($, epi) {

    // If epi.shell already has sub objects we have to preserve them
    // and add them once epi.shell core functionality been defined.
    var preservedEpiShell = null;
    if (!epi.shell) {
        preservedEpiShell = epi.shell;
    }

    epi.shell = function() {
        /// <summary>
        ///     Base namespace for common functionality in EPiServer.
        ///     Note: Some functionality needs to be initialized with init() when document is ready.
        /// </summary>
        var pub = {}; // object returned by function. Exposes public methods and variables.

        pub.init = function() {
            /// <summary>Initialize Base Functionality</summary>
            epi.shell.registerInitFunction(DOMReadyInit);
            $(document).ready(function() { epi.shell.runInitFunctions(true); });

            //Attach to the global ajaxComplete event so we can handle authorization
            $(document.body).ajaxComplete(function(event, request, settings) {
                //If it is the EPiServer login screen, redirect to the top adress
                var isLoginScreen;
                if (request) {
                    try {
                        isLoginScreen = request.getResponseHeader("X-EPiLogOnScreen");
                    }
                    catch (ex) {
                    }
                }
                if (isLoginScreen) {
                    window.location.reload();
                }
            });
        };

        var DOMReadyInit = function() {
            // Hack to prevent shift+F10 from opening default menu...
            $("body").bind("keydown", function(e) { if (e.shiftKey && e.which === epi.shell.keyCode.f10) { $(e.target).trigger("contextmenu"); e.stopPropagation(); e.preventDefault(); } });
        };

        pub.setLoadingCursor = function() {
            /// <summary>
            ///     Shows that the server is working by changing the cursor to waiting.
            /// </summary>
            $("body").css("cursor", "wait");
        };

        pub.resetCursor = function() {
            /// <summary>
            ///     Shows that the server is done working by changing the cursor to default.
            /// </summary>
            $("body").css("cursor", "default");
        };

        //Array with all the init functions
        var _initFunctions = [];

        //Index that points on what inits have been run in the array.
        var _initFunctionsRunIndex = 0;

        pub.registerInitFunction = function(func) {
            ///<summary>
            ///     Adds a function to the initfunction array.
            ///</summary>
            /// <param name="func" type="function">
            ///     function to add to the list
            /// </param>
            _initFunctions[_initFunctions.length] = func;
        };

        pub.runInitFunctions = function(runAllInitFunctions) {
            ///<summary>
            ///     Runs the init functions that is in the initFunctions array. if true is passed as parameter all functions will be run
            ///     else only newly added functions will be run
            ///</summary>
            /// <param name="runAllInitFunctions" type="Boolean">
            ///     Boolean that decides if we are to run all init methods or only the ones added after the last run.
            /// </param>
            if (runAllInitFunctions) {
                $.each(_initFunctions, function (index, func) {
                    func();
                });
            } else {
                var index;
                var funcLength = _initFunctions.length;
                for (index = _initFunctionsRunIndex; index < funcLength; index++) {
                    _initFunctions[index]();
                }
            }
            _initFunctionsRunIndex = _initFunctions.length;
        };

        var _ajaxQueue = [];

        var _isAjaxQueueRunning = false;

        pub.ajax = function(ajaxObject) {
            $(document).trigger("epiajaxrequestrunning", { ajaxObject: ajaxObject });
            $.ajax({
                url: ajaxObject.url,
                type: ajaxObject.type,
                dataType: ajaxObject.dataType,
                cache: false,
                data: ajaxObject.data,
                error: function(xmlHttpRequest, status, errorThrown) {
                    if ($.isFunction(ajaxObject.error)) {
                        ajaxObject.error(xmlHttpRequest, status, errorThrown); //Run any errorhandler
                    }
                    else {
                        //Trigger an error event
                        $(document).trigger("epiajaxrequestfailed", { xmlHttpRequest: xmlHttpRequest, status: status, errorThrown: errorThrown });
                    }
                    epi.shell.resetCursor();
                },
                success: function(data) {
                    if ($.isFunction(ajaxObject.success)) {
                        ajaxObject.success(data); // Run the success handler
                    }
                    epi.shell.resetCursor();
                }
            });
        };

        pub.addToAjaxQueue = function(ajaxObject, eventData) {
            ///<summary>
            ///     Adds a new ajax request to the ajax queue and if no current requests are running it
            ///     starts by running the first request in the queue.
            ///</summary>
            /// <param name="ajaxObject" type="Object">
            ///     The options object for the ajax request
            /// </param>
            _ajaxQueue.push({ ajaxObject: ajaxObject, eventData: eventData });
            _runAjaxRequestsInQueue();
        };

        var _runAjaxRequestsInQueue = function() {
            ///<summary>
            ///     Runs a ajax request for the first ajaxrequestobject in the queue.
            ///</summary>
            if (!_isAjaxQueueRunning) {
                _isAjaxQueueRunning = true;
                epi.shell.setLoadingCursor();

                var current = _ajaxQueue[0];
                $(document).trigger("epiajaxrequestrunning", current);
                $.ajax({
                    url: current.ajaxObject.url,
                    type: current.ajaxObject.type,
                    dataType: current.ajaxObject.dataType,
                    cache: false,
                    data: current.ajaxObject.data,
                    error: function(xmlHttpRequest, status, errorThrown) {
                        if ($.isFunction(current.ajaxObject.error)) {
                            current.ajaxObject.error(xmlHttpRequest, status, errorThrown); //Run any errorhandler
                        }
                        _ajaxQueue.splice(0); //Clear the rest of the queue
                        epi.shell.resetCursor();

                        _isAjaxQueueRunning = false;
                    },
                    success: function(data) {
                        current.ajaxObject.success(data); // Run the success handler
                        _ajaxQueue.splice(0, 1); //Remove the ajaxrequest in first queue position
                        if (_ajaxQueue.length > 0) { //if we have more ajax requests in the queue, run next request.
                            _isAjaxQueueRunning = false;
                            _runAjaxRequestsInQueue();
                        }
                        epi.shell.resetCursor();
                        _isAjaxQueueRunning = false;
                    }
                });
            }
        };

        pub.keyCode = {
            tab: 9,
            enter: 13,
            shift: 16,
            ctrl: 17,
            alt: 18,
            escape: 27,
            space: 32,
            pageUp: 33,
            pageDown: 34,
            arrowLeft: 37,
            arrowUp: 38,
            arrowRight: 39,
            arrowDown: 40,
            del: 46,
            s: 83,
            contextMenu: 93,
            f10: 121
        };

        pub.isArrowKey = function(keyCode) {
            /// <summary>
            ///     Return true if supplied keyCode is one of the arrowKeys
            /// </summary>
            /// <param name="keyCode" type="Number">
            ///     keyCode (normal use e.which)
            /// </param>
            /// <returns type="Boolean" />
            var returnValue = false;
            switch (keyCode) {
                case pub.keyCode.arrowDown:
                case pub.keyCode.arrowLeft:
                case pub.keyCode.arrowRight:
                case pub.keyCode.arrowUp:
                    returnValue = true;
                    break;
            }
            return returnValue;
        };

        pub.serializeQueryStringAsArray = function(queryString) {
            /// <summary>
            ///     Splits a querysting and returns it as a name value collection
            /// </summary>
            /// <param name="queryString">
            ///     The query string to split
            /// </param>
            var queryStringParameters = [];
            //Remove first ?
            queryString = decodeURIComponent(queryString.substring(1));

            //split on &
            var keyValues = queryString.split("&");

            for (var i in keyValues) {
                var key = keyValues[i].split("=");
                queryStringParameters[key[0].toLowerCase()] = key[1];
            }
            return queryStringParameters;
        };

        pub.getSimilarElementInCertainPosition = function(element, index, className) {
            /// <summary>
            ///     Returns a visible element with same tag name and css class(es) which in
            ///     document structure is in [index] position in relation to supplied element.
            ///     index = -1 will return an element with above characteristics right before supplied element.
            /// </summary>
            /// <param name="element" type="jQuery or DOM element">
            ///     Element
            /// </param>
            /// <param name="index" type="Number">
            ///     Positive or negative number indicating how many steps from current element returned element is.
            /// </param>
            /// <param name="class" type="String">
            ///     Will override default use of supplied element class name[s]. Optional.
            /// </param>
            /// <returns type="jQuery" />

            if (!index) {
                index = 0; // Will cause supplied element to be returned!
            }

            if (element.get !== undefined) {
                element = element.get(0);
            }

            if (typeof className === "undefined" || className === "") {
                className = element.className;
            }

            var elements = $(element.tagName + "." + className + ":visible");
            var i;
            var length = elements.length;

            for (i = 0; i < length; i++) {
                if (elements[i] === element) {
                    if (i + index < length && i + index >= 0) {
                        return $(elements[i + index]);
                    }
                }
            }

            return null;
        };

        var idPrefix = "epi";
        var idCount = -1;
        pub.generateId = function(prefix) {
            /// <summary>
            ///     Generates a unique id with a specified prefix
            /// </summary>
            /// <param name="prefix" type="String">
            ///     The prefix to set on the unique id
            /// </param>
            idCount++;
            if (arguments.length === 0) {
                prefix = idPrefix;
            }
            while ($("#" + prefix + idCount).length !== 0) {
                idCount++;
            }
            return prefix + idCount;
        };

        pub.events = function() {
            var pub = {};

            var eventHandlers = [];

            pub.bindFrameClickHandler = function(functionToExecute) {
                eventHandlers.push(functionToExecute);

                if (eventHandlers.length == 1) {
                    // Since this seems to be the first event handler we need to bind the global event handler
                    _updateClickHandler(window, true);
                }
            };

            pub.unbindFrameClickHandler = function(functionToRemove) {
                // Find the matching event handler and remove it from the queue
                var n = eventHandlers.length;
                while (n--) {
                    if (eventHandlers[n] === functionToRemove) {
                        eventHandlers.splice(n, 1);
                        break;
                    }
                }

                if (eventHandlers.length === 0) {
                    // If there is no more event handlers in the queue, remove the global event handler
                    _updateClickHandler(window, false);
                }

            };

            var _updateClickHandler = function(frame, addHandler) {
                /// Recursively iterates frames and removes events to each frame.
                for (var i = 0; i < frame.frames.length; i++) {
                    try {
                        var subFrame = frame.frames[i];
                        _updateClickHandler(subFrame, addHandler);
                    } catch (ex) {
                    }
                }

                try {
                    if (addHandler) {
                        $(frame.document).bind("click", _sendToHandlers);
                        $(frame.document).bind("contextmenu", _sendToHandlers);  //Right click
                    }
                    else {
                        $(frame.document).unbind("click", _sendToHandlers);
                        $(frame.document).unbind("contextmenu", _sendToHandlers); //Right click
                    }
                } catch (ex) {
                }
            };

            var _sendToHandlers = function(e) {
                for (var i = 0; i < eventHandlers.length; i++) {
                    eventHandlers[i].call(this, e);
                }
            };

            return pub;
        } ();

        return pub;
    } ();

    // Merge all epi.shell functionality defined prior the core definition above.
    if (preservedEpiShell != null) {
        $.extend(epi.shell, preservedEpiShell);
    }

    epi.shell.init();



    epi.shell.layout = function() {
        var opt = {
            area: "#epi-applicationBody",
            globalDocument: "#epi-globalDocument",
            content: null
        };
        var priv = {
            initialized: false,
            scrollbarWidth: null,
            getScrollbarWidth: function() {
                /// <summary>
                ///     Returns the width of the browser scrollbar.
                /// </summary>
                if (!priv.scrollbarWidth) {
                    var div = $('<div style="width:50px;height:50px;overflow:hidden;position:absolute;top:-200px;left:-200px;"><div style="height:100px;"></div>');
                    // Append our div, do our calculation and then remove it
                    $('body').append(div);
                    var w1 = $('div', div).innerWidth();
                    div.css('overflow-y', 'scroll');
                    var w2 = $('div', div).innerWidth();
                    div.remove();
                    priv.scrollbarWidth = w1 - w2;
                }
                return priv.scrollbarWidth;
            },
            changeHandler: function(e) {
                var scrollarea = $(opt.area);

                //Hide the area
                scrollarea.hide();

                //Calculate the new height, if it is below 0 set it to 0
                var newHeight = $(document.documentElement).height() - $(opt.globalDocument).height() - 2;
                if (newHeight < 0) {
                    newHeight = 0;
                }
                //Set the new height and show it
                scrollarea.height(newHeight).show();

                if (opt.content) {
                    var panel = $(opt.content);
                    panel.width(scrollarea.width() - priv.getScrollbarWidth());
                }
            }
        };
        var pub = {
            initScrollableArea: function(options) {
                /// <summary>
                ///     Sets up a scrollable area and removes scrolling from the document
                /// </summary>
                /// <param name="options" type="Object">
                ///     area: the scrollable area
                ///     globalDocument: is used to calculate the non-scrollable size
                ///     content: scrollable content whose width is adjusted
                /// </param>
                $.extend(opt, options);
                if (!priv.initialized) {
                    priv.initialized = true;
                    $(document.documentElement).addClass("epi-scrollableArea");
                    $(document).bind("layoutchange", priv.changeHandler);
                    $(window).resize(pub.reportChanges);
                }
                $(opt.area).addClass("epi-scrollable").css("overflow", "auto");
                pub.reportChanges();
            },
            reportChanges: function(e) {
                /// <summary>
                ///     Triggers the layoutchange event which causes the layout to be reloaded
                /// </summary>
                $(document).trigger("layoutchange");
            }
        };
        return pub;
    } ();

    epi.shell.resource = function() {
        var pub = {};
        var _resources = {};

        pub.add = function(resourceSetKey, resourceSet) {
            /// <summary>Adds a resource set to the resource manager</summary>
            /// <param name="resourceSetKey" type="String">Resource set name. E.g. EPiServer.Shell.Resources.Texts</param>
            /// <param name="resourceSet" type="Object">The resources JSON object. E.g. {MyTitle:'Title', MyText:'Text'}</param>
            _resources[resourceSetKey] = resourceSet;
        };

        pub.get = function(resourceSetKey, resourceKey) {
            /// <summary>Gets a resource value from the resource manager</summary>
            /// <param name="resourceSetKey" type="String">The resource set to look in. E.g. EPiServer.Shell.Resources.Texts</param>
            /// <param name="resourceKey" type="Object">The resources key to look for. E.g. 'MyTitle'</param>
            ///<returns type="String">The resource value if found, otherwise an error text</returns>
            var resourceValue = "The [" + resourceKey + "] key does not exist in [" + resourceSetKey + "].";
            if (_resources[resourceSetKey] && _resources[resourceSetKey][resourceKey]) {
                resourceValue = _resources[resourceSetKey][resourceKey];
            }
            return resourceValue;
        };

        return pub;
    } ();
})(epiJQuery, epi);
