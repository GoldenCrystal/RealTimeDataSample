"use strict";

(function ($, Gauge) {
    var performanceHub = $.connection.performanceHub;

    var ioPerSecondGauge = new Gauge("svg#ioPerSecond", { left: 120, top: 10, right: 10, bottom: 10 }, 800);
    var ioPerMinuteGauge = new Gauge("svg#ioPerMinute", { left: 120, top: 10, right: 10, bottom: 10 }, 800);

    performanceHub.client.ioPerSecond = function (value) {
        ioPerSecondGauge.update(value);
    };

    performanceHub.client.ioPerMinute = function (value) {
        ioPerMinuteGauge.update(value);
    };

    $.connection.hub.start();

    $.connection.hub.disconnected(function () {
        setTimeout(function () {
            $.connection.hub.start();
        }, 5000);
    });
})(jQuery, Gauge);