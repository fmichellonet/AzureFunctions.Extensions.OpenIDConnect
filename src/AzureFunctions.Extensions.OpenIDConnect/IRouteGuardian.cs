using Microsoft.AspNetCore.Authorization;

namespace AzureFunctions.Extensions.OpenIDConnect
{
    public interface IRouteGuardian
    {
        bool IsProtectedRoute(string functionName);
        AuthorizeAttribute GetAuthorizationConfiguration(string functionName);
    }
}