using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Fleck;

namespace jamescms.Services.WebSocketControllers
{
    public class WebSocketListener
    {
        public void InitializeListener()
        {
            var server = new WebSocketServer("ws://localhost:8989/FileTail");

            server.Start(socket =>
            {
                socket.OnOpen = () => Console.WriteLine("Open!");
                socket.OnClose = () => Console.WriteLine("Close!");
                socket.OnMessage = message => socket.Send(message);
            });

            //var listener = new TcpListener(IPAddress.Loopback, 8989);
            //try
            //{
            //    listener.Start();
            //    using (var client = listener.AcceptTcpClient())
            //    using (var stream = client.GetStream())
            //    using (var reader = new StreamReader(stream))
            //    using (var writer = new StreamWriter(stream))
            //    {
            //        writer.WriteLine("HTTP/1.1 101 Web Socket Protocol Handshake");
            //        writer.WriteLine("Upgrade: WebSocket");
            //        writer.WriteLine("Connection: Upgrade");
            //        writer.WriteLine("WebSocket-Origin: http://localhost:50695");
            //        writer.WriteLine("WebSocket-Location: ws://localhost:8989/websession");
            //        writer.WriteLine("");
            //    }
            //    listener.Stop();
            //}
            //catch
            //{ }
        }
    }
}