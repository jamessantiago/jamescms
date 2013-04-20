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

namespace jamescms.Services.WebSocketControllers
{
    public class WebSocketQuizGame
    {

        private Logger logger = LogManager.GetLogger("WebSocketQuizGame");
        private WebSocketServer server;
        private List<IWebSocketConnection> sockets = new List<IWebSocketConnection>();
        //private IWebSocketConnection socket;
        private QuizGame quizGame;
        private int currentMessageIndex = 0;

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
            server = new WebSocketServer("ws://localhost:8990/quizgame");
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
                    Socket.Send(message.Value);
                }
            }
            quizGame = QuizGame.Instance;
            QuizGameStarted = true;
            if (!sockets.Any(d => d == Socket))
                sockets.Add(Socket);
            AuthenticateUser(Socket.ConnectionInfo.Cookies, Socket);
            if (sockets.Count == 1)
            {
                quizGame.MessageArrived += quizGame_MessageArrived;
                quizGame_MessageArrived(null, null);
            }
        }

        private void quizGame_MessageArrived(object sender, EventArgs args)
        {
            foreach (var message in quizGame.Messages.Where(d => d.Key > currentMessageIndex))
            {
                foreach (var socket in sockets)
                    socket.Send(message.Value);
            }
            currentMessageIndex = quizGame.Messages.Last().Key;
        }

        public void HandleChatMessage(string message)
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
                    default:
                        break;
                }
            }
        }

        private void AuthenticateUser(IDictionary<string, string> SocketCookies, IWebSocketConnection socket)
        {
            if (SocketCookies.Any(d => d.Key == ".ASPXAUTH"))
            {
                var userData = System.Web.Security.FormsAuthentication.Decrypt(SocketCookies[".ASPXAUTH"]);
                if (!userData.Expired)
                {
                    QuizGame.Instance.UserJoin(socket.ConnectionInfo.Id, userData.Name);
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
                            }
                            else
                            {
                                gameProfile.LastTimeSeen = DateTime.Now;
                            }
                            uow.qg.SaveChanges();

                            QuizMessage points = new QuizMessage()
                            {
                                Type = "SetPoints",
                                Message = gameProfile.Points != null ? gameProfile.Points.ToString() : "0"
                            };
                            string setPoints = new JavaScriptSerializer().Serialize(points);
                            socket.Send(setPoints);
                        }
                    }
                }
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