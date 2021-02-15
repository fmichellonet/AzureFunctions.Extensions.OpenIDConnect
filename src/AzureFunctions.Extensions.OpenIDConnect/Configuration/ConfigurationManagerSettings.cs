using System;

namespace AzureFunctions.Extensions.OpenIDConnect.Configuration
{
    internal class ConfigurationManagerSettings
    {
        public static string DefaultOpenidConfigurationEndPoint = ".well-known/openid-configuration";

        public Uri MetadataAddress { get; }

        private ConfigurationManagerSettings(string metadataAddress)
        {
            if(string.IsNullOrEmpty(metadataAddress))
            {
                throw new ArgumentNullException(nameof(metadataAddress));
            }

            MetadataAddress = new Uri(metadataAddress);
        }

        public static ConfigurationManagerSettings FromIssuerBaseAddress(string baseUrl)
        {
            return new ConfigurationManagerSettings($"{baseUrl}{DefaultOpenidConfigurationEndPoint}");
        }

        public static ConfigurationManagerSettings WithSpecificConfigurationEndpoint(string metadataEndpointUrl)
        {
            return new ConfigurationManagerSettings(metadataEndpointUrl);
        }
    }
}
