using System.Security.Claims;
using Microsoft.Azure.Functions.Worker.Http;

namespace AzureFunctions.Extensions.OpenIDConnect.Isolated;

public static class HttpRequestDataExtensions
{
    internal const string UserKey = "OpenIDConnect_principal";
    public static ClaimsPrincipal User(this HttpRequestData httpRequestData)
    {
        if (httpRequestData.FunctionContext.Items.TryGetValue(UserKey, out var user))
        {
            return user as ClaimsPrincipal;
        }
        return null;
    }
}