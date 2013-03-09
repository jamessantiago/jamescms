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

        private static string FilePath { get { return Path.Combine("D:\\", "TextFiles"); } }
        private static string RepoPath { get { return Path.Combine(FilePath, ".git"); } }

        public static void PushTextFiles()
        {
            try
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
                            GitHelper.AddUpdateFile(textPath, repoPath);
                        }
                        GitHelper.CommitChanges("Changes from website", repoPath);
                        string headSha = GitHelper.GetHeadCommitSha(repoPath);
                        uow.tc.Settings.First().HeadSha = headSha;
                        uow.tc.SaveChanges();
                        logger.Debug("Git head is now at " + headSha);
                    }
                }
                else
                    logger.Debug("Missing TextFiles path or git repo");
            }
            catch (Exception ex)
            {
                logger.ErrorException("Failed to push texts to file", ex);
            }
        }

        public static void PullTextFiles()
        {
            try
            {
                string filePath = FilePath;
                string repoPath = RepoPath;
                if (Directory.Exists(filePath) && Directory.Exists(repoPath))
                {
                    logger.Debug("Pulling texts from " + filePath);
                    using (UnitOfWork uow = new UnitOfWork())
                    {
                        string headSha = GitHelper.GetHeadCommitSha(repoPath);
                        if (uow.tc.Settings.First().HeadSha != headSha)
                        {
                            foreach (var file in Directory.GetFiles(filePath))
                            {
                                try
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
                                        logger.Debug("Added " + text.Title);
                                    }
                                    else
                                    {
                                        Text newText = new Text()
                                        {
                                            Article = model.Article,
                                            Posted = model.Posted,
                                            Title = model.Title,
                                            UrlTitle = urlTitle,
                                            Updated = DateTime.Now
                                        };
                                        uow.tc.Texts.Add(newText);
                                        logger.Debug("Updated " + newText.Title);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.ErrorException("Failed to update text file " + file, ex);
                                }
                            }
                            uow.tc.Settings.First().HeadSha = headSha;
                            uow.tc.SaveChanges();
                            logger.Debug("Successfully completed pull");
                        }
                        else
                            logger.Debug("HEAD is same as last pull");
                    }
                }
                else
                    logger.Debug("Missing TextFiles path or git repo");
            }
            catch (Exception ex)
            {
                logger.ErrorException("Failed to update text files", ex);
            }
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