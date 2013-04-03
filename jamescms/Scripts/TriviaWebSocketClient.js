var socket;
var connectionAttempts = 0;
var user = "";

function startGame()
{
    establishConnection();
    user = $("#username").val();
}

function SendMessage()
{
    var message = $("#chatInput").val();
    socket.send("{'Type':'chat', 'User': '"+ user + "', 'Message':'" + message + "'}")
    $("#chatInput").val('');
}

function MessageReceived(message)
{
    $("#chatWindow").append(message);
}


function establishConnection() {
    if (socket != null) {
        socket.close();
    }
    socket = new WebSocket("ws://santiagodevelopment.com:8989/@Html.Raw(Session.SessionID)");
    socket.onopen = function (connection) {
        $("#status").html("connected");
    };
    socket.onerror = function (error) {
        $("#status").html(error.data);
    };
    socket.onclose = function () {
        $("#data").append("closed");
    };
    socket.onmessage = function (message) {
        var data = JSON.par(message.data)
        data.
    };
    
};