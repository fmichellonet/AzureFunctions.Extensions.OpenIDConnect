using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using NSubstitute;
using NUnit.Framework;
using Microsoft.Azure.Functions.Worker.Http;
using BindingMetadata = Microsoft.Azure.Functions.Worker.BindingMetadata;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace AzureFunctions.Extensions.OpenIDConnect.Isolated.Tests;

public class AuthorizationFunctionMiddlewareShould
{
    [Test]
    public async Task PassExecutionToNextMiddleware_WhenRouteIsNotProtected()
    {
        // Arrange
        var routeGuardian = Substitute.For<IRouteGuardian>();
        routeGuardian.IsProtectedRoute(Arg.Any<string>()).Returns(false);

        var context = Substitute.ForPartsOf<FunctionContext>();
        var next = Substitute.For<FunctionExecutionDelegate>();
        var middleware = new AuthorizationFunctionMiddleware(null, routeGuardian, null, null, null);

        // Act
        await middleware.Invoke(context, next);

        // Arrange
        await next.Received(1).Invoke(Arg.Any<FunctionContext>());
    }

    [Test]
    public async Task ReturnUnauthorizedWhenAuthenticationFailed()
    {
        // Arrange
        var routeGuardian = Substitute.For<IRouteGuardian>();
        routeGuardian.IsProtectedRoute(Arg.Any<string>()).Returns(true);

        var authenticationService = Substitute.For<IAuthenticationService>();
        authenticationService.AuthenticateAsync(Arg.Any<IHeaderDictionary>())
            .Returns(new ApiAuthenticationResult(new ClaimsPrincipal(), "it just failed"));
        
        var context = Substitute.For<FunctionContext>();
        
        //var functionDefinition = BuildFunctionDefinition();
        //context.FunctionDefinition.Returns(functionDefinition);
        
        var functionContextAccessor = Substitute.For<IHttpFunctionContextAccessor>();
        functionContextAccessor.GetHttpRequestDataAsync()
            .Returns(ValueTask.FromResult(TestingHttpRequestData.For(context)));

        var contextAccessorFactory = Substitute.For<IHttpFunctionContextAccessorFactory>();
        contextAccessorFactory.Create(context)
            .Returns(functionContextAccessor);
        
        var middleware = new AuthorizationFunctionMiddleware(authenticationService, routeGuardian, null, null, contextAccessorFactory);

        var next = Substitute.For<FunctionExecutionDelegate>();

        // Act
        await middleware.Invoke(context, next);
        
        // Assert
        functionContextAccessor.Received(1).SetInvocationResult(Arg.Is<HttpResponseData>(x => x.StatusCode == HttpStatusCode.Unauthorized));

        await next.Received(0).Invoke(Arg.Any<FunctionContext>());
    }

    

    [Test]
    public async Task ReturnForbiddenWhenAuthorizationFailed()
    {
        // Arrange
        var routeGuardian = Substitute.For<IRouteGuardian>();
        routeGuardian.IsProtectedRoute(Arg.Any<string>()).Returns(true);
        routeGuardian.GetAuthorizationConfiguration(Arg.Any<string>()).Returns(new AuthorizeAttribute());

        var authenticationService = Substitute.For<IAuthenticationService>();
        authenticationService.AuthenticateAsync(Arg.Any<IHeaderDictionary>())
            .Returns(new ApiAuthenticationResult(new ClaimsPrincipal()));

        var requirements = new[]
        {
            new RolesAuthorizationRequirement(new[] { "Admin" })
        };
        var requirementsRetriever = Substitute.For<IAuthorizationRequirementsRetriever>();
        requirementsRetriever.ForAttribute(Arg.Any<AuthorizeAttribute>())
            .Returns(requirements);

        var authorizationService = Substitute.For<IAuthorizationService>();
        var context = Substitute.For<FunctionContext>();

        authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), context, requirements)
            .Returns(AuthorizationResult.Failed());

        var functionContextAccessor = Substitute.For<IHttpFunctionContextAccessor>();
        functionContextAccessor.GetHttpRequestDataAsync()
            .Returns(ValueTask.FromResult(TestingHttpRequestData.For(context)));

        var contextAccessorFactory = Substitute.For<IHttpFunctionContextAccessorFactory>();
        contextAccessorFactory.Create(context)
            .Returns(functionContextAccessor);

        var middleware = new AuthorizationFunctionMiddleware(authenticationService, routeGuardian, 
            authorizationService, requirementsRetriever, contextAccessorFactory);

        var next = Substitute.For<FunctionExecutionDelegate>();

        // Act
        await middleware.Invoke(context, next);

        // Assert
        functionContextAccessor.Received(1).SetInvocationResult(Arg.Is<HttpResponseData>(x => x.StatusCode == HttpStatusCode.Forbidden));

        await next.Received(0).Invoke(Arg.Any<FunctionContext>());
    }
    

    private static FunctionDefinition BuildFunctionDefinition()
    {
        var def = Substitute.For<FunctionDefinition>();
        var inputBindings = new Dictionary<string, BindingMetadata>()
        {
            {"1", BuildHttpTriggerMetadata()}
        };
        def.InputBindings.Returns(inputBindings.ToImmutableDictionary());
        return def;
    }

    private static BindingMetadata BuildHttpTriggerMetadata()
    {
        var metadata = Substitute.For<BindingMetadata>();
        metadata.Name.Returns("SomeName");
        metadata.Type.Returns("httpTrigger");
        return metadata;
    }

    
}
