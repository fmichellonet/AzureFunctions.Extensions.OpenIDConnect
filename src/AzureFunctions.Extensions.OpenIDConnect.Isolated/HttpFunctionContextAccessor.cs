using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace AzureFunctions.Extensions.OpenIDConnect.Isolated;

public class HttpFunctionContextAccessor : IHttpFunctionContextAccessor
{
    private readonly FunctionContext _executingContext;

    public HttpFunctionContextAccessor(FunctionContext executingContext)
    {
        _executingContext = executingContext;
    }

    public ValueTask<HttpRequestData> GetHttpRequestDataAsync()
    {
        return _executingContext.GetHttpRequestDataAsync();
    }

    public string FunctionName => _executingContext.FunctionDefinition.Name;

    public void SetInvocationResult(HttpResponseData response)
    {
        _executingContext.GetInvocationResult().Value = response;
    }
}