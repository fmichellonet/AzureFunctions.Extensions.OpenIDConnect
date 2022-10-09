using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace AzureFunctions.Extensions.OpenIDConnect.Isolated.Tests;

public class TestingHttpRequestData : HttpRequestData
{
    private readonly FunctionContext _functionContext;

    private TestingHttpRequestData(FunctionContext functionContext,
        HttpHeadersCollection headers) : base(functionContext)
    {
        _functionContext = functionContext;
        Headers = headers;
    }

    public override Stream Body { get; }
    public override HttpHeadersCollection Headers { get; }
    public override IReadOnlyCollection<IHttpCookie> Cookies { get; }
    public override Uri Url { get; }
    public override IEnumerable<ClaimsIdentity> Identities { get; }
    public override string Method { get; }
    public override HttpResponseData CreateResponse()
    {
        return new TestingHttpResponseData(_functionContext);
    }

    public static HttpRequestData For(FunctionContext functionContext, HttpHeadersCollection headers = null)
    {
        return new TestingHttpRequestData(functionContext,
            headers ?? new HttpHeadersCollection());
    }
}