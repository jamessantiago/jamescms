using System.Web;
using System.Web.Mvc;
using jamescms.Filters;

namespace jamescms
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new NLogHandleErrorAttribute());
        }
    }

    public class NLogHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            var logger = NLog.LogManager.GetLogger("Global");
            logger.FatalException(filterContext.Exception.Message, filterContext.Exception);
            base.OnException(filterContext);
        }
    }
}