using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Razor;
using System.Web.WebPages;

namespace jamescms.Helpers
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString AjaxLink(this HtmlHelper helper, string linkText, string target, string action, string controller)
        {
            var currentAction = helper.ViewContext.ParentActionViewContext.RouteData.GetRequiredString("action");
            var currentController = helper.ViewContext.RouteData.GetRequiredString("controller");
            if (action == currentAction && controller == currentController)
            {
                var anchor = new TagBuilder("input");
                anchor.Attributes["type"] = "button";
                anchor.Attributes["href"] = "#";
                anchor.Attributes["target"] = "#" + target;
                anchor.AddCssClass("jLoad"); anchor.AddCssClass("form-button");                
                anchor.SetInnerText(linkText);
                return MvcHtmlString.Create(anchor.ToString());
            }
            return helper.ActionLink(linkText, action, controller);
        }
    }
}