using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using jamescms.Helpers;
using jamescms.Models;
using System.IO;
using NLog;
using Fleck;
using System.Net;

namespace jamescms.Services.WebSocketControllers
{
    public class WebSocketQuizGame
    {
        private Logger logger = LogManager.GetLogger("WebSocketQuizGame");
        private WebSocketServer server;
        private IWebSocketConnection socket;

        public void Start()
        {
            server = new WebSocketServer("ws://santiagodevelopment.com:8990/quizgame");
            try
            {
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
        }

        public void HandleChatMessage(string message)
        {

        }
    }
}