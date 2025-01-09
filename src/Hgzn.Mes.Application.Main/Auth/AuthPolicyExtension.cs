using Hgzn.Mes.Application.Auth.Requirements;
using Microsoft.AspNetCore.Authorization;
using Hgzn.Mes.Domain.ValueObjects;

namespace Hgzn.Mes.Application.Auth
{
    public static class AuthPolicyExtension
    {
        public static void AddPolicyExt(this AuthorizationOptions options, IEnumerable<ScopeDefinition> scopes)
        {
            foreach (var scope in scopes)
            {
                options.AddPolicy(scope.Name, policy =>
                    policy.AddRequirements(new CustomRequireScope(scope.Name)));
            }
        }
    }
}