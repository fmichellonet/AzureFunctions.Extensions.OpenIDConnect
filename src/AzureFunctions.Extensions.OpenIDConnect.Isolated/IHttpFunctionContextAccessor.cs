using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;

namespace AzureFunctions.Extensions.OpenIDConnect.Isolated;

public interface IHttpFunctionContextAccessor
{
    ValueTask<HttpRequestData> GetHttpRequestDataAsync();

    string FunctionName { get; }

    void SetInvocationResult(HttpResponseData response);
}