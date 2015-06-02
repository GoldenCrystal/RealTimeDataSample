"use strict";

var Gauge = (function (d3) {
    // Creates a new instance of the Gauge class.
    return function Gauge(element, margin, transitionDuration) {
        var _margin = { left: 50, top: 10, right: 10, bottom: 10 };
        var _transitionDuration = typeof transitionDuration === "number" ? +transitionDuration : 250;

        var _min = null;
        var _max = null;
        var _value = null;

        var _element = d3.select(element);

        if (_element.node().tagName !== "svg") {
            _element = _element.append("svg");
        }

        var _groupElement = _element.append("g");

        _groupElement.append("g").attr("class", "grid");
        _groupElement.append("g").attr("class", "chart");
        _groupElement.append("g").attr("class", "axis");

        var _y = d3.scale.linear();
        var _yAxis = d3.svg.axis().scale(_y).orient("left");

        if (margin !== undefined && margin !== null) setMargin(margin);

        function getMargin() {
            return {
                left: _margin.left,
                top: _margin.top,
                right: _margin.right,
                bottom: _margin.bottom,
            };
        }

        function setMargin(value) {
            if (!value) value = 0;

            if (typeof value === 'number') {
                _margin.left = value;
                _margin.top = value;
                _margin.right = value;
                _margin.bottom = value;
            } else {
                if (value.left) _margin.left = value.left;
                if (value.top) _margin.top = value.top;
                if (value.right) _margin.right = value.right;
                if (value.bottom) _margin.bottom = value.bottom;
            }

            update();
        }

        function getTransitionDuration() {
            return _transitionDuration;
        }

        function setTransitionDuration(value) {
            _transitionDuration = +value;

            if (_transitionDuration < 0) _transitionDuration = 0;
        }

        function getValue() {
            return _value;
        }

        function setValue(value) {
            _value = typeof value === "number" ? value : 0;
            update();
        }

        function getMin() {
            return _min;
        }

        function setMin(value) {
            _min = typeof value === "number" ? value : 0;
            update();
        }

        function getMax() {
            return _max;
        }

        function setMax(value) {
            _max = typeof value === "number" ? value : 0;
            update();
        }

        // Updates the gauge. All parameters are facultative.
        function update(value, min, max) {
            if (arguments.length > 0) {
                if (arguments.length == 1) {
                    if (_min === null || _min > value) _min = value;
                    if (_max === null || _max < value) _max = value;
                } else {
                    if (min !== undefined && min !== null) { _min = min; }
                    if (max !== undefined && max !== null) { _max = max; }
                }
            }

            var initialize = _value === null;

            _value = typeof value === "number" ? value : 0;

            var rect = _element.node().getBoundingClientRect();

            var width = rect.width - margin.left - margin.right;
            var height = rect.height - margin.top - margin.bottom;

            _y.domain([_min, _max]).rangeRound([height, 0]).nice();

            _groupElement.transition()
                .ease("sin-in-out")
                .duration(initialize ? 0 : _transitionDuration)
                .attr("transform", "translate(" + margin.left + ", " + margin.top + ")");

            var chart = _groupElement.select("g.chart");

            chart.transition()
                .ease("sin-in-out")
                .duration(_transitionDuration);

            var grid = _groupElement.select("g.grid");

            grid.transition()
                .ease("sin-in-out")
                .duration(_transitionDuration);

            var gridLines = grid.selectAll("line.grid").data(_y.ticks());

            gridLines.enter().append("line").attr("class", "grid");
            gridLines.exit().remove();

            gridLines.transition()
                .ease("sin-in-out")
                .duration(_transitionDuration)
                .attr("x1", "0")
                .attr("x2", width)
                .attr("y1", _y)
                .attr("y2", _y);

            var dataRect = chart.selectAll("rect.bar").data([_value]);

            dataRect.enter().append("rect").attr("class", "bar");
            dataRect.exit().remove();

            dataRect.transition()
                .ease("sin-in-out")
                .duration(_transitionDuration)
                .attr("x", 10)
                .attr("y", function (d) { return _y(d); })
                .attr("width", width - 20)
                .attr("height", function (d) { return height - _y(d); });

            var maxLine = chart.selectAll("line.max").data([_max]);

            maxLine.enter().append("line").attr("class", "max");
            maxLine.exit().remove();

            maxLine.transition()
                .ease("sin-in-out")
                .duration(_transitionDuration)
                .attr("x1", "0")
                .attr("x2", width)
                .attr("y1", _y)
                .attr("y2", _y);

            var minLine = chart.selectAll("line.min").data([_min]);

            minLine.enter().append("line").attr("class", "min");
            minLine.exit().remove();

            minLine.transition()
                .ease("sin-in-out")
                .duration(_transitionDuration)
                .attr("x1", "0")
                .attr("x2", width)
                .attr("y1", _y)
                .attr("y2", _y);

            _groupElement.select("g.axis")
                .transition()
                .ease("sin-in-out")
                .duration(_transitionDuration)
                .call(_yAxis);
        }

        Object.defineProperties(this, {
            "update": { get: function () { return update; } },
            "margin": { get: getMargin, set: setMargin },
            "value": { get: getValue, set: setValue },
            "min": { get: getMin, set: setMin },
            "max": { get: getMax, set: setMax },
            "transitionDuration": { get: getTransitionDuration, set: setTransitionDuration },
        });
    };
})(d3);
