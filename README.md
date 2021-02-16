# AzureFunctions.Extensions.OpenIDConnect
[![Build status](https://github.com/fmichellonet/AzureFunctions.Extensions.OpenIDConnect/workflows/Continuous%20Integration/badge.svg)](https://github.com/fmichellonet/AzureFunctions.Extensions.OpenIDConnect/actions?query=workflow%3A%22Continuous+Integration%22) [![NuGet Badge](https://buildstats.info/nuget/AzureFunctions.Extensions.OpenIDConnect)](https://www.nuget.org/packages/AzureFunctions.Extensions.OpenIDConnect/)

> This project is originally forked from https://github.com/AspNetMonsters/AzureFunctions.OidcAuthentication. Thanks goes to [David Paquette](https://github.com/dpaquette) for the helpful intial codebase.


## Why?
As of writing this, securing Azure Functions using Bearer token is clumsy. For some auth providers, you can enable App Service Authentication in the Azure Portal but that only works for the deployed version of your app which makes testing locally difficult and clumsy.

This library makes it easy to authenticate a user by validating a bearer token.

## Requirements

Azure Functions using v3 runtime and of course an identity provider (e.g. Google, Azure AD, etc..)

## How to use it

Add AzureFunctions.Extensions.OpenIDConnect NuGet package to your Azure Functions project.

> dotnet package install AzureFunctions.Extensions.OpenIDConnect

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

### Securing an Azure Function
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
            string responseMessage = $"Hello, {name}. This HTTP triggered function is protected.";

            return new OkObjectResult(responseMessage);
        }
   }
}
```
