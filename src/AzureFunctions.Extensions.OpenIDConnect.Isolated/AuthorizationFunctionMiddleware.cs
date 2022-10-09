using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Primitives;

namespace AzureFunctions.Extensions.OpenIDConnect.Isolated
{
    public class AuthorizationFunctionMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IRouteGuardian _routeGuardian;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAuthorizationRequirementsRetriever _requirementsRetriever;
        private readonly IHttpFunctionContextAccessorFactory _httpFunctionContextAccessorFactory;

        public AuthorizationFunctionMiddleware(IAuthenticationService authenticationService, 
            IRouteGuardian routeGuardian, IAuthorizationService authorizationService, 
            IAuthorizationRequirementsRetriever requirementsRetriever,
            IHttpFunctionContextAccessorFactory httpFunctionContextAccessorFactory)
        {
            _authenticationService = authenticationService;
            _routeGuardian = routeGuardian;
            _authorizationService = authorizationService;
            _requirementsRetriever = requirementsRetriever;
            _httpFunctionContextAccessorFactory = httpFunctionContextAccessorFactory;
        }

        public async Task Invoke(FunctionContext executingContext, FunctionExecutionDelegate next)
        {
            if (_routeGuardian.IsProtectedRoute(executingContext.FunctionDefinition.Name))
            {
                var functionContextAccessor = _httpFunctionContextAccessorFactory.Create(executingContext);
                var requestData =  await functionContextAccessor.GetHttpRequestDataAsync();

                var headers = new HeaderDictionary(requestData.Headers.ToDictionary(x => x.Key, x => new StringValues(x.Value.ToArray())));

                // Authenticate the user
                var authenticationResult = await _authenticationService.AuthenticateAsync(headers);

                if (authenticationResult.Failed)
                {
                    Unauthorized(functionContextAccessor, requestData);
                    return;
                }
            
                var attribute = _routeGuardian.GetAuthorizationConfiguration(executingContext.FunctionDefinition.Name);
                var requirements = _requirementsRetriever.ForAttribute(attribute);

                if (requirements != null)
                {
                    var authorizationResult = await _authorizationService.AuthorizeAsync(authenticationResult.User, null, requirements);
                    if (!authorizationResult.Succeeded)
                    {
                        Forbidden(functionContextAccessor, requestData, authorizationResult.Failure);
                        return;
                    }
                }

            }

            await next(executingContext);
        }

        private static void Unauthorized(IHttpFunctionContextAccessor executingContext,
            HttpRequestData httpRequestData)
        {
            var response = HttpResponseData.CreateResponse(httpRequestData);
            response.StatusCode = HttpStatusCode.Unauthorized;
            executingContext.SetInvocationResult(response);
        }

        private static void Forbidden(IHttpFunctionContextAccessor executingContext, 
            HttpRequestData httpRequestData, AuthorizationFailure failure)
        {
            var response = HttpResponseData.CreateResponse(httpRequestData);
            response.StatusCode = HttpStatusCode.Forbidden;
            response.Body.Write(Encoding.UTF8.GetBytes(String.Join(',', failure.FailedRequirements.Select(x => x.ToString()))));
            executingContext.SetInvocationResult(response);
        }
    }
}