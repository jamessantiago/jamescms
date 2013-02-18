using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace jamescms
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "sd", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute("Fallback", "{controller}/{action}", new { controller = "Fallback", action = "Init" }, new[] { "XSockets.Longpolling" });
        }
    }
}