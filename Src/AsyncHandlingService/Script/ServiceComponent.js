(function ($) {
    var KEY = 'FD153B31-FE65-4665-A6B3-E9079D5DBCAB';

    function main() {
        var self = this;

        var url = self.url;
        var type = self.type;

        if ($ == null) throw new Error("Invoking general service need load jQuery library.");

        var gs = null;

        if (typeof type == 'string') {
            type = $.trim(type);
        }

        function IntervalCleaner() {
            this.timerHandle = null;
            this.intervalHandle = null;
        }

        IntervalCleaner.prototype = {
            clear: function () {
                if (this.timerHandle) {
                    clearTimeout(this.timerHandle);
                    this.timerHandle = null;
                }

                if (this.intervalHandle) {
                    clearInterval(this.intervalHandle);
                    this.intervalHandle = null;
                }
            }
        };

        function ServiceCallbacks() {
            this.refresh();
        }

        ServiceCallbacks.prototype = {
            all: ['prepare', 'start', 'success', 'error', 'end'],

            refresh: function () {
                var options = ServiceOptions.current();
                var context = options.context;
                var isContextChanged = this.isChanged('context', context);

                for (var i = 0; i < this.all.length; i++) {
                    var name = this.all[i];
                    f = options[name] || $.noop;

                    if (!$.isFunction(f)) {
                        continue;
                    }

                    if (this.isChanged(name, f) || isContextChanged) {
                        this.backup(name, f);
                        this[name] = (context != null) ? $.proxy(f, context) : f;
                    }
                }

                this.backup('context', context);
            },

            isChanged: function (name, f) {
                return (this.backup(name) != f);
            },

            backup: function (name, f) {
                var innerName = '_$' + name;

                if (arguments.length == 2) {
                    this[innerName] = f;
                    return f;
                } else {
                    return this[innerName];
                }
            },

            getAjaxStart: function () {
                var options = ServiceOptions.current();
                var parameter = options.parameter;
                var data = options.data;
                var callbacks = this;

                return function (xhr, ajaxSettings) {
                    Sys.Debug.trace(String.format("[Asynchronous Service - {0}]{1}Invoking start...", type, callbacks.getInvokingServiceMethod()));
                    Sys.Debug.traceDump(options);
                    Sys.Debug.trace("Prepare to invoke start callback, if you return false, the execution will be cancelled; arguments are:");
                    Sys.Debug.traceDump({
                        'arg1 (jQuery AJAX settings object)': ajaxSettings,
                        'arg2 (options data)': options.data,
                        'arg3 (service object)': 'gs'
                    });

                    var start = callbacks.start(ajaxSettings, options.data, gs);

                    if (start === false) {
                        Sys.Debug.trace(String.format("[Asynchronous Service - {0}]{1}Invoking cancelled.", type, callbacks.getInvokingServiceMethod()));
                        Sys.Debug.trace("Prepare to invoke end callback, arguments are:");
                        Sys.Debug.traceDump({
                            'arg1 (boolean, indicates whether cancelled by start callback)': true,
                            'arg2 (options data)': options.data,
                            'arg3 (service object)': 'gs'
                        });

                        callbacks.end(true, options.data, gs);
                        ServiceOptions.counter.next();
                    }

                    return start;
                }
            },

            getAjaxSuccess: function () {
                var options = ServiceOptions.current();
                var parameter = options.parameter;
                var data = options.data;
                var callbacks = this;

                return function (responseText, status, xhr) {
                    Sys.Debug.trace(
                            String.format(
                                    "[Asynchronous Service - {0}]{1}Invoke success, response text: {2}",
                                    type,
                                    callbacks.getInvokingServiceMethod(),
                                    responseText
                                )
                        );

                    var result = Sys.Serialization.JavaScriptSerializer.deserialize(responseText);

                    Sys.Debug.trace(String.format("[Asynchronous Service - {0}]{1}Invoke result:", type, callbacks.getInvokingServiceMethod()));
                    Sys.Debug.traceDump(result);

                    if (result.Success) {
                        var returnValue = result.ReturnValue;
                        Sys.Debug.trace("Prepare to invoke success callback, arguments are:");
                        Sys.Debug.traceDump({
                            'arg1 (return value)': returnValue,
                            'arg2 (options data)': options.data,
                            'arg3 (service object)': 'gs'
                        });

                        callbacks.success(returnValue, options.data, gs);
                    } else {
                        var error = result.Error;

                        error.type = 'logic';
                        error.message = error.Message;

                        Sys.Debug.trace(String.format("[Asynchronous Service - {0}]{1}Invoking error, status: {2}", type, callbacks.getInvokingServiceMethod(), status));
                        Sys.Debug.traceDump(error);
                        Sys.Debug.trace("Prepare to invoke error callback, arguments are:");
                        Sys.Debug.traceDump({
                            'arg1 (error object)': error,
                            'arg2 (options data)': options.data,
                            'arg3 (service object)': 'gs'
                        });

                        callbacks.error(error, options.data, gs);
                    }
                }
            },

            getAjaxError: function () {
                var options = ServiceOptions.current();
                var parameter = options.parameter;
                var data = options.data;
                var callbacks = this;

                return function (xhr, status, errorThrown) {
                    var error = {
                        type: 'system',
                        message: errorThrown,
                        status: status
                    };

                    Sys.Debug.trace(String.format("[Asynchronous Service - {0}]{1}Invoking error, status: {2}, error thrown: {3}", type, callbacks.getAjaxComplete(), status, errorThrown));
                    Sys.Debug.traceDump(error);
                    Sys.Debug.trace("Prepare to invoke error callback, arguments are:");
                    Sys.Debug.traceDump({
                        'arg1 (error object)': error,
                        'arg2 (options data)': options.data,
                        'arg3 (service object)': 'gs'
                    });

                    callbacks.error(error, options.data, gs);
                }
            },

            getAjaxComplete: function () {
                var options = ServiceOptions.current();
                var callbacks = this;

                return function (xhr, status) {
                    Sys.Debug.trace(String.format("[Asynchronous Service - {0}]{1}Invoke end: {2}", type, callbacks.getInvokingServiceMethod(), status));
                    try {
                        Sys.Debug.trace("Prepare to invoke end callback, arguments are:");
                        Sys.Debug.traceDump({
                            'arg1 (boolean, indicates whether cancelled by start callback)': false,
                            'arg2 (options data)': options.data,
                            'arg3 (service object)': 'gs'
                        });

                        callbacks.end(false, options.data, gs);
                    } finally {
                        ServiceOptions.counter.next();
                    }
                }
            },

            getInvokingServiceMethod: function () {
                return (gs.method != null) ? String.format('.[{0}]', gs.method) : '';
            }
        };

        function ServiceOptions() {
            this.parameter = null;
            this.data = null;
            this.context = null;
            this.asyc = true;
            this.delay = 0;
            this.interval = 0;
            this.method = 'POST';
            this.timeout = null;
            this.prepare = $.noop;
            this.start = $.noop;
            this.success = $.noop;
            this.error = $.noop;
            this.end = $.noop;
        }

        (function () {
            var options = new ServiceOptions();
            var callbacks = null;
            var cleaner = new IntervalCleaner();

            ServiceOptions.current = function () { return options; };
            ServiceOptions.callbacks = function () {
                if (callbacks == null) {
                    callbacks = new ServiceCallbacks();
                } else {
                    callbacks.refresh();
                }

                return callbacks;
            }
            ServiceOptions.cleaner = function () {
                if (cleaner == null) {
                    cleaner = new IntervalCleaner();
                }

                return cleaner;
            }
        })();

        ServiceOptions.ajax = function () {
            var options = ServiceOptions.current();
            var callbacks = ServiceOptions.callbacks();

            Sys.Debug.trace(
                    String.format(
                            "[Asynchronous Service - {0}]{1}Prepare to invoke prepare callback; you can set options at this step; arguments are:",
                            type,
                            callbacks.getInvokingServiceMethod()
                        )
                );
            Sys.Debug.traceDump({
                'arg1 (options)': options,
                'arg2 (options data)': options.data,
                'arg3 (service object)': 'gs'
            });

            callbacks.prepare(options, options.data, gs);
            callbacks.refresh();
            ServiceOptions.validate();

            var ajaxSettings = {
                url: url,
                type: options.method,
                async: options.async,
                timeout: options.timeout,
                cache: false,
                dataType: 'text',
                data: ServiceOptions.getRequestData(),
                responseType: 'text',
                beforeSend: callbacks.getAjaxStart(),
                success: callbacks.getAjaxSuccess(),
                error: callbacks.getAjaxError(),
                complete: callbacks.getAjaxComplete()
            };

            $.ajax(ajaxSettings);
        }

        ServiceOptions.init = function (PARAMETER, SUCCESS, ERROR, DATA, CONTEXT) {
            options = ServiceOptions.current();

            var parameter;
            var success;
            var error;
            var data;
            var context;

            var inputSettings = null;

            switch (arguments.length) {
                case 0:
                    break;

                case 1:
                    if ($.isFunction(PARAMETER)) {
                        success = PARAMETER;  // (success)
                    } else if (ServiceOptions.isOptions(PARAMETER)) {
                        inputSettings = PARAMETER; // (options)
                    } else {
                        parameter = PARAMETER;  // (parameter)
                    }

                    break;

                case 2:
                    if ($.isFunction(PARAMETER)) {
                        success = PARAMETER;

                        if ($.isFunction(SUCCESS)) {
                            error = SUCCESS;  // (success, error)
                        } else {
                            data = SUCCESS;  // (success, data)
                        }
                    } else {
                        parameter = PARAMETER;
                        success = SUCCESS;  // (parameter, success)
                    }
                    break;

                case 3:
                    if (!$.isFunction(PARAMETER)) {
                        parameter = PARAMETER;
                        success = SUCCESS;

                        if ($.isFunction(ERROR)) {
                            error = ERROR;  // (parameter, success, error)
                        } else {
                            data = ERROR;  // (parameter, success, data)
                        }
                    } else {
                        success = PARAMETER;

                        if ($.isFunction(SUCCESS)) {
                            error = SUCCESS;
                            data = ERROR;  // (success, error, data)
                        } else {
                            data = SUCCESS;
                            context = ERROR;  // (success, data, context)
                        }
                    }
                    break;

                case 4:
                    if (!$.isFunction(PARAMETER)) {
                        parameter = PARAMETER;
                        success = SUCCESS;

                        if ($.isFunction(ERROR)) {
                            error = ERROR;
                            data = DATA;  // (parameter, success, error, data)
                        } else {
                            data = ERROR;
                            context = DATA;  // (parameter, success, data, context)
                        }
                    } else {
                        success = PARAMETER;
                        error = SUCCESS;
                        data = ERROR;
                        context = DATA; // (success, error, data, context)
                    }
                    break;

                case 5:
                    parameter = PARAMETER;
                    success = SUCCESS;
                    error = ERROR;
                    data = DATA;
                    context = CONTEXT;  // (parameter, success, error, data, context)
                    break;

                default:
                    throw new Error("Parameter mismatch.");
            }

            if (parameter !== undefined) options.parameter = parameter;
            if (success !== undefined) options.success = success || $.noop;
            if (error !== undefined) options.error = error || $.noop;
            if (data !== undefined) options.data = data;
            if (context !== undefined) options.context = context;

            if (inputSettings != null) {
                $.extend(true, options, inputSettings);
            }

            return options;
        };

        ServiceOptions.isOptions = function (inputArg) {
            if (inputArg == null) {
                return false;
            }

            var primitiveTypes = ['number', 'string', 'boolean'];

            if ($.inArray(typeof inputArg, primitiveTypes) != -1) {
                return false;
            }

            if (inputArg instanceof Date) {
                return false;
            }

            if ($.isArray(inputArg)) {
                return false;
            }

            var options = new ServiceOptions();

            for (var p in options) {
                if (p in inputArg) {
                    return true;
                }
            }

            return false;
        };

        ServiceOptions.validate = function () {
            var isValid = ServiceOptions.isValid();

            if (!isValid) {
                var msg = String.format(
                    "The general service exec method signature should be one of the following:\r\n{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}\r\n{5}\r\n{6}\r\n{7}\r\n{8}\r\n{9}\r\n{10}\r\n{11}\r\n{12}\r\n{13}",
                    "cipace.gs.exec()",
                    "cipace.gs.exec(options)",
                    "cipace.gs.exec(success)",
                    "cipace.gs.exec(parameter, success)",
                    "cipace.gs.exec(success, error)",
                    "cipace.gs.exec(success, data)",
                    "cipace.gs.exec(parameter, success, error)",
                    "cipace.gs.exec(parameter, success, data)",
                    "cipace.gs.exec(success, error, data)",
                    "cipace.gs.exec(success, data, context)",
                    "cipace.gs.exec(parameter, success, error, data)",
                    "cipace.gs.exec(parameter, success, data, context)",
                    "cipace.gs.exec(success, error, data, context)",
                    "cipace.gs.exec(parameter, success, error, data, context)"
                );

                throw new Error(msg);
            }
        };

        ServiceOptions.isValid = function () {
            var callbacks = ServiceOptions.callbacks();

            for (var i = 0; i < callbacks.all.length; i++) {
                var name = callbacks.all[i];
                var f = options[name];

                if ((f != null) && !$.isFunction(f)) {
                    return false;
                }
            }

            return true;
        };

        ServiceOptions.getRequestData = function () {
            var options = ServiceOptions.current();
            var parameter = options.parameter;

            if (gs.method != null) {
                parameter = {
                    'method': gs.method,
                    'args': Sys.Serialization.JavaScriptSerializer.serialize(options.parameter)
                };
            }

            var str = Sys.Serialization.JavaScriptSerializer.serialize(parameter);

            var result = {
                type: type,
                param: str
            };

            return result;
        };

        ServiceOptions.getDelay = function () {
            var options = ServiceOptions.current();
            if ((typeof options.delay == 'number') && (options.delay > 0)) {
                return options.delay;
            } else {
                return 0;
            }
        };

        ServiceOptions.getInterval = function () {
            var options = ServiceOptions.current();
            if ((typeof options.interval == 'number') && (options.interval > 0)) {
                return options.interval;
            } else {
                return 0;
            }
        };

        ServiceOptions.run = function () {
            var options = ServiceOptions.current();
            var delay = ServiceOptions.getDelay();
            var interval = ServiceOptions.getInterval();

            if ((delay == 0) && (interval == 0)) {
                ServiceOptions.ajax();
            } else {
                var cleaner = ServiceOptions.cleaner();

                if ((delay > 0) && (interval == 0)) {
                    cleaner.timerHandle = setTimeout(ServiceOptions.ajax, delay);
                } else {
                    function repeat() {
                        cleaner.intervalHandle = setInterval(ServiceOptions.ajax, interval);
                    }

                    cleaner.timerHandle = setTimeout(repeat, delay);
                }
            }
        };

        ServiceOptions.counter = (function () {
            var i = 0;

            return {
                times: function () { return i; },
                next: function () { return i++; },
                reset: function () { i = 0; }
            };
        })();

        function GeneralService() { }

        GeneralService.prototype = {
            method: null,

            exec: function (parameter, success, error, data, context) {
                if ((typeof type != 'string') || (type.length == 0)) {
                    throw new Error("The service component type is not set.");
                }

                this.prepare.apply(this, arguments);
                ServiceOptions.run();

                return this;
            },

            terminate: function () {
                var cleaner = ServiceOptions.cleaner();

                cleaner.clear();

                return this;
            },

            prepare: function (parameter, success, error, data, context) {
                ServiceOptions.init.apply(ServiceOptions, arguments);

                return this;
            },

            forward: function (methodName) {
                if (methodName == null) {
                    this.method = null;
                } else {
                    if (typeof methodName !== 'string') {
                        throw Error('methodName parameter should be string type.');
                    }

                    var methodName = $.trim(methodName);

                    this.method = (methodName != '') ? methodName : null;
                }

                return this;
            },

            clone: function (methodName) {
                var gs = main.call(self);

                if (methodName == null) {
                    methodName = this.method;
                }

                gs.forward(methodName);

                return gs;
            },

            create: function (serviceType) {
                if (typeof serviceType != 'string') {
                    throw new Error("Should set service component serviceType.");
                }

                serviceType = $.trim(serviceType);

                if (serviceType.length == 0) {
                    throw new Error("Service serviceType should not be empty.");
                }

                return main.call({'url': self.url, 'type': serviceType});
            },

            reset: ServiceOptions.counter.reset,

            getTimes: ServiceOptions.counter.times,

            getUrl: function () {
                return url;
            },

            getType: function () {
                return type;
            }
        };

        gs = new GeneralService();

        return gs;
    }

    if (typeof window[KEY] === 'function') {
        return;
    }

    window[KEY] = main;
})(jQuery);