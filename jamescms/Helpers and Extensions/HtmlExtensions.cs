using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Razor;
using System.Web.Routing;
using System.Text;
using System.Collections;
using System.Web.WebPages;
using System.ComponentModel;

namespace System.Web.Mvc
{
    public static class HtmlExtensions
    {

        public static MvcHtmlString AjaxButton(this HtmlHelper helper, string buttonText, string target, string action)
        {
            string controller = helper.ViewContext.RouteData.GetRequiredString("controller");
            return AjaxButton(helper, buttonText, target, action, controller, null);
        }

        public static MvcHtmlString AjaxButton(this HtmlHelper helper, string buttonText, string target, string action, object routeValues)
        {
            string controller = helper.ViewContext.RouteData.GetRequiredString("controller");
            return AjaxButton(helper, buttonText, target, action, controller, routeValues);
        }

        public static MvcHtmlString AjaxButton(this HtmlHelper helper, string buttonText, string target, string action, string controller, object routeValues)
        {            
            var anchor = new TagBuilder("input");
            anchor.Attributes["type"] = "button";
            anchor.Attributes["href-to-load"] = UrlHelper.GenerateUrl("Default", action, controller, new RouteValueDictionary(routeValues), RouteTable.Routes, helper.ViewContext.RequestContext, false);
            anchor.Attributes["target"] = "#" + target;
            anchor.Attributes["href-loading"] = UrlHelper.GenerateContentUrl("~/images/loading.gif", helper.ViewContext.HttpContext);
            anchor.Attributes["value"] = buttonText;
            foreach (string css in new string[] {"jLoad", "form-button"})
                anchor.AddCssClass(css);            
            return MvcHtmlString.Create(anchor.ToString());
        }

        public static MvcHtmlString AjaxReadyLoadDiv(this HtmlHelper helper, string id, string action)
        {
            string controller = helper.ViewContext.RouteData.GetRequiredString("controller");
            return AjaxReadyLoadDiv(helper, id, action, controller, null);
        }

        public static MvcHtmlString AjaxReadyLoadDiv(this HtmlHelper helper, string id, string action, object routeValues)
        {
            string controller = helper.ViewContext.RouteData.GetRequiredString("controller");
            return AjaxReadyLoadDiv(helper, id, action, controller, routeValues);
        }

        public static MvcHtmlString AjaxReadyLoadDiv(this HtmlHelper helper, string id, string action, string controller, object routeValues)
        {
            var anchor = new TagBuilder("div");
            anchor.Attributes["id"] = id;
            anchor.Attributes["href-to-load"] = UrlHelper.GenerateUrl("Default", action, controller, new RouteValueDictionary(routeValues), RouteTable.Routes, helper.ViewContext.RequestContext, false);
            anchor.Attributes["href-loading"] = UrlHelper.GenerateContentUrl("~/images/loading.gif", helper.ViewContext.HttpContext);
            anchor.AddCssClass("readyLoad");
            return MvcHtmlString.Create(anchor.ToString());
        }

        public static string ToAttributeList(this object list)
        {
            StringBuilder sb = new StringBuilder();
            if (list != null)
            {
                Hashtable attributeHash = GetPropertyHash(list);
                string resultFormat = "{0}=\"{1}\" ";
                foreach (string attribute in attributeHash.Keys)
                {
                    sb.AppendFormat(resultFormat, attribute.Replace("_", ""),
                        attributeHash[attribute]);
                }
            }
            return sb.ToString();
        }

        public static string ToAttributeList(this object list,
                                             params object[] ignoreList)
        {
            Hashtable attributeHash = GetPropertyHash(list);

            string resultFormat = "{0}=\"{1}\" ";
            StringBuilder sb = new StringBuilder();
            foreach (string attribute in attributeHash.Keys)
            {
                if (!ignoreList.Contains(attribute))
                {
                    sb.AppendFormat(resultFormat, attribute,
                        attributeHash[attribute]);
                }
            }
            return sb.ToString();
        }

        public static Hashtable GetPropertyHash(object properties)
        {
            Hashtable values = null;

            if (properties != null)
            {
                values = new Hashtable();
                PropertyDescriptorCollection props =
                    TypeDescriptor.GetProperties(properties);

                foreach (PropertyDescriptor prop in props)
                {
                    values.Add(prop.Name, prop.GetValue(properties));
                }
            }
            return values;
        }
    }
}