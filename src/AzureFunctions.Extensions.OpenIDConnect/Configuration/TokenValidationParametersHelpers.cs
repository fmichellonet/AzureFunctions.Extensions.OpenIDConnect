using Microsoft.IdentityModel.Tokens;

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
    }
}