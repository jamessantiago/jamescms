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

namespace jamescms.Services.WebSocketControllers
{
    public class WebSocketQuizGame
    {
        private Logger logger = LogManager.GetLogger("WebSocketQuizGame");
        private WebSocketServer server;
        private IWebSocketConnection socket;
        private QuizGame quizGame;
        private int currentMessageIndex = 0;

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
            socket = Socket;            
            quizGame = QuizGame.Instance;
            quizGame.MessageArrived += quizGame_MessageArrived;
            quizGame_MessageArrived(null, null);            
        }

        private void quizGame_MessageArrived(object sender, EventArgs args)
        {
            logger.Debug("new message event triggered");
            foreach (var message in quizGame.Messages.Where(d => d.Key > currentMessageIndex))
            {
                logger.Debug("Sending message to client: " + message);
                socket.Send(message.Value);
            }
            currentMessageIndex = quizGame.Messages.Last().Key;
        }

        public void HandleChatMessage(string message)
        {
            logger.Debug("message arrived: " + message);
            var msg = JsonConvert.DeserializeObject<QuizGameMessage>(message);
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
                                socket.Send(oldMessage.Value);
                        }
                        break;
                    default:
                        break;
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