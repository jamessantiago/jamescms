using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System;
using XSockets.Core.Common.Socket.Event.Arguments;
using XSockets.Core.XSocket;
using XSockets.Core.XSocket.Helpers;
using System.Diagnostics;
using jamescms.Helpers;

namespace XSocketServerPlugins
{
    public class FileXSocket : XSocketController
    {
        private FileTail fileTail;

        public FileXSocket(string FilePath)
        {
            fileTail = new FileTail(FilePath, true);
            fileTail.ChangesArrived += fileTail_ChangesArrived;
            SendFullFile();
        }

        public void SendFullFile()
        {            
            this.Send(fileTail.Changes, "FullFile");
        }

        public void fileTail_ChangesArrived(object sender, EventArgs e)
        {
            this.Send(fileTail.Changes, "Changes");
        }

    }
}