namespace AzureFunctions.Extensions.OpenIDConnect
{
    /// <summary>
    /// Encapsulates settings used in OpenID Connect (OIDC) API authentication.
    /// </summary>
    public class OidcApiAuthSettings
    {
        private string _issuerUrl;

        /// <summary>
        /// Identifies the API to be authorized by the Open ID Connect provider (issuer).
        /// </summary>
        /// <remarks>
        /// The "Audience" is the identifer used by the authorization provider to identify
        /// the API (HTTP triggered Azure Function) being protected. This is often a URL but
        /// it is not used as a URL is is simply used as an identifier.
        /// 
        /// For Auth0 use the API's Identifier in the Auth0 Dashboard.
        /// </remarks>
        public string Audience { get; set; }

        /// <summary>
        /// The URL of the Open ID Connect provider (issuer) that will perform API authorization.
        /// </summary>
        /// <remarks>
        /// The "Issuer" is the URL for the authorization provider's end-point. This URL will be
        /// used as part of the OpenID Connect protocol to obtain the the signing keys
        /// that will be used to validate the JWT Bearer tokens in incoming HTTP request headers.
        /// 
        /// For Auth0 the URL format is:  https://{Auth0-tenant-domain}.auth0.com 
        /// </remarks>
        public string IssuerUrl
        {
            get
            {
                return _issuerUrl;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.EndsWith("/"))
                {
                    _issuerUrl = value + "/";
                }
                else
                {
                    _issuerUrl = value;
                }
            }
        }


        /// <summary>
        /// (Optional) The URL for the identity provider's well-known openid-configuration url.
        /// Default Vaule: `$"{IssuerUrl}.well-known/openid-configuration"`
        /// </summary>
        /// <remarks>
        /// For Auth0, leave this blank.
        /// For Azure AD B2C, use `https://yourb2cdomain.b2clogin.com/yourb2cdomain.onmicrosoft.com/yoursigninuserflowname/v2.0/.well-known/openid-configuration/`
        /// </remarks>
        public string MetadataAddress { get; set; }

          /// <summary>
        /// (Optional) A string defining the name of the claim that will identify the user's name
        /// Default value: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
        /// </summary>
        /// <remarks>
        /// The "Issuer" is the URL for the authorization provider's end-point. This URL will be
        /// used as part of the OpenID Connect protocol to obtain the the signing keys
        /// that will be used to validate the JWT Bearer tokens in incoming HTTP request headers.
        /// 
        /// For Auth0 the URL format is:  https://{Auth0-tenant-domain}.auth0.com 
        /// </remarks>
        public string NameClaimType {get; set;}

        /// <summary>
        /// (Optional) A string defining the name of the claim that will identify the user's role membership
        /// Default value: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
        /// </summary>
        /// <remarks>
        /// The "Issuer" is the URL for the authorization provider's end-point. This URL will be
        /// used as part of the OpenID Connect protocol to obtain the the signing keys
        /// that will be used to validate the JWT Bearer tokens in incoming HTTP request headers.
        /// 
        /// For Auth0 the URL format is:  https://{Auth0-tenant-domain}.auth0.com 
        /// </remarks>
        public string RoleClaimType {get; set;}


        public string Issuer { get; set; }
    }
}
