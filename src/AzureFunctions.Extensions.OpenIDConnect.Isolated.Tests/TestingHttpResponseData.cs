using System.IO;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace AzureFunctions.Extensions.OpenIDConnect.Isolated.Tests;

public class TestingHttpResponseData : HttpResponseData
{
    public TestingHttpResponseData(FunctionContext functionContext) : base(functionContext)
    {
    }

    public override HttpStatusCode StatusCode { get; set; }
    public override HttpHeadersCollection Headers { get; set; }
    public override Stream Body { get; set; } = new MemoryStream();
    public override HttpCookies Cookies { get; }
}