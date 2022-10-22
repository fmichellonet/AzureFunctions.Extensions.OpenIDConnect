using AzureFunctions.Extensions.OpenIDConnect.Isolated.Configuration;
using AzureFunctions.Extensions.OpenIDConnect.Tests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using System.Collections.Generic;

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

        [Test]
        public void BeSecure_When_Using_Audiences_And_Issuer()
        {
            // Arrange
            var collection = ServiceCollectionFixture.MinimalAzFunctionsServices();

            var audiences = new List<string> { "my_audience_1", "my_audience_2" };
            var issuer = "https://me.secure.com";

            collection.AddOpenIDConnect(builder =>
            {
                builder.SetIssuerBaseUrlConfiguration("http://anyurl.com");
                builder.SetTokenValidation(audiences, issuer);
            });

            var provider = collection.BuildServiceProvider();

            var expected = new TokenValidationParameters
            {
                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,

                ValidateAudience = true,
                ValidAudiences = audiences,

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