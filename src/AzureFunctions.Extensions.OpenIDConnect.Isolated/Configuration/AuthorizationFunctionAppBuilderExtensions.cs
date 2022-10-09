using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;

namespace AzureFunctions.Extensions.OpenIDConnect.Isolated.Configuration
{
    public static class AuthorizationFunctionAppBuilderExtensions
    {
        public static IFunctionsWorkerApplicationBuilder UseAuthorization(this IFunctionsWorkerApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            app.UseMiddleware<AuthorizationFunctionMiddleware>();

            return app;
        }
    }
}