using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.WebPages.OAuth;
using jamescms.Models;

namespace jamescms
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            //OAuthWebSecurity.RegisterTwitterClient(
            //    consumerKey: "",
            //    consumerSecret: "");

            //OAuthWebSecurity.RegisterLinkedInClient(
            //    consumerKey: "",
            //    consumerSecret: "");

            OAuthWebSecurity.RegisterFacebookClient(
                appId: jcms.FacebookAppId,
                appSecret: jcms.FacebookAppSecret);

            OAuthWebSecurity.RegisterGoogleClient();

            OAuthWebSecurity.RegisterYahooClient();

        }
    }
}
