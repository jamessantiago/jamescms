using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using NLog;

namespace jamescms.Helpers
{
    public class FileTail : IDisposable
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
        public event EventHandler FileRead;
        public event UnhandledExceptionEventHandler Error;

        private StringBuilder changes = new StringBuilder();
        private long previousSize;
        private FileSystemWatcher fileWatcher;                
        private byte[] b = new byte[1024];
        private UTF8Encoding encoder = new UTF8Encoding(true);
        private string filePath;
        private bool autoFlush = false;
        private Logger logger = LogManager.GetLogger("filetail");


        public FileTail(string FilePath, bool AutoFlushChanges)
        {
            filePath = FilePath;
            autoFlush = AutoFlushChanges;
        }


        public void StartTrackingFileTail()
        {
            try
            {
                //logger.Debug("file tracking started for " + filePath);
                using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    int bytesRead;
                    while ((bytesRead = fileStream.Read(b, 0, b.Length)) > 0)
                    {
                        changes.Append(encoder.GetString(b, 0, bytesRead));
                    }
                    if (fileStream.Length > 0)
                        previousSize = fileStream.Length;
                    //logger.Debug("previousSize: " + previousSize);
                }
                if (FileRead != null)
                    FileRead(this, new EventArgs());
                string filename = Path.GetFileName(filePath);
                string path = filePath.Replace(filename, "");
                fileWatcher = new FileSystemWatcher(path, filename);
                fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
                fileWatcher.IncludeSubdirectories = false;
                fileWatcher.Changed += new FileSystemEventHandler(File_Changed);
                fileWatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                if (Error != null)
                    Error(this, new UnhandledExceptionEventArgs(ex, false));
            }
        }

        public void File_Changed(object source, FileSystemEventArgs e)
        {
            try
            {
                using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    fileStream.Seek(previousSize, SeekOrigin.Begin);
                    int bytesRead;
                    while ((bytesRead = fileStream.Read(b, 0, b.Length)) > 0)
                    {
                        changes.Append(encoder.GetString(b, 0, bytesRead));
                    }
                    if (fileStream.Length > 0)
                        previousSize = fileStream.Length;
                    //logger.Debug("previousSize: " + previousSize);
                    
                }
                if (ChangesArrived != null && changes.Length > 0)
                    ChangesArrived(this, new EventArgs());
            }
            catch (Exception ex)
            {
                if (Error != null)
                    Error(this, new UnhandledExceptionEventArgs(ex, false));
            }
        }        
        
        #region Dispose

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FileTail()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (fileWatcher != null)
                {
                    fileWatcher.EnableRaisingEvents = false;
                    fileWatcher.Dispose();
                    fileWatcher = null;
                }
                if (ChangesArrived != null)
                    ChangesArrived = null;
                if (Error != null)
                    Error = null;
                if (FileRead != null)
                    FileRead = null;
            }
        }

        #endregion Dispose

    }
}