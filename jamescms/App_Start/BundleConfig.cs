﻿using System.Web;
using System.Web.Optimization;

namespace jamescms
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.migrate*",
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/master").Include(
                    "~/Scripts/MarkdownDeepLib.min.js",
                    "~/Scripts/Prettify/prettify.js",
                    "~/Scripts/Prettify/lang-log.js",
                    "~/Scripts/master.js",
                    "~/Scripts/GoogleAnalytics.js"));

            bundles.Add(new ScriptBundle("~/bundles/gallerific").Include(
                    "~/Scripts/jquery.opacityrollover.js",
                    "~/Scripts/jquery.galleriffic.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/triviawebsocketclient").Include(
                    "~/Games/QuizGame/TriviaWebSocketClient.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            bundles.Add(new StyleBundle("~/Content/Initializr").Include(
                        "~/Content/themes/Initializr/normalize.min.css",
                        "~/Content/themes/Initializr/main.css",
                        "~/Scripts/mdd_styles.css",
                        "~/Content/Prettify/Themes/sunburst.css"));
        }
    }
}