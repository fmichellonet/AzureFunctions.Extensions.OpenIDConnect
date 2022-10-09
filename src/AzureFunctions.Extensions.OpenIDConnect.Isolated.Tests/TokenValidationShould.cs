﻿using AzureFunctions.Extensions.OpenIDConnect.Isolated.Configuration;
using AzureFunctions.Extensions.OpenIDConnect.Tests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;

namespace AzureFunctions.Extensions.OpenIDConnect.Isolated.Tests
{
    public class TokenValidationShould
    {
        [Test]
        public void BeSecure_When_Using_Audience_And_Issuer()
        {
            // Arrange
            var collection = ServiceCollectionFixture.MinimalAzFunctionsServices();

            var audience = "my_audience";
            var issuer = "https://me.secure.com";

            collection.AddOpenIDConnect(builder =>
            {
                builder.SetIssuerBaseUrlConfiguration("http://anyurl.com");
                builder.SetTokenValidation(audience, issuer);
            });

            var provider = collection.BuildServiceProvider();

            var expected = new TokenValidationParameters
            {
                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,

                ValidateAudience = true,
                ValidAudience = audience,

                ValidateIssuer = true,
                ValidIssuer = issuer
            };

            // Act
            var tokenValidationParameters = provider.GetService<TokenValidationParameters>();

            // Assert
            tokenValidationParameters.Should().BeEquivalentTo(expected);
        }
    }
}