using System;
using System.Threading.Tasks;
using AzureFunctions.Extensions.OpenIDConnect.Configuration;
using AzureFunctions.Extensions.OpenIDConnect.Isolated.Configuration;
using Microsoft.Extensions.Hosting;

namespace Isolated_Net6;

static class Program
{
    private static async Task Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults((context, builder) =>
            {
                builder.UseAuthorization();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddOpenIDConnect(config =>
                {
                    var audience = Environment.GetEnvironmentVariable("OpenIdConnect_Audience");
                    var issuer = Environment.GetEnvironmentVariable("OpenIdConnect_Issuer");
                    var issuerUrl = Environment.GetEnvironmentVariable("OpenIdConnect_IssuerUrl");

                    config.SetTokenValidation(TokenValidationParametersHelpers.Default(audience, issuer));
                    config.SetIssuerBaseUrlConfiguration(issuerUrl);
                });
            })
            .Build();
        
        await host.RunAsync();
    }
}