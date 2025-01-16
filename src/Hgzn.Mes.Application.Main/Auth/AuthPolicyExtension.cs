using Hgzn.Mes.Application.Main.Auth.Requirements;
using Hgzn.Mes.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;

namespace Hgzn.Mes.Application.Main.Auth
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