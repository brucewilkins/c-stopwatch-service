//$.connection.hub.url = "";
var stopwatch = $.connection.stopwatchHub;
//$.connection.headers.add("Authorization", "BASIC QnJ1Y2U6Q2hhbWE=");

stopwatch.client.updateElapsedTime = function (stopwatches)
{
    $("#stopwatches").empty();
    for (var i = 0; i < stopwatches.length; i++)
        $("#stopwatches").append('<li><strong>' + stopwatches[i].stopwatchName + '</strong> (elapsed: ' + stopwatches[i].elapsed + ' ms)');
}

$("#sendMessage").click(function () {
    stopwatch.server.start($("#stopwatchName").val());
});

$("#refreshList").click(function () {
    stopwatch.server.getStopwatches().done(function (stopwatches) {
        $("#stopwatches").empty();
        for (var i = 0; i < stopwatches.length; i++)
            $("#stopwatches").append('<li><strong>' + stopwatches[i].stopwatchName + '</strong> (elapsed: ' + stopwatches[i].elapsed + ' ms)');
    });
});

$.connection.hub.logging = true;
$.connection.hub.start().done(function () {
    $("#status").text('Connected');
});