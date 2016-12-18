var userName = prompt("Enter your name: ");
var stopwatch = $.connection.stopwatchHub;
$connection.headers.add("Authorization", "BASIC QnJ1Y2U6Q2hhbWE=");
stopwatch.client.messageReceived = function (originatorUser, message) {
    $("#messages").append('<li><strong>' + originatorUser + '</strong>: ' + message);
};

//chat.client.getConnectedUsers = function (userList) {
//    for (var i = 0; i < userList.length; i++)
//        addUser(userList[i]);
//};

//chat.client.newUserAdded = function (newUser) {
//    addUser(newUser);
//}

//$("#messageBox").focus();

//$("#sendMessage").click(function () {
//    chat.server.send(userName, $("#messageBox").val());
//    $("#messageBox").val("");
//    $("#messageBox").focus();
//});

//$("#messageBox").keyup(function (event) {
//    if (event.keyCode == 13)
//        $("#sendMessage").click();
//});

//function addUser(user) {
//    $("#userList").append('<li>' + user + '</li>');
//}

$.connection.hub.logging = true;
$.connection.hub.start().done(function () {
    stopwatch.server.connect(userName);
});