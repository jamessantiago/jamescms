using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XSockets;
using XSockets.Core.XSocket;
using System.Diagnostics;

namespace jamescms.Controllers
{
    public class SocketsController : XSocketController
    {

        public SocketsController()
        {
            Debug.AutoFlush = true;
        }

        public void noBinding()
        {
            Debug.WriteLine("no binding called");
        }
    }
}
