using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Configuration;
using NLog;

namespace jamescms
{

    public class jcms: System.Web.HttpApplication
    {
        #region Private Properties

        private Logger logger = LogManager.GetLogger("MvcApplication");
        private static string _facebookAppId { get; set; }
        private static string _facebookAppSecret { get; set; }

        #endregion Private Properties

        #region Public Properties

        public static string FacebookAppId { 
            get { if (_facebookAppId == null) { _facebookAppId = WebConfigurationManager.AppSettings["FacebookAppId"] ?? ""; }
                  return _facebookAppId;}
        }

        public static string FacebookAppSecret
        { 
            get { if (_facebookAppSecret == null) { _facebookAppSecret = WebConfigurationManager.AppSettings["FacebookAppSecret"] ?? ""; }
                  return _facebookAppSecret;}
        }

        #endregion Public Properties

        protected void Application_Start()
        {            
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            logger.Info("jamescms has started");
            BundleTable.EnableOptimizations = true;
        }
    }    
}