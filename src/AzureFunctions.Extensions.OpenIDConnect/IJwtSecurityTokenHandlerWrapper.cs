namespace AzureFunctions.Extensions.OpenIDConnect
{
    using System.Security.Claims;
    using Microsoft.IdentityModel.Tokens;

    internal interface IJwtSecurityTokenHandlerWrapper
    {
        ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters);
    }
}
