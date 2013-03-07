using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using jamescms.Helpers;
using jamescms.Models;
using System.IO;
using NLog;
using Fleck;
using System.Net;

namespace jamescms.Services.WebSocketControllers
{
    public class WebSocketFileTail : IDisposable
    {
        private FileTail fileTail;
        private WebSocketServer server;
        private IWebSocketConnection socket;

        private Logger logger = LogManager.GetLogger("WebSocketFileTail");
        private string filePath;
        private string serverName;

        public WebSocketFileTail(string FilePath, string ServerName)
        {
            serverName = ServerName;
            filePath = FilePath;
        }

        public void Start()
        {
            server = new WebSocketServer("ws://localhost:8989/" + serverName);
            try
            {                
                server.Start(socket =>
                {
                    socket.OnOpen = () => InitiateFileTail(socket);
                    socket.OnError = error => logger.DebugException("Error occurred establishing new websocket", error);
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
            fileTail = new FileTail(filePath, true);
            fileTail.FileRead += new EventHandler(filetail_FileReadChangesArrived);
            fileTail.ChangesArrived += new EventHandler(filetail_FileReadChangesArrived);
            fileTail.Error += new UnhandledExceptionEventHandler(filetail_Error);
            fileTail.StartTrackingFileTail();

        }

        private void filetail_FileReadChangesArrived(object sender, EventArgs e)
        {
            var message = fileTail.Changes;
            if (!string.IsNullOrEmpty(message))
                socket.Send(message);
        }

        private void filetail_Error(object sender, UnhandledExceptionEventArgs e)
        {
            logger.DebugException("File tail reported an error", (Exception)e.ExceptionObject);
        }

        private void KillFileTail()
        {
            socket.Close();            
        }

        #region Dispose

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~WebSocketFileTail()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (socket != null)
                {
                    socket.Close();
                    socket = null;
                }
                if (server != null)
                {
                    server.ListenerSocket.Close();
                    server.Dispose();
                    server = null;
                }
                if (fileTail != null)
                {
                    fileTail.Dispose();
                    fileTail = null;
                }
            }
        }

        #endregion Dispose
    }
}