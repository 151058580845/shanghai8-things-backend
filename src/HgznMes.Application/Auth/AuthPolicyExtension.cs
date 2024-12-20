using HgznMes.Application.Auth.Requirements;
using Microsoft.AspNetCore.Authorization;
using HgznMes.Domain.ValueObjects;

namespace HgznMes.Application.Auth
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