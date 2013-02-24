using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace jamescms.Services
{
    public class WebSocketListener_test
    {
        public void InitializeListener()
        {
            var listener = new TcpListener(IPAddress.Loopback, 8989);
            try
            {
                listener.Start();
                using (var client = listener.AcceptTcpClient())
                using (var stream = client.GetStream())
                using (var reader = new StreamReader(stream))
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("HTTP/1.1 101 Web Socket Protocol Handshake");
                    writer.WriteLine("Upgrade: WebSocket");
                    writer.WriteLine("Connection: Upgrade");
                    writer.WriteLine("WebSocket-Origin: http://localhost:50695/guide");
                    writer.WriteLine("WebSocket-Location: ws://localhost:8989/websession");
                    writer.WriteLine("");
                    writer.WriteLine("test");
                }
                listener.Stop();
            }
            catch
            { }
        }
    }
}