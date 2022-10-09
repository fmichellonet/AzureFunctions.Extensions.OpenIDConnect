using System.Linq;
using Microsoft.Azure.WebJobs;
using System.Reflection;

namespace AzureFunctions.Extensions.OpenIDConnect.InProcess
{
    public class FunctionsAnalyzer : IFunctionsAnalyzer
    {
        public bool IsAzureFunction(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes<FunctionNameAttribute>().Any();
        }

        public bool IsHttpTrigger(MethodInfo methodInfo)
        {
            return methodInfo.GetParameters()
                .Any(paramInfo => paramInfo.GetCustomAttributes<HttpTriggerAttribute>().Any());
        }

        public string GetRoute(MethodInfo methodInfo)
        {
            return methodInfo.GetParameters()
                .SelectMany(paramInfo => paramInfo.GetCustomAttributes<HttpTriggerAttribute>())
                .Select(x => x.Route)
                .First();
        }

        public string GetFunctionName(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes<FunctionNameAttribute>()
                .Select(x => x.Name)
                .First();
        }
    }
}