using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Razor;
using System.Web.WebPages;

namespace System.Web.Mvc
{
    public static class HtmlExtensions
    {

        public static MvcHtmlString AjaxButton(this HtmlHelper helper, string linkText, string target, string action)
        {
            string controller = helper.ViewContext.RouteData.GetRequiredString("controller");
            return AjaxButton(helper, linkText, target, action, controller);
        }

        public static MvcHtmlString AjaxButton(this HtmlHelper helper, string linkText, string target, string action, string controller)
        {            
            var anchor = new TagBuilder("input");
            anchor.Attributes["type"] = "button";
            string url = UrlHelper.GenerateUrl("Default", action, controller, null, null, null, false);
            anchor.Attributes["href-to-load"] = url;
            anchor.Attributes["target"] = "#" + target;
            foreach (string css in new string[] {"jLoad", "form-button"})
                anchor.AddCssClass(css);
            anchor.SetInnerText(linkText);
            return MvcHtmlString.Create(anchor.ToString());
        }
    }
}