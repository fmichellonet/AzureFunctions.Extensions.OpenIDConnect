namespace AzureFunctions.Extensions.OpenIDConnect
{
    using System.Security.Claims;

    /// <summary>
    /// Encapsulates the results of an API authorization.
    /// </summary>
    public class ApiAuthenticationResult
    {
        /// <summary>
        /// Constructs a success authorization.
        /// </summary>
        public ApiAuthenticationResult(ClaimsPrincipal principal)
        {
            User = principal;
        }

        /// <summary>
        /// Constructs a failed authorization with given reason.
        /// </summary>
        /// <param name="failureReason">
        /// Describes the reason for the authorization failure.
        /// </param>
        public ApiAuthenticationResult(ClaimsPrincipal principal, string failureReason)
        {
            User = principal;
            FailureReason = failureReason;
        }

        /// <summary>
        /// True if authorization failed.
        /// </summary>
        public bool Failed => FailureReason != null;

        /// <summary>
        /// String describing the reason for the authorization failure.
        /// </summary>
        public string FailureReason { get; }

        public ClaimsPrincipal User { get; }

        /// <summary>
        /// True if authorization was successful.
        /// </summary>
        public bool Success => !Failed;
    }
}
