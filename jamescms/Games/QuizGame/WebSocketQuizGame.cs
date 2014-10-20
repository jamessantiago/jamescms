using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using jamescms.Helpers;
using jamescms.Models;
using jamescms.Games;
using System.IO;
using NLog;
using Fleck;
using System.Net;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Timers;

namespace jamescms.Services.WebSocketControllers
{
    public class WebSocketQuizGame
    {

        private Logger logger = LogManager.GetLogger("WebSocketQuizGame");
        private WebSocketServer server;
        private List<IWebSocketConnection> sockets = new List<IWebSocketConnection>();
        private object socketSync = new object();
        //private IWebSocketConnection socket;
        private QuizGame quizGame;
        private int currentMessageIndex = 0;
        private Timer clientPing;
        private Dictionary<Guid, int> clientConnections = new Dictionary<Guid,int>();

        public bool QuizGameStarted = false;

        public string[] Connections
        {
            get
            {
                if (sockets != null)
                    return sockets.Select(d => d.ConnectionInfo.Id.ToString()).ToArray();
                else
                    return new string[]{""};
            }
        }

        public void Start()
        {   
            server = new WebSocketServer("ws://santiagodevelopment.com:8990/quizgame");
            try
            {
                logger.Debug("Initializing quiz game");                
                server.Start(socket =>
                {
                    socket.OnOpen = () => StartQuizGame(socket);
                    socket.OnError = error => logger.DebugException("Error occurred establishing new websocket", error);
                    socket.OnMessage = message => HandleChatMessage(message);      
                });
            }
            catch (Exception ex)
            {
                logger.DebugException("Failed to establish quiz game socket", ex);
            }
        }

        public void StartQuizGame(IWebSocketConnection Socket)
        {
            //if quiz game already started send messages in memory to new connection
            if (quizGame != null && quizGame.Messages != null && quizGame.Messages.Any())
            {
                foreach (var message in quizGame.Messages.OrderBy(d => d.Key))
                {
                    QuizMessage msg = new JavaScriptSerializer().Deserialize<QuizMessage>(message.Value);
                    if (msg.SocketId == Guid.Empty)
                    {
                        Socket.Send(message.Value);
                    }
                }
            }
            quizGame = QuizGame.Instance;
            QuizGameStarted = true;
            lock (socketSync)
            {
                if (!sockets.Any(d => d == Socket))
                    sockets.Add(Socket);
            }
            AuthenticateUser(Socket.ConnectionInfo.Cookies, Socket);
            if (sockets.Count == 1)
            {
                quizGame.MessageArrived += quizGame_MessageArrived;
                quizGame_MessageArrived(null, null);
                clientPing = new Timer(15000);
                clientPing.Elapsed += clientPing_Elapsed;
                clientPing.Start();
            }
        }

        void clientPing_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                foreach (var conn in clientConnections)
                {
                    if (conn.Value > 2)
                    {
                        var socket = sockets.FirstOrDefault(d => d.ConnectionInfo.Id == conn.Key);
                        if (socket != null)
                        {
                            lock (socketSync)
                            {
                                sockets.Remove(socket);
                                socket.Close();
                                QuizGame.Instance.UserQuit(socket.ConnectionInfo.Id);
                            }
                        }
                    }
                }

