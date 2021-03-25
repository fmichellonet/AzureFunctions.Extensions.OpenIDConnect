using System;
using Microsoft.AspNetCore.Authorization;

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
        private readonly IAuthorizationService _authorizationService;
        private readonly IAuthorizationRequirementsRetriever _requirementsRetriever;

        public AuthorizeFilter(IHttpContextAccessor httpContextAccessor, 
            IAuthenticationService authenticationService, IRouteGuardian routeGuardian,
            IAuthorizationService authorizationService, IAuthorizationRequirementsRetriever requirementsRetriever)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationService = authenticationService;
            _routeGuardian = routeGuardian;
            _authorizationService = authorizationService;
            _requirementsRetriever = requirementsRetriever;
        }

        public override async Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        {
            if (_routeGuardian.IsProtectedRoute(executingContext.FunctionName))
            {
                var httpContext = _httpContextAccessor.HttpContext;

                // Authenticate the user
                var authenticationResult = await _authenticationService.AuthenticateAsync(httpContext.Request.Headers);
                
                if (authenticationResult.Failed)
                {
                    await Unauthorized(httpContext, cancellationToken);
                    return;
                }

                httpContext.User = authenticationResult.User;

                var attribute = _routeGuardian.GetAuthorizationConfiguration(executingContext.FunctionName);
                var requirements = _requirementsRetriever.ForAttribute(attribute);

                if (requirements != null)
                {
                    var authorizationResult = await _authorizationService.AuthorizeAsync(httpContext.User, null, requirements);
                    if (!authorizationResult.Succeeded)
                    {
                        await Forbidden(httpContext, authorizationResult.Failure, cancellationToken);
                    }
                }
                
            }
            await base.OnExecutingAsync(executingContext, cancellationToken);
        }

        private async Task Unauthorized(HttpContext httpContext, CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await httpContext.Response.WriteAsync(string.Empty, cancellationToken);
            throw new UnauthorizedAccessException();
        }

        private async Task Forbidden(HttpContext httpContext, AuthorizationFailure failure, CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await httpContext.Response.WriteAsync(failure.FailedRequirements.ToString(), cancellationToken);
            throw new UnauthorizedAccessException();
        }
    }
}