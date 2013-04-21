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
    if (message.length > 0)
    {
        socket.send("{'Type':'chat', 'User': '" + user + "', 'Message':'" + message + "'}");
    }        
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
    $("#chatWindow").append("<span style='color:yellow'>Establishing connection to the game server</span><br/>");
    socket = new WebSocket("ws://santiagodevelopment.com:8990/quizgame");
    socket.onopen = function (connection) {
        $("#chatWindow").append("<span style='color:yellow'>Connection established</span><br/>");
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
        $("#chatWindow").append("<span style='color:red'>Connection to the game server has been closed.  Try refreshing the page</span><br/>");
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
        else if (data.Type == "Ping")
        {
            socket.send("{'Type':'pong', 'Message':'" + data.Message + "'}");
        }
        else if (data.Type == "SetStats")
        {
            $("#answered").html(data.Answered);
            $("#attempts").html(data.Attempts);
            $("#points").html(data.Points);
            $("#leader").html(data.Leader);
        }
        else if (data.Type == "SetUsers")
        {
            $("#userList").html("");
            $.each(data.Users, function (index, value) {
                $("#userList").append(value + "<br/>");
            });
        }
    }
};