                foreach (var socket in sockets)
                {
                    if (!clientConnections.Any(d => d.Key == socket.ConnectionInfo.Id))
                        clientConnections.Add(socket.ConnectionInfo.Id, 0);

                    QuizMessage msg = new QuizMessage()
                    {
                        Type = "Ping",
                        Message = socket.ConnectionInfo.Id.ToString()
                    };
                    string pingClient = new JavaScriptSerializer().Serialize(msg);
                    socket.Send(pingClient);
                    clientConnections[socket.ConnectionInfo.Id]++;
                }
            }
            catch (Exception ex)
            {
                logger.DebugException("clientPing_Elapsed", ex);
            }
        }

        private void quizGame_MessageArrived(object sender, EventArgs args)
        {
            try
            {
                lock (socketSync)
                {
                    foreach (var message in quizGame.Messages.Where(d => d.Key > currentMessageIndex))
                    {
                        QuizMessage msg = new JavaScriptSerializer().Deserialize<QuizMessage>(message.Value);
                        if (msg.SocketId != Guid.Empty)
                        {
                            var socket = sockets.FirstOrDefault(d => d.ConnectionInfo.Id == msg.SocketId);
                            if (socket != null)
                                socket.Send(message.Value);
                        }
                        else
                        {
                            foreach (var socket in sockets)
                                socket.Send(message.Value);
                        }
                    }
                    currentMessageIndex = quizGame.Messages.Keys.Max();
                }
            }
            catch (Exception ex)
            {
                logger.DebugException("quizGame_MessageArrived", ex);
            }
            
        }

        public void HandleChatMessage(string message)
        {
            try
            {
                logger.Debug("message arrived: " + message);
                var msg = JsonConvert.DeserializeObject<QuizGameMessage>(message);
                msg.Message = HttpUtility.HtmlEncode(msg.Message);
                if (msg != null)
                {
                    switch (msg.Type.ToLower())
                    {
                        case "chat":
                            quizGame.AttemptAnswer(msg.Message, msg.User);
                            break;
                        case "get":
                            int index = 0;
                            if (int.TryParse(msg.Message, out index))
                            {
                                foreach (var oldMessage in quizGame.Messages.Where(d => d.Key > index))
                                    foreach (var socket in sockets)
                                        socket.Send(oldMessage.Value);
                            }
                            break;
                        case "pong":
                            Guid sockedId;
                            if (Guid.TryParse(msg.Message, out sockedId))
                            {
                                if (clientConnections.ContainsKey(sockedId))
                                    clientConnections[sockedId] = 0;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.DebugException("HandleChatMessage", ex);
            }
        }

        private void AuthenticateUser(IDictionary<string, string> SocketCookies, IWebSocketConnection socket)
        {
            try
            {
                if (SocketCookies.Any(d => d.Key == ".ASPXAUTH"))
                {
                    var userData = System.Web.Security.FormsAuthentication.Decrypt(SocketCookies[".ASPXAUTH"]);
                    if (!userData.Expired)
                    {

                        QuizMessage msg = new QuizMessage()
                        {
                            Type = "SetName",
                            Message = userData.Name
                        };
                        string setUser = new JavaScriptSerializer().Serialize(msg);
                        socket.Send(setUser);
                        using (UnitOfWork uow = new UnitOfWork())
                        {
                            var userProfile = uow.uc.UserProfiles.FirstOrDefault(d => d.UserName == userData.Name);
                            if (userProfile != null)
                            {
                                var gameProfile = uow.qg.UserGameProfiles.FirstOrDefault(d => d.AccountModelUserId == userProfile.UserId);
                                if (gameProfile == null)
                                {
                                    UserGameProfile newProfile = new UserGameProfile()
                                    {
                                        AccountModelUserId = userProfile.UserId,
                                        Attempts = 0,
                                        CorrectAnswers = 0,
                                        LastTimeSeen = DateTime.Now,
                                        Points = 0
                                    };
                                    uow.qg.UserGameProfiles.Add(newProfile);
                                    uow.qg.SaveChanges();
                                    gameProfile = newProfile;
                                }
                                else
                                {
                                    gameProfile.LastTimeSeen = DateTime.Now;
                                }
                                uow.qg.SaveChanges();
                            }
                        }
                        QuizGame.Instance.UserJoin(socket.ConnectionInfo.Id, userData.Name);
                    }
                }
                else
                {
                    Random random = new Random();
                    int userid = random.Next(0, 1000);
                    while (QuizGame.Instance.Users.Any(d => d.UserName == "Guest" + userid.ToString()))
                        userid = random.Next(0, 1000);
                    string username = "Guest" + userid.ToString();
                    QuizMessage msg = new QuizMessage()
                    {
                        Type = "SetName",
                        Message = username
                    };
                    string setUser = new JavaScriptSerializer().Serialize(msg);
                    socket.Send(setUser);
                    QuizGame.Instance.UserJoin(socket.ConnectionInfo.Id, username);
                }
            }
            catch (Exception ex)
            {
                logger.DebugException("AuthenticateUser", ex);
            }
        }
        
    }

    public class QuizGameMessage
    {
        public string Type { get; set; }
        public string User { get; set; }
        public string Message { get; set; }
    }
}