using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using jamescms.Models;
using NLog;

namespace jamescms.Helpers
{
    public static class TextFileHelper
    {
        private static Logger logger = LogManager.GetLogger("TextFileHelper");

        public static void PushTextFiles()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "TextFiles");
            if (Directory.Exists(filePath))
            {
                logger.Debug("Writing texts to " + filePath);
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var texts = uow.tc.Texts;
                    foreach (var text in texts)
                    {
                        string textPath = Path.Combine(filePath, text.UrlTitle + ".md");
                        File.WriteAllText(textPath, text.CraftTextString());
                    }
                }
            }
        }

        public static void PullTextFiles()
        {

        }

        public static string CraftTextString(this Text text)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("----");
            sb.AppendLine("Posted: " + text.Posted.ToString("dd-MMM-yyyy"));
            sb.AppendLine("Title: " + text.Title);
            sb.AppendLine("----");
            sb.AppendLine(text.Article);
            return sb.ToString();
        }
    }
}