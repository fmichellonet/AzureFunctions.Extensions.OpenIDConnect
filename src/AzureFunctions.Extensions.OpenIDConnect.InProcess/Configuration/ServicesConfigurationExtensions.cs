using System;
using AzureFunctions.Extensions.OpenIDConnect.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFunctions.Extensions.OpenIDConnect.InProcess.Configuration
{
    public static class ServicesConfigurationExtensions
    {
        public static void AddOpenIDConnect(this IServiceCollection services, string issuer, string audience)
        {
            RegisterServices(services);
            ServicesConfigurationExtensionsBase.AddOpenIDConnect(services, issuer, audience);
        }

        public static void AddOpenIDConnect(this IServiceCollection services, Action<ConfigurationBuilder> configurator)
        {
            RegisterServices(services);
            ServicesConfigurationExtensionsBase.AddOpenIDConnect(services, configurator);
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IAuthorizationService, DefaultAuthorizationService>();
            services.AddSingleton<IAuthorizationPolicyProvider, DefaultAuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandlerProvider, DefaultAuthorizationHandlerProvider>();
            services.AddSingleton<IFunctionsAnalyzer, FunctionsAnalyzer>();
            services.AddSingleton<IFunctionFilter, AuthorizeFilter>();
        }
    }
}