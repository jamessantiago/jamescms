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

        public void Start()
        {
            server = new WebSocketServer("ws://santiagodevelopment.com:8990/" + serverName);
            try
            {
                server.Start(socket =>
                {
                    socket.OnOpen = () => 
                    socket.OnError = error => logger.DebugException("Error occurred establishing new websocket", error);
                    socket.OnMessage = 
                });
            }
            catch (Exception ex)
            {
                logger.DebugException("Failed to establish file tail web socket", ex);
            }
        }

        public 
    }
}