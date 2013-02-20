using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog.Config;

namespace jamescms
{
    public class NLogConfig
    {
        public static void RegisterLayouts()
        {
            NLog.Config.ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("web_variables", typeof(NLogRenderers.WebVariablesRenderer));
        }
    }
}