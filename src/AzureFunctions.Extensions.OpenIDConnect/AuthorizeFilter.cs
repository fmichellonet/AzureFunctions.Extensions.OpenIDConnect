namespace AzureFunctions.Extensions.OpenIDConnect
{
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs.Host;

    public class AuthorizeFilter : FunctionInvocationFilterAttribute
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticationService _authenticationService;
        private readonly IRouteGuardian _routeGuardian;

        public AuthorizeFilter(IHttpContextAccessor httpContextAccessor, IAuthenticationService authenticationService, IRouteGuardian routeGuardian)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationService = authenticationService;
            _routeGuardian = routeGuardian;
        }

        public override async Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        {
            if (await _routeGuardian.ShouldAuthorize(executingContext.FunctionName))
            {
                var httpContext = _httpContextAccessor.HttpContext;

                // Authenticate the user
                var authResult = await _authenticationService.AuthenticateAsync(httpContext.Request.Headers);

                if (authResult.Failed)
                {
                    await Unauthorized(httpContext, cancellationToken);
                    return;
                }
            }
            await base.OnExecutingAsync(executingContext, cancellationToken);
        }

        private async Task Unauthorized(HttpContext httpContext, CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await httpContext.Response.WriteAsync(string.Empty, cancellationToken);
        }
    }
}