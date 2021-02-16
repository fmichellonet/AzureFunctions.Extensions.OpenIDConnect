namespace AzureFunctions.Extensions.OpenIDConnect
{
    using System.Threading.Tasks;

    public interface IRouteGuardian
    {
        Task<bool> ShouldAuthorize(string functionName);
    }
}