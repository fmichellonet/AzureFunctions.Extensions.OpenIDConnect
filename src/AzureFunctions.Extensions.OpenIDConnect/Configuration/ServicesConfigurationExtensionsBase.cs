using System;
using System.Runtime.CompilerServices;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;

[assembly: InternalsVisibleToAttribute("AzureFunctions.Extensions.OpenIDConnect.Isolated")]
[assembly: InternalsVisibleToAttribute("AzureFunctions.Extensions.OpenIDConnect.InProcess")]
namespace AzureFunctions.Extensions.OpenIDConnect.Configuration
{
    internal static class ServicesConfigurationExtensionsBase
    {
        internal static void AddOpenIDConnect(this IServiceCollection services, string issuer, string audience)
        {
            Action<ConfigurationBuilder> configurator = builder =>
            {
                builder.SetTokenValidation(TokenValidationParametersHelpers.Default(audience, issuer));

                builder.SetIssuerBaseUrlConfiguration(issuer);
            };

            AddOpenIDConnect(services, configurator);
        }

        internal static void AddOpenIDConnect(this IServiceCollection services, Action<ConfigurationBuilder> configurator)
        {
            if (configurator == null)
            {
                throw new ArgumentNullException(nameof(configurator));
            }

            var builder = new ConfigurationBuilder(services);
            configurator(builder);

            if (!builder.IsValid)
            {
                throw new ArgumentException("Be sure to configure Token Validation and Configuration Manager (for example issuer url)");
            }

            // These are created as a singletons, so that only one instance of each
            // is created for the lifetime of the hosting Azure Function App.
            // That helps reduce the number of calls to the authorization service
            // for the signing keys and other stuff that can be used across multiple
            // calls to the HTTP triggered Azure Functions.

            
            services.AddSingleton<IAuthorizationHeaderBearerTokenExtractor, AuthorizationHeaderBearerTokenExtractor>();
            services.AddSingleton<IJwtSecurityTokenHandlerWrapper, JwtSecurityTokenHandlerWrapper>();
            services.AddSingleton<IOpenIdConnectConfigurationManager, OpenIdConnectConfigurationManager>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IRouteGuardian, RouteGuardian>();
            
            services.AddHttpContextAccessor();
            services.AddSingleton<IAuthorizationService, DefaultAuthorizationService>();
            services.AddSingleton<IAuthorizationPolicyProvider, DefaultAuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandlerProvider, DefaultAuthorizationHandlerProvider>();
        }
    }
}