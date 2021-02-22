using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace AzureFunctions.Extensions.OpenIDConnect.Tests
{
    public static class ServiceCollectionFixture
    {
        public static ServiceCollection MinimalAzFunctionsServices(ServiceCollection collection = null)
        {
            collection ??= new ServiceCollection();

            collection.AddSingleton<IAuthorizationHandlerContextFactory, DefaultAuthorizationHandlerContextFactory>();
            collection.AddSingleton<IAuthorizationEvaluator, DefaultAuthorizationEvaluator>();

            var authorizationOptions = Substitute.For<IOptions<AuthorizationOptions>>();
            authorizationOptions.Value.Returns(new AuthorizationOptions { InvokeHandlersAfterFailure = false });
            collection.AddSingleton(authorizationOptions);

            var logger = Substitute.For<ILogger<DefaultAuthorizationService>>();
            collection.AddSingleton<ILogger<DefaultAuthorizationService>>(logger);

            return collection;
        }
    }
}