namespace AzureFunctions.Extensions.OpenIDConnect
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.Authorization;

    public delegate IEnumerable<Type> FunctionTypeCrawler();

    public class RouteGuardian : IRouteGuardian
    {
        private readonly Dictionary<string, AuthorizeAttribute> _routeProtection;

        public static readonly Func<IEnumerable<Type>> AppDomainTypeCrawler = () =>
        {
            return AppDomain.CurrentDomain
                            .GetAssemblies()
                            .Where(a => !a.IsDynamic)
                            .SelectMany(x => x.GetTypes());
        };

        public RouteGuardian(FunctionTypeCrawler typeCrawler, IFunctionsAnalyzer functionsAnalyzer)
        {
            var httpTriggerMethods = typeCrawler().SelectMany(type => type.GetMethods())
                .Where(methodInfo => methodInfo.IsPublic && functionsAnalyzer.IsAzureFunction(methodInfo) && functionsAnalyzer.IsHttpTrigger(methodInfo));

            var infos = httpTriggerMethods.Select(methodInfo =>
            {
                
                var authorizeAttributeOnType = methodInfo.DeclaringType?.GetCustomAttributes<AuthorizeAttribute>().FirstOrDefault();
                var authorizeAttributeOnMethod = methodInfo.GetCustomAttributes<AuthorizeAttribute>().FirstOrDefault();
                var anonymousAttributeOnMethod = methodInfo.GetCustomAttributes<AllowAnonymousAttribute>().FirstOrDefault();
                
                return new
                {
                    FunctionName = functionsAnalyzer.GetFunctionName(methodInfo),
                    AuthorizeAttribute = anonymousAttributeOnMethod != null ? null : authorizeAttributeOnMethod ?? authorizeAttributeOnType,
                    Route = functionsAnalyzer.GetRoute(methodInfo)
                };
            });

            _routeProtection = infos.Where(x => x.AuthorizeAttribute != null)
                                    .ToDictionary(x => x.FunctionName, x => x.AuthorizeAttribute);
        }

        public bool IsProtectedRoute(string functionName)
        {
            return _routeProtection.ContainsKey(functionName);
        }

        public AuthorizeAttribute GetAuthorizationConfiguration(string functionName)
        {
            return _routeProtection[functionName];
        }
    }
}