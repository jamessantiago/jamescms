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
    public class WebSocketFilePreview : IDisposable
    {
        private FilePreview filePreview;
        private WebSocketServer server;
        private IWebSocketConnection socket;

        private Logger logger = LogManager.GetLogger("WebSocketFileTail");
        private string filePath;
        private string serverName;

        public WebSocketFilePreview(string FilePath, string ServerName)
        {
            serverName = ServerName;
            filePath = FilePath;
        }

        public void Start()
        {
            server = new WebSocketServer("ws://santiagodevelopment.com:8991/" + serverName);
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
            filePreview = new FilePreview(filePath);
            filePreview.FileRead += new EventHandler(filepreview_FileReadChangesArrived);
            filePreview.ChangesArrived += new EventHandler(filepreview_FileReadChangesArrived);
            filePreview.Error += new UnhandledExceptionEventHandler(filepreview_Error);
            filePreview.StartTrackingFileTail();

        }

        private void filepreview_FileReadChangesArrived(object sender, EventArgs e)
        {
            var message = filePreview.FileText;
            if (!string.IsNullOrEmpty(message))
            {
                message = MarkdownHelper.Markdown(message, null).ToString();
                socket.Send(message);
            }
        }

        private void filepreview_Error(object sender, UnhandledExceptionEventArgs e)
        {
            logger.DebugException("File preview reported an error", (Exception)e.ExceptionObject);
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

        ~WebSocketFilePreview()
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
                if (filePreview != null)
                {
                    filePreview.Dispose();
                    filePreview = null;
                }
            }
        }

        #endregion Dispose
    }
}