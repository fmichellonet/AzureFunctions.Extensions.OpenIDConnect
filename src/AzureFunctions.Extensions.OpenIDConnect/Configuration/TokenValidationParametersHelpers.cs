using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace AzureFunctions.Extensions.OpenIDConnect.Configuration
{
    public static class TokenValidationParametersHelpers
    {
        public static TokenValidationParameters Default(string audience, string issuer)
        {
            return new TokenValidationParameters
            {
                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,

                ValidateAudience = true,
                ValidAudience = audience,

                ValidateIssuer = true,
                ValidIssuer = issuer
            };
        }

        public static TokenValidationParameters Default(IEnumerable<string> audiences, string issuer)
        {
            return new TokenValidationParameters
            {
                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,

                ValidateAudience = true,
                ValidAudiences = audiences,

                ValidateIssuer = true,
                ValidIssuer = issuer
            };
        }
    }
}