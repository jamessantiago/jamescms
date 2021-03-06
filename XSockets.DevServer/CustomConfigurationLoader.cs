using System;
using System.Collections.Generic;
using XSockets.Core.Common.Configuration;

namespace XSockets.DevServer
{
    /// <summary>
    /// Custom config...
    /// Make changes to this class or remove and create your own for the configuration.
    /// Just make sure to implement IConfigurationLoader and Export the interface
    /// </summary>
    
    public class CustomConfigurationLoader : IConfigurationLoader
    {
        public CustomConfigurationLoader()
        {
        }

        public IConfigurationSettings _settings = null;

        public Uri GetUri(string location)
        {
            try
            {
                return new Uri(location);
            }
            catch (Exception)
            {

                return new Uri(string.Format("ws://{0}", location));
            }

        }

        /// <summary>
        /// Get server settings from config file.
        /// </summary>
        public IConfigurationSettings ConfigurationSettings
        {
            get
            {
                if (this._settings == null)
                {
                    var uri = GetUri("ws://localhost");

                    this._settings = new XSockets.Core.Configuration.ConfigurationSettings
                        {
                            Port = 4502,
                            Origin = new List<string>() { "http://localhost:50695" },
                            Location = uri.Host,
                            Scheme = uri.Scheme,
                            Uri = uri,
                            BufferSize = 8192,
                            RemoveInactiveStorageAfterXDays = 7,
                            RemoveInactiveChannelsAfterXMinutes = 30,
                            NumberOfAllowedConections = -1,
                            Endpoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 4502)
                        };
                }
                return this._settings;
            }
        }
    }
}