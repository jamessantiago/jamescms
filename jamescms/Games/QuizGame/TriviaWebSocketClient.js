var socket;
var connectionAttempts = 0;
var MAX_CONNECTIONS = 4
var user = "";

startGame();
$("#chatInput").focus();

function startGame()
{
    $("#status").html("connecting");
    establishConnection();
    user = GenerateUser();
}

function SendMessage()
{
    var message = $("#chatInput").val();
    socket.send("{'Type':'chat', 'User': '"+ user + "', 'Message':'" + message + "'}")
    $("#chatInput").val('');
}

function htmlEncode(value) {
    return $("<div/>").text(value).html();
}

function MessageReceived(message)
{
    $("#chatWindow").append(message);
}

function GenerateUser()
{
    return "User" + Math.floor(Math.random() * 1001).toString();
}

$("#sendChat").click(function() {
    SendMessage();
});

function establishConnection() {
    if (socket != null) {
        socket.close();
    }
    socket = new WebSocket("ws://localhost:8990/quizgame");
    socket.onopen = function (connection) {
        $("#status").html("connected");
    };
    socket.onerror = function (error) {
        $("#status").html(error.data);
        if (connectionAttempts < MAX_CONNECTIONS) {
            connectionAttempts++;
            establishConnection();
        }
    };
    socket.onclose = function () {
        $("#status").html("closed");
    };
    socket.onmessage = function (message) {        
        var data = JSON.parse(message.data);
        if (data.To == "All" || data.To == user)
        {
            $("#chatWindow").append("<span style='color:#2d7b44'>" + data.From + ":</span> " + data.Message + "\n");
            $("#chatWindow").prop({ scrollTop: $("#chatWindow").prop("scrollHeight") });
        }
        else if (data.Type == "SetName")
        {
            user = data.Message;
        }
    }
};