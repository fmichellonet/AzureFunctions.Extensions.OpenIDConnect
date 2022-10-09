using Microsoft.Azure.Functions.Worker;

namespace AzureFunctions.Extensions.OpenIDConnect.Isolated;

public interface IHttpFunctionContextAccessorFactory
{
    public IHttpFunctionContextAccessor Create(FunctionContext executingContext);
}