namespace AzureFunctions.Extensions.OpenIDConnect
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public interface IAuthenticationService
    {
        Task<ApiAuthenticationResult> AuthenticateAsync(IHeaderDictionary httpRequestHeaders);
    }
}