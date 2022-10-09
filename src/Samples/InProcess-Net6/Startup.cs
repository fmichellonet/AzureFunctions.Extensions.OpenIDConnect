using System;
using AzureFunctions.Extensions.OpenIDConnect.Configuration;
using AzureFunctions.Extensions.OpenIDConnect.InProcess.Configuration;
using InProcess_Net6;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace InProcess_Net6;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var audience = Environment.GetEnvironmentVariable("OpenIdConnect_Audience");
        var issuer = Environment.GetEnvironmentVariable("OpenIdConnect_Issuer");
        var issuerUrl = Environment.GetEnvironmentVariable("OpenIdConnect_IssuerUrl");
        builder.Services.AddOpenIDConnect(config =>
        {
            config.SetTokenValidation(TokenValidationParametersHelpers.Default(audience, issuer));
            config.SetIssuerBaseUrlConfiguration(issuerUrl);
        });
    }
}