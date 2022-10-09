using AzureFunctions.Extensions.OpenIDConnect.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AzureFunctions.Extensions.OpenIDConnect.Isolated.Configuration;

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
        services.AddSingleton<IAuthorizationHandlerContextFactory, DefaultAuthorizationHandlerContextFactory>();
        services.AddSingleton<IAuthorizationEvaluator, DefaultAuthorizationEvaluator>();
        services.AddSingleton<IFunctionsAnalyzer, FunctionsAnalyzer>();
        services.AddSingleton<IHttpFunctionContextAccessorFactory, HttpFunctionContextAccessorFactory>();
    }
}