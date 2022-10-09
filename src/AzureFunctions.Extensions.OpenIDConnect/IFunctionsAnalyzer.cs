using System.Reflection;

namespace AzureFunctions.Extensions.OpenIDConnect
{
    public interface IFunctionsAnalyzer
    {
        bool IsAzureFunction(MethodInfo methodInfo);
        bool IsHttpTrigger(MethodInfo methodInfo);
        string GetRoute(MethodInfo methodInfo);
        string GetFunctionName(MethodInfo methodInfo);
    }
}