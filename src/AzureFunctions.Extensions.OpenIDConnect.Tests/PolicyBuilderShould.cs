using System;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using NUnit.Framework;

namespace AzureFunctions.Extensions.OpenIDConnect.Tests
{
    [TestFixture]
    public class PolicyBuilderShould
    {
        [Test]
        public void Register_Created_Policies()
        {
            // Arrange
            var claim = new ClaimsAuthorizationRequirement("locale", new[] {"fr"});
            Action<AuthorizationPolicyBuilder> configurePolicy = policyBuilder => policyBuilder.Requirements.Add(claim);
            var policyBuilder = new AuthorizationPolicyBuilder();

            // Act
            configurePolicy(policyBuilder);
            var res = policyBuilder.Build();

            // Assert
            res.Requirements.Should().HaveCount(1);
            res.Requirements.Should().BeEquivalentTo(claim);
        }
    }
}