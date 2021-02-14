namespace AzureFunctions.Extensions.OpenIDConnect
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Encapsulates checks of bearer tokens in HTTP request headers.
    /// </summary>
    internal class ApiAuthenticationService : IApiAuthentication
    {
        private readonly IAuthorizationHeaderBearerTokenExtractor _authorizationHeaderBearerTokenExractor;

        private readonly IJwtSecurityTokenHandlerWrapper _jwtSecurityTokenHandlerWrapper;

        private readonly IOidcConfigurationManager _oidcConfigurationManager;

        private readonly string _issuerUrl;
        private readonly string _issuer;
        private readonly string _audience;

        private readonly string _nameClaimType;
        private readonly string _roleClaimType;

        public ApiAuthenticationService(
            IOptions<OidcApiAuthSettings> apiAuthorizationSettingsOptions,
            IAuthorizationHeaderBearerTokenExtractor authorizationHeaderBearerTokenExractor,
            IJwtSecurityTokenHandlerWrapper jwtSecurityTokenHandlerWrapper,
            IOidcConfigurationManager oidcConfigurationManager)
        {
            _issuerUrl = apiAuthorizationSettingsOptions?.Value?.IssuerUrl;
            _issuer = apiAuthorizationSettingsOptions?.Value?.Issuer ?? _issuerUrl;
            _audience = apiAuthorizationSettingsOptions?.Value?.Audience;
            _nameClaimType = apiAuthorizationSettingsOptions?.Value?.NameClaimType ??  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
            _roleClaimType = apiAuthorizationSettingsOptions?.Value?.RoleClaimType ?? "http://schemas.microsoft.com/ws/2008/06/identity/claims/roleidentifier";

            _authorizationHeaderBearerTokenExractor = authorizationHeaderBearerTokenExractor;

            _jwtSecurityTokenHandlerWrapper = jwtSecurityTokenHandlerWrapper;

            _oidcConfigurationManager = oidcConfigurationManager;
        }

        /// <summary>
        /// Checks the given HTTP request headers for a valid OpenID Connect (OIDC) Authorization token.
        /// </summary>
        /// <param name="httpRequestHeaders">
        /// The HTTP request headers to check.
        /// </param>
        /// <returns>
        /// Information about the success or failure of the authorization.
        /// </returns>
        public async Task<ApiAuthenticationResult> AuthenticateAsync(
            IHeaderDictionary httpRequestHeaders)
        {
            bool isTokenValid = false;
            ClaimsPrincipal principal = new ClaimsPrincipal();

            string authorizationBearerToken = _authorizationHeaderBearerTokenExractor.GetToken(
                httpRequestHeaders);
            if (authorizationBearerToken == null)
            {
                return new ApiAuthenticationResult(principal,
                    "Authorization header is missing, invalid format, or is not a Bearer token.");
            }

            int validationRetryCount = 0;

            do
            {
                IEnumerable<SecurityKey> isserSigningKeys;
                try
                {
                    // Get the cached signing keys if they were retrieved previously. 
                    // If they haven't been retrieved, or the cached keys are stale,
                    // then a fresh set of signing keys are retrieved from the OpenID Connect provider
                    // (issuer) cached and returned.
                    // This method will throw if the configuration cannot be retrieved, instead of returning null.
                    isserSigningKeys = await _oidcConfigurationManager.GetIssuerSigningKeysAsync();
                }
                catch (Exception ex)
                {
                    return new ApiAuthenticationResult(principal,
                        "Problem getting signing keys from Open ID Connect provider (issuer)."
                        + $" ConfigurationManager threw {ex.GetType()} Message: {ex.Message}");
                }

                try
                {
                    // Try to validate the token.

                    var tokenValidationParameters = new TokenValidationParameters
                    {
                        RequireSignedTokens = true,
                        ValidAudience = _audience,
                        ValidateAudience = true,
                        ValidIssuer = _issuer,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        IssuerSigningKeys = isserSigningKeys,
                        NameClaimType = _nameClaimType,
                        RoleClaimType = _roleClaimType
                    };

                    // Throws if the the token cannot be validated.
                    principal = _jwtSecurityTokenHandlerWrapper.ValidateToken(
                        authorizationBearerToken,
                        tokenValidationParameters);

                    isTokenValid = true;
                }
                catch (SecurityTokenSignatureKeyNotFoundException)
                {
                    // A SecurityTokenSignatureKeyNotFoundException is thrown if the signing keys for
                    // validating the JWT could not be found. This could happen if the issuer has
                    // changed the signing keys since the last time they were retrieved by the
                    // ConfigurationManager. To handle this we ask the ConfigurationManger to refresh
                    // which causes it to retrieve the keys again the next time we ask for them.
                    // Then we retry by asking for the signing keys and validating the token again.
                    // We only retry once.

                    _oidcConfigurationManager.RequestRefresh();

                    validationRetryCount++;
                }
                catch (Exception ex)
                {
                    return new ApiAuthenticationResult(principal,
                        $"Authorization Failed. {ex.GetType()} caught while validating JWT token."
                        + $"Message: {ex.Message}");
                }

            } while (!isTokenValid && validationRetryCount <= 1);

            // Success result.
            return new ApiAuthenticationResult(principal);
        }
    }
}
