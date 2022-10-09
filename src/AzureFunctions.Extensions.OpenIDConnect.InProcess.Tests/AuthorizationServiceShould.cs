using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AzureFunctions.Extensions.OpenIDConnect.InProcess.Configuration;
using AzureFunctions.Extensions.OpenIDConnect.Tests;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace AzureFunctions.Extensions.OpenIDConnect.InProcess.Tests
{
    [TestFixture]
    public class AuthorizationServiceShould
    {

        [TestCase("locale", "fr", "fr", true)]
        [TestCase("locale", "it", "fr", false)]
        public async Task ApplyPolicyRequirements(string claimType, string claimValue, string claimRequiredValue, bool isAuthorized)
        {
            // Arrange
            var policyName = "my policy";
            var localeClaim = new Claim(claimType, claimValue);

            var collection = ServiceCollectionFixture.MinimalAzFunctionsServices()
                            .WithClaimsAuthorizationHandler(claimType, new[] { claimRequiredValue });

            collection.AddOpenIDConnect(builder =>
            {
                builder.SetIssuerBaseUrlConfiguration("https://issuer.com/");
                builder.SetTokenValidation("issuer", "audience");
                builder.SetTypeCrawler(() => new Type[] { });
                builder.AddPolicy(policyName, policy => policy.Requirements.Add(new ClaimsAuthorizationRequirement(claimType, new[] { claimRequiredValue })));
            });

            var provider = collection.BuildServiceProvider();

            var retriever = provider.GetService<IAuthorizationRequirementsRetriever>();
            var authorizationService = provider.GetService<IAuthorizationService>();

            var user = Substitute.For<ClaimsPrincipal>();
            user.Claims.Returns(new List<Claim> { localeClaim });


            // Act
            var requirements = retriever.ForAttribute(new AuthorizeAttribute(policyName));
            var authResult = await authorizationService.AuthorizeAsync(user, null, requirements);

            // Assert
            authResult.Succeeded.Should().Be(isAuthorized);
        }
    }
}