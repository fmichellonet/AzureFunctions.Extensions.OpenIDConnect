using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AzureFunctions.Extensions.OpenIDConnect.Configuration
{

    public class ConfigurationBuilder
    {
        private readonly IServiceCollection _services;
        private bool _hasConfigurationManagerSettings;
        private bool _hasTokenValidationParameters;

        internal bool IsValid => _hasTokenValidationParameters && _hasConfigurationManagerSettings;

        internal ConfigurationBuilder(IServiceCollection services)
        {
            _services = services;

            // register default services.
            _services.AddSingleton<IFunctionsTypeCrawler, AppDomainFunctionsTypeCrawler>();
        }

        public void SetTokenValidation(string audience, string issuer)
        {
            SetTokenValidation(TokenValidationParametersHelpers.Default(audience, issuer));
        }

        public void SetTokenValidation(TokenValidationParameters settings)
        {
            _services.AddSingleton(settings);
            _hasTokenValidationParameters = true;
        }
        
        public void SetIssuerBaseUrlConfiguration(string issuerUrl)
        {
            SetConfigurationManagerSettings(ConfigurationManagerSettings.FromIssuerBaseAddress(issuerUrl));
        }

        public void SetIssuerMetadataConfigurationEndpoint(string metadataUrl)
        {
            SetConfigurationManagerSettings(ConfigurationManagerSettings.WithSpecificConfigurationEndpoint(metadataUrl));
        }

        private void SetConfigurationManagerSettings(ConfigurationManagerSettings settings)
        {
            _services.AddSingleton(settings);
            _hasConfigurationManagerSettings = true;
        }
    }
}