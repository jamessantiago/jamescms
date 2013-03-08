using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LibGit2Sharp;
using NLog;

namespace jamescms.Helpers
{
    public static class GitHelper
    {
        private static Logger logger = LogManager.GetLogger("GitHelper");

        public static string GetHeadCommitSha(string RepoPath)
        {
            using (var repo = new Repository(RepoPath))
            {
                var headCommit = repo.Head.Commits.First();
                return headCommit.Sha;
            }
        }

        public static void CommitChanges(string Message, string RepoPath)
        {
            using (var repo = new Repository(RepoPath))
            {
                logger.Debug("Git commit to " + RepoPath);
                Signature sig = new Signature("SantiagoDevelopment", "SantiagoDevelopment@SantiagoDevelopment.com", DateTimeOffset.Now );
                repo.Commit(Message, sig);
            }
        }

        public static void AddUpdateFile(string FileName, string RepoPath)
        {
            using (var repo = new Repository(RepoPath))
            {
                repo.Index.Stage(FileName);
            }
        }
    }
}