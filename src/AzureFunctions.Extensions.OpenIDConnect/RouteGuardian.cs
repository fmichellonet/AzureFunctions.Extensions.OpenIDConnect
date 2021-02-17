namespace AzureFunctions.Extensions.OpenIDConnect
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Azure.WebJobs;

    public interface IFunctionsTypeCrawler
    {
        IEnumerable<Type> GetTypes();
    }

    public class AppDomainFunctionsTypeCrawler : IFunctionsTypeCrawler
    {
        public IEnumerable<Type> GetTypes()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic)
                .SelectMany(x => x.GetTypes());
        }
    }

    public class RouteGuardian : IRouteGuardian
    {

        private readonly Dictionary<string, AuthorizeAttribute> _routeProtection;
        
        public RouteGuardian(IFunctionsTypeCrawler typeCrawler)
        {
            var httpTriggerMethods = typeCrawler.GetTypes().SelectMany(type => type.GetMethods()
                .Where(
                    methodInfo => methodInfo.IsPublic && methodInfo.GetCustomAttributes<FunctionNameAttribute>().Any() &&
                    methodInfo.GetParameters().Any(paramInfo => paramInfo.GetCustomAttributes<HttpTriggerAttribute>().Any())
                )
            );

            var infos = httpTriggerMethods.Select(methodInfo =>
            {
                var httpTriggerAttribute = methodInfo.GetParameters()
                                                     .SelectMany(paramInfo =>paramInfo.GetCustomAttributes<HttpTriggerAttribute>())
                                                     .First();
                
                var functionNameAttribute = methodInfo.GetCustomAttributes<FunctionNameAttribute>().First();

                var authorizeAttribute = methodInfo.GetCustomAttributes<AuthorizeAttribute>().FirstOrDefault();

                return new AzureFunctionInfo
                {
                    FunctionName = functionNameAttribute.Name,
                    AuthorizeAttribute = authorizeAttribute,
                    Route = httpTriggerAttribute.Route
                };
            });

            _routeProtection = infos.Where(x => x.AuthorizeAttribute != null)
                                    .ToDictionary(x => x.FunctionName, x => x.AuthorizeAttribute);
        }

        public Task<bool> ShouldAuthorize(string functionName)
        {
            return Task.FromResult(_routeProtection.ContainsKey(functionName));
        }

        private class AzureFunctionInfo
        {
            public string FunctionName { get; set; }
            public AuthorizeAttribute AuthorizeAttribute { get; set; }
            public string Route { get; set; }
        }
    }
}