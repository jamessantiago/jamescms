using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Configuration;
using System.Data.Entity;
using jamescms.Models;
using NLog;

namespace jamescms
{

    public class jcms : System.Web.HttpApplication
    {
        #region Private Properties

        private Logger logger = LogManager.GetLogger("MvcApplication");
        private static string _facebookAppId { get; set; }
        private static string _facebookAppSecret { get; set; }
        private static string _fullTextEnabled { get; set; }
        private static string _textWallPhotoAlbum { get; set; }

        #endregion Private Properties

        #region Public Properties

        public static string FacebookAppId
        {
            get
            {
                if (_facebookAppId == null) { _facebookAppId = WebConfigurationManager.AppSettings["FacebookAppId"] ?? ""; }
                return _facebookAppId;
            }
        }

        public static string FacebookAppSecret
        {
            get
            {
                if (_facebookAppSecret == null) { _facebookAppSecret = WebConfigurationManager.AppSettings["FacebookAppSecret"] ?? ""; }
                return _facebookAppSecret;
            }
        }

        public static bool FullTextEnabled
        {
            get
            {
                if (_fullTextEnabled == null)
                {
                    bool enabled = false;
                    bool.TryParse(WebConfigurationManager.AppSettings["FullTextEnabled"], out enabled);
                    _fullTextEnabled = enabled.ToString();
                }
                return bool.Parse(_fullTextEnabled);
            }
        }

        public static string TextWallPhotoAlbum
        {
            get
            {
                if (_textWallPhotoAlbum == null) { _textWallPhotoAlbum = WebConfigurationManager.AppSettings["TextWallPhotoAlbum"] ?? ""; }
                return _textWallPhotoAlbum;
            }
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
            BundleTable.EnableOptimizations = true;
            AuthConfig.RegisterAuth();
            if (FullTextEnabled)
            {
                Database.SetInitializer<TextContext>(
                    new CompositeDatabaseInitializer<TextContext>(
                        new IndexInitializer<TextContext>(),
                        new FullTextIndexInitializer<TextContext>()));
            }
            else
            {
                Database.SetInitializer<TextContext>(
                    new IndexInitializer<TextContext>()
                    );
            }
            logger.Debug("jamescms has started");
        }

        #endregion Application Start

        #region Application Error

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            logger.FatalException(exception.Message, exception);

            Response.Clear();

            HttpException httpException = exception as HttpException;

            RouteData routeData = new RouteData();
            bool isAjax = false;

            if (httpException == null)
            {
                Response.StatusCode = 500;
                routeData.Values.Add("action", "Index");
            }
            else
            {
                //TODO: perform some security actions here

                var statusCode = httpException.GetHttpCode();
                Response.StatusCode = statusCode;

                if (System.Web.Mvc.AjaxRequestExtensions.IsAjaxRequest(HttpContext.Current.Request.RequestContext.HttpContext.Request))
                {
                    //redirect to ajax
                    isAjax = true;
                    routeData.Values.Add("action", "Ajax");
                }
                else
                {
                    //redirect to page
                    routeData.Values.Add("action", "Index");
                }
            }

            routeData.Values.Add("error", exception);

            Server.ClearError();

            Response.TrySkipIisCustomErrors = true;

            //if url is the frontpage or if ajax is used redirect to error controller
            if (System.Text.RegularExpressions.Regex.Match(Request.RawUrl, @"\.com/?(\/Home/?(Index)?)?$", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Success
                || isAjax)
            {
                routeData.Values.Add("controller", "Error");
                IController errorController = new Controllers.errorController();
                errorController.Execute(new RequestContext(
                     new HttpContextWrapper(Context), routeData));
            }
            else
            {
                routeData.Values.Add("controller", "sd");
                IController sdController = new Controllers.sdController();
                sdController.Execute(new RequestContext(
                     new HttpContextWrapper(Context), routeData));
            }
        }

        #endregion Application Error
    }
}