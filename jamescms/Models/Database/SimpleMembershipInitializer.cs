using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using WebMatrix.WebData;
using jamescms.Models;
using System.Web.Security;

namespace jamescms
{
    public class SimpleMembershipInitializer<T> : IDatabaseInitializer<T> where T : DbContext
    {

        public void InitializeDatabase(T context)
        {
            try
            {
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
            WebSecurity.InitializeDatabaseConnection("AccountConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
        }
    }
}