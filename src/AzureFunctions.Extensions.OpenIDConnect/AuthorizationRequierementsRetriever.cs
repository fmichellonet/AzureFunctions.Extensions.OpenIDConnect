using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace AzureFunctions.Extensions.OpenIDConnect
{
    public class AuthorizationRequirementsRetriever : IAuthorizationRequirementsRetriever
    {
        private readonly IDictionary<string, AuthorizationPolicy> _policyMap;

        public AuthorizationRequirementsRetriever(IDictionary<string, AuthorizationPolicy> policyMap)
        {
            _policyMap = policyMap;
        }

        public IEnumerable<IAuthorizationRequirement> ForAttribute(AuthorizeAttribute attribute)
        {
            if (!string.IsNullOrEmpty(attribute.Roles))
            {
                return ForRoleAttribute(attribute);
            }

            if (!string.IsNullOrEmpty(attribute.Policy))
            {
                return ForPolicyAttribute(attribute);
            }

            return null;
        }

        private IEnumerable<IAuthorizationRequirement> ForRoleAttribute(AuthorizeAttribute attribute)
        {
            var requirements = new List<RolesAuthorizationRequirement>
            {
                new RolesAuthorizationRequirement(attribute.Roles.Split(','))
            };

            return requirements;
        }

        private IEnumerable<IAuthorizationRequirement> ForPolicyAttribute(AuthorizeAttribute attribute)
        {
            if (!_policyMap.ContainsKey(attribute.Policy))
            {
                throw new ArgumentException($"{attribute.Policy} policy has not been registered");
            }

            return _policyMap[attribute.Policy].Requirements;
        }
    }
}