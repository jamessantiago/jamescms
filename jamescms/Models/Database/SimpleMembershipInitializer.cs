using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using WebMatrix.WebData;
using jamescms.Models;
using System.Web.Security;
using NLog;

namespace jamescms
{
    public class SimpleMembershipInitializer<T> : IDatabaseInitializer<T> where T : DbContext
    {
        private static Logger logger = LogManager.GetLogger("SimpleMembershipInitializer");

        public void InitializeDatabase(T context)
        {
            try
            {
                logger.Debug("Initializing membership database");
                context.Database.CreateIfNotExists();

                //WebSecurity.InitializeDatabaseConnection("AccountConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
            
                if (!Roles.RoleExists("Guides"))
                {
                    Roles.CreateRole("Guides");
                    WebSecurity.CreateUserAndAccount("admin", "password");
                    Roles.AddUserToRole("admin", "Guides");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
            }
        }

        public static void InitializeDatabaseConnection()
        {
            logger.Debug("Initializing websecurity database connection");
            WebSecurity.InitializeDatabaseConnection("AccountConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
        }
    }
}