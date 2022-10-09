using Microsoft.Azure.Functions.Worker;

namespace AzureFunctions.Extensions.OpenIDConnect.Isolated;

public class HttpFunctionContextAccessorFactory : IHttpFunctionContextAccessorFactory
{
    public IHttpFunctionContextAccessor Create(FunctionContext executingContext)
    {
        return new HttpFunctionContextAccessor(executingContext);
    }
}