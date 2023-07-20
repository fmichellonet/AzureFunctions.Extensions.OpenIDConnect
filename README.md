# AzureFunctions.Extensions.OpenIDConnect
[![Build status](https://github.com/fmichellonet/AzureFunctions.Extensions.OpenIDConnect/workflows/Continuous%20Integration/badge.svg)](https://github.com/fmichellonet/AzureFunctions.Extensions.OpenIDConnect/actions?query=workflow%3A%22Continuous+Integration%22) [![NuGet Badge](https://buildstats.info/nuget/AzureFunctions.Extensions.OpenIDConnect)](https://www.nuget.org/packages/AzureFunctions.Extensions.OpenIDConnect/)

> This project is originally forked from https://github.com/AspNetMonsters/AzureFunctions.OidcAuthentication. Thanks goes to [David Paquette](https://github.com/dpaquette) for the helpful intial codebase.


## Why?
As of writing this, securing Azure Functions using Bearer token is clumsy. For some auth providers, you can enable App Service Authentication in the Azure Portal but that only works for the deployed version of your app which makes testing locally difficult and clumsy.

This library makes it easy to authenticate a user by validating a bearer token.

## Requirements

Azure Functions using v3 or V4 runtime and of course an identity provider (e.g. Google, Azure AD, etc..)

## How to use it

AzureFunctions.Extensions.OpenIDConnect support [In-Process and Isolated](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide#differences-with-net-class-library-functions) azure functions hosting
and brings the same features set. Be sure to use the nuget package according to your hosting choice.

### In process

Add AzureFunctions.Extensions.OpenIDConnect.InProcess NuGet package to your Azure Functions project.

> dotnet package install AzureFunctions.Extensions.OpenIDConnect.InProcess

Now head over the Configure method of the Startup class, add configure OpenID-Connect the way you like it.

```csharp
namespace MySecuredApp
{
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
}
```

### Isolated

Add AzureFunctions.Extensions.OpenIDConnect.Isolated NuGet package to your Azure Functions project.

> dotnet package install AzureFunctions.Extensions.OpenIDConnect.Isolated

Now head over your entrypoint class, add configure the host like this :

```csharp
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
```

## Securing an Azure Function
Now that everything is configured, you can decorate your http-triggered functions with the well known [Authorize](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authorization.authorizeattribute?view=aspnetcore-3.1) attribute as follows:

```csharp
namespace MySecuredApp
{
    public class MyFunction
    {
        [Authorize]
        [FunctionName("MyFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string responseMessage = $"Hello. This HTTP triggered function is protected.";

            return new OkObjectResult(responseMessage);
        }
   }
}
```
