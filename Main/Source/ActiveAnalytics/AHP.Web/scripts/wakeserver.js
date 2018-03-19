/*
Purpose: Checks if the BO Session has timed out.
*/
var fnDocReady = function ($, wakeServerPath, logoutUrl, window, dialog) {

    return function () {
        window.hasTimedout = false;
        var timer = null,
            fnShowDialog = function () {                
                if (window.hasTimedout) {
                    $("main").hide().remove();
                    $("#sessionExpiredPanel").show();
                    setTimeout(function () {
                        //redirect to logon page
                        window.location.href = logoutUrl;
                    }, 5000);
                }                
            },
            fnWakeServer = function () {
                if (window.hasTimedout) {
                    //show modal dialog
                    fnShowDialog();
                    //clear timer when timedout
                    clearInterval(timer);
                    return;
                }
                var jqXhr = $.post(wakeServerPath, "", null);
                jqXhr.done(function (rsp, status, xhr) {
                    if (rsp && rsp.Success) {
                        //user session available in system
                    } else {
                        window.hasTimedout = true;
                        //show modal dialog
                        fnShowDialog();
                    }
                })
                jqXhr.fail(function (rsp, status, xhr) {
                    window.hasTimedout = true;
                    //show modal dialog
                    fnShowDialog();
                });
            },
        //every 93 seconds ping server,session timeout is 1200 seconds(20 min),so eventually we will timeout at 1209(93*13 times)
        timeout = 93000;
        timer = setInterval(fnWakeServer, timeout);

        $(document).on('show', function () {
            // the page gained visibility
            fnWakeServer();
        });
    };   
};
//initialize the closure
$(fnDocReady($, window.wakeServerPath, window.logoutPath, window, $.featherlight));