using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using jamescms.Helpers;
using jamescms.Models;
using System.IO;
using NLog;
using Fleck;

namespace jamescms.Services.WebSocketControllers
{
    public class WebSocketFileTail
    {
        private FileTail fileTail;
        private WebSocketServer server;
        private IWebSocketConnection socket;

        private Logger logger = LogManager.GetLogger("WebSocketFileTail");
        private string filePath;

        public WebSocketFileTail(string FilePath)
        {
            filePath = FilePath;
        }

        public void Start()
        {
            server = new WebSocketServer("ws://localhost:8989/FileTail");

            try
            {
                server.Start(socket =>
                {
                    socket.OnOpen = () => InitiateFileTail(socket);
                    socket.OnError = error => logger.DebugException("Error occurred establishing new websocket", error);
                    //socket.OnMessage = message => logger.Debug("Websocket message received: " + message);
                });                
            }
            catch (Exception ex)
            {
                logger.DebugException("Failed to establish file tail web socket", ex);
            }
        }

        private void InitiateFileTail(IWebSocketConnection Socket)
        {
            socket = Socket;
            //logger.Debug("Initiating file tail");
            fileTail = new FileTail(filePath, true);
            fileTail.FileRead += new EventHandler(filetail_FileReadChangesArrived);
            fileTail.ChangesArrived += new EventHandler(filetail_FileReadChangesArrived);
            fileTail.Error += new UnhandledExceptionEventHandler(filetail_Error);
            fileTail.StartTrackingFileTail();
        }

        private void filetail_FileReadChangesArrived(object sender, EventArgs e)
        {
            //logger.Debug("File changes triggered");
            var message = fileTail.Changes.Replace("\n", "<br />");
            if (!string.IsNullOrEmpty(message))
                socket.Send(message);
        }

        private void filetail_Error(object sender, UnhandledExceptionEventArgs e)
        {
            logger.DebugException("File tail reported an error", (Exception)e.ExceptionObject);
        }

        private void KillFileTail()
        {
            //logger.Debug("Closing file tail websocket");
            socket.Close();            
        }
    }
}