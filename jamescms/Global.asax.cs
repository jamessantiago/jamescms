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

        #region Application Start

        protected void Application_Start()
        {
            logger.Debug("Registering application settings");
            NLogConfig.RegisterLayouts();
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            BundleTable.EnableOptimizations = true;
            logger.Debug("jamescms has started");
        }

        #endregion Application Start

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            logger.FatalException(exception.Message, exception);

            Response.Clear();

            HttpException httpException = exception as HttpException;

            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");

            if (httpException == null)
            {
                Response.StatusCode = 500;
                routeData.Values.Add("action", "Index");
            }
            else
            {
                var statusCode = httpException.GetHttpCode();
                Response.StatusCode = statusCode;

                if (System.Web.Mvc.AjaxRequestExtensions.IsAjaxRequest(HttpContext.Current.Request.RequestContext.HttpContext.Request))
                {
                    //redirect to ajax
                    routeData.Values.Add("action", "Err");
                }
                else
                {
                    //redirect to page
                    routeData.Values.Add("action", "Err2");
                }
            }

            routeData.Values.Add("error", exception);

            Server.ClearError();

            Response.TrySkipIisCustomErrors = true;

            IController errorController = new Controllers.ErrorController();
            errorController.Execute(new RequestContext(
                 new HttpContextWrapper(Context), routeData));
        }
    }    
}