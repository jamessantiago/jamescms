using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MarkdownDeep;
using jamescms;
using jamescms.Models;
using System.Web.Routing;

namespace MarkdownDeep
{
    public partial class jamesMarkdown : MarkdownDeep.Markdown
    {
        public RequestContext RequestContext {get; set;}

        public override string OnQualifyUrl(string url)
        {
            if (url.StartsWith("GetPhoto", StringComparison.CurrentCultureIgnoreCase))
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    string photoid = url.ToLower().Replace("getphoto=", "");
                    var photo = uow.pr.GetPhoto(jcms.TextWallPhotoAlbum, photoid, Google.GData.Photos.PicasaQuery.AccessLevel.AccessPublic);
                    return photo.Media.Content.Attributes["url"].ToString();
                }
            }
            else if (url.Contains(','))
            {
                string action = "", controller = "", query = "", finalurl = "";
                var urlParts = url.Split(',');
                if (urlParts.Length == 2)
                {
                    action = urlParts[0];
                    controller = urlParts[1];
                    finalurl = UrlHelper.GenerateUrl("Default", action, controller, null, RouteTable.Routes, RequestContext, false);

                }
                if (urlParts.Length >= 3)
                {
                    action = urlParts[0];
                    controller = urlParts[1];
                    query = "?";
                    for (int i = 2; i < urlParts.Length; i++)
                    {
                        if (urlParts[i].Contains('='))
                        {
                            query += urlParts[i];
                            if (i != (urlParts.Length - 1))
                                query += "&";
                        }
                        else
                            query += "/" + urlParts[i];
                    }
                    finalurl = UrlHelper.GenerateUrl("Default", action, controller, null, RouteTable.Routes, RequestContext, false) + query;
                }
                return finalurl;
            }
            else
                return base.OnQualifyUrl(url);
        }
    }
}