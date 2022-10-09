using System.Linq;
using System.Reflection;
using Microsoft.Azure.Functions.Worker;

namespace AzureFunctions.Extensions.OpenIDConnect.Isolated
{
    public class FunctionsAnalyzer : IFunctionsAnalyzer
    {
        public bool IsAzureFunction(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes<FunctionAttribute>().Any();
        }

        public bool IsHttpTrigger(MethodInfo methodInfo)
        {
            return methodInfo.GetParameters()
                .Any(paramInfo => paramInfo.GetCustomAttributes<HttpTriggerAttribute>().Any());
        }

        public string? GetRoute(MethodInfo methodInfo)
        {
            return methodInfo.GetParameters()
                .SelectMany(paramInfo =>paramInfo.GetCustomAttributes<HttpTriggerAttribute>())
                .Select(x => x.Route)
                .First();
        }

        public string GetFunctionName(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes<FunctionAttribute>()
                    .Select(x => x.Name)
                    .First();
        }
    }
}