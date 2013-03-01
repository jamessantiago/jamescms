using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using jamescms.Models;
using jamescms.Helpers;
using NLog;

namespace jamescms.Helpers
{
    public static class TextFileHelper
    {
        private static Logger logger = LogManager.GetLogger("TextFileHelper");

        private static string FilePath { get { return Path.Combine(Directory.GetCurrentDirectory(), "TextFiles"); } }
        private static string RepoPath { get { return Path.Combine(FilePath, "TextFiles.git"); } }

        public static void PushTextFiles()
        {
            string filePath = FilePath;
            string repoPath = RepoPath;
            if (Directory.Exists(filePath) && Directory.Exists(repoPath))
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
                    GitHelper.CommitChanges("Changes from website", repoPath);
                    string headSha = GitHelper.GetHeadCommitSha(repoPath);
                    uow.tc.Settings.First().HeadSha = headSha;
                    logger.Debug("Git head is now at " + headSha);
                }
            }
            else
                logger.Debug("Missing TextFiles path or git repo");
        }

        public static void PullTextFiles()
        {
            string filePath = FilePath;
            string repoPath = RepoPath;
            if (Directory.Exists(filePath) && Directory.Exists(repoPath))
            {
                logger.Debug("Pulling texts from " + filePath);
                using (UnitOfWork uow = new UnitOfWork())
                {
                    foreach (var file in Directory.GetFiles(filePath))
                    {
                        string fileData = File.ReadAllText(file);
                        string urlTitle = Path.GetFileNameWithoutExtension(file);
                        Text model = fileData.ToText();
                        var text = uow.tc.Texts.Where(d => d.UrlTitle == urlTitle).FirstOrDefault();
                        if (text != null)
                        {
                            text.Posted = model.Posted;
                            text.Title = model.Title;
                            text.Article = model.Article;
                            text.Updated = DateTime.Now;
                        }
                        else
                        {
                            Text newText = new Text()
                            {
                                Article = model.Article,
                                Posted = model.Posted,
                                Title = model.Title,
                                UrlTitle = urlTitle,
                                Updated = model.Updated
                            };
                            uow.tc.Texts.Add(newText);
                        }
                    }
                    uow.tc.SaveChanges();
                }
            }
            else
                logger.Debug("Missing TextFiles path or git repo");
        }

        public static Text ToText(this string fileData)
        {
            Text text = new Text();
            StringReader sr = new StringReader(fileData);
            sr.ReadLine();
            string posted = sr.ReadLine();
            string title = sr.ReadLine();
            sr.ReadLine();
            string article = sr.ReadToEnd();

            posted = posted.Replace("Posted: ", "");
            text.Posted = DateTime.Parse(posted);
            text.Title = title.Replace("Title: ", "");
            text.Article = article;
            return text;            
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