using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AzureFunctions.Extensions.OpenIDConnect.Configuration
{
    using System;
    using System.Collections.Generic;

    public class ConfigurationBuilder
    {
        private readonly IServiceCollection _services;
        private readonly IDictionary<string, AuthorizationPolicy> _policyMap;
        private bool _hasConfigurationManagerSettings;
        private bool _hasTokenValidationParameters;


        internal bool IsValid => _hasTokenValidationParameters && _hasConfigurationManagerSettings;

        internal ConfigurationBuilder(IServiceCollection services)
        {
            _services = services;
            _policyMap = new Dictionary<string, AuthorizationPolicy>(StringComparer.OrdinalIgnoreCase);

            // defaulting
            SetTypeCrawler(RouteGuardian.AppDomainTypeCrawler);
            _services.AddSingleton<IAuthorizationRequirementsRetriever>(new AuthorizationRequirementsRetriever(_policyMap));
            var authorizationHandlers = _policyMap.Values.SelectMany(x => x.Requirements.Cast<IAuthorizationHandler>());
            _services.AddSingleton<IAuthorizationHandlerProvider>(new DefaultAuthorizationHandlerProvider(authorizationHandlers));
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

        public void SetTypeCrawler(Func<IEnumerable<Type>> functionTypeCrawler)
        {
            _services.AddSingleton<FunctionTypeCrawler>(() => functionTypeCrawler());
        }

        public void AddPolicy(string name, Action<AuthorizationPolicyBuilder> configurePolicy)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configurePolicy == null)
            {
                throw new ArgumentNullException(nameof(configurePolicy));
            }

            var policyBuilder = new AuthorizationPolicyBuilder();
            configurePolicy(policyBuilder);
            _policyMap[name] = policyBuilder.Build();
        }
    }
}