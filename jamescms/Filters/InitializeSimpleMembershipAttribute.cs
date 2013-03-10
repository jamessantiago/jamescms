using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using WebMatrix.WebData;
using jamescms.Models;
using System.Web.Security;

namespace jamescms.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;
        private static NLog.Logger logger = NLog.LogManager.GetLogger("InitializeSimpleMembershipAttribute");

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Ensure ASP.NET Simple Membership is initialized only once per app start            
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                lock (_initializerLock)
                {
                    logger.Debug("Initializing membership database");
                    Database.SetInitializer<UsersContext>(null);

                    try
                    {
                        using (var context = new UsersContext())
                        {
                            if (!context.Database.Exists())
                            {
                                // Create the SimpleMembership database without Entity Framework migration schema
                                ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                                logger.Debug("Created new membership database");

                            }
                        }

                        WebSecurity.InitializeDatabaseConnection("AccountConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
                        logger.Debug("Membership database connection initialized");
                        if (!Roles.RoleExists("Guides"))
                        {
                            Roles.CreateRole("Guides");
                            WebSecurity.CreateUserAndAccount("admin", "password");
                            Roles.AddUserToRole("admin", "Guides");
                            logger.Debug("Created default admin account");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                    }
                }
            }
        }
    }
}
