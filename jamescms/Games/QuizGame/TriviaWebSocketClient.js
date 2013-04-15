var socket;
var connectionAttempts = 0;
var MAX_CONNECTIONS = 4
var user = "";

startGame();

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
    socket = new WebSocket("ws://santiagodevelopment.com:8990/quizgame" + sessionId);
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
        if (data.To.toLowerCase() == "all" || data.To.toLowerCase() == user)
        {
            $("#chatWindow").append(data.From + ": " + data.Message + "\n");
        }
        }
};