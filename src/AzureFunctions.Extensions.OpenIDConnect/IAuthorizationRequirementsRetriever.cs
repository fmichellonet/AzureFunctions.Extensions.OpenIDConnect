using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace AzureFunctions.Extensions.OpenIDConnect
{
    public interface IAuthorizationRequirementsRetriever
    {
        IEnumerable<IAuthorizationRequirement> ForAttribute(AuthorizeAttribute attribute);
    }
}