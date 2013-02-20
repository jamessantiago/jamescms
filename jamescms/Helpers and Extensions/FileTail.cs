using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace jamescms.Helpers_and_Extensions
{
    public class FileTail
    {
        public string Changes { 
            get
            {
                if (autoFlush)
                {
                    string temp = changes.ToString();
                    changes.Clear();
                    return temp;
                }
                else
                    return changes.ToString();
            } }

        public event EventHandler ChangesArrived;

        private StringBuilder changes;
        private FileStream fileStream;
        private long previousSize;
        private FileSystemWatcher fileWatcher;                
        private byte[] b = new byte[1024];
        private UTF8Encoding encoder = new UTF8Encoding(true);
        private bool autoFlush = false;

        

        public FileTail(string FilePath, bool AutoFlushChanges)
        {
            autoFlush = AutoFlushChanges;
            fileStream = File.OpenRead(FilePath);
            while (fileStream.Read(b, 0, b.Length) > 0)
            {
                changes.Append(encoder.GetString(b));
            }
            previousSize = fileStream.Length;
            fileWatcher = new FileSystemWatcher(FilePath);
            fileWatcher.Changed += File_Changed;
        }

        public void File_Changed(object source, FileSystemEventArgs e)
        {
            fileStream.Seek(previousSize, SeekOrigin.Begin);
            while (fileStream.Read(b, 0, b.Length) > 0)
            {
                changes.Append(encoder.GetString(b));
            }
            previousSize = fileStream.Length;
            ChangesArrived(this, new EventArgs());
        }
        
    }
}