using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Security.Principal;
using Hgzn.Mes.Application.Main.Utilities;

namespace Hgzn.Mes.Infrastructure.Utilities.CurrentUser;

public static class CurrentExtension
{
    public static string? FindClaimValue(this ICurrentUser currentUser, string claimType)
    {
        return currentUser.FindClaim(claimType)?.Value;
    }
    public static Guid? FindUserId([NotNull] this ClaimsPrincipal principal)
    {
        Check.NotNull(principal, nameof(principal));

        var userIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == ClaimType.UserId);
        if (userIdOrNull == null || userIdOrNull.Value.IsNullOrWhiteSpace())
        {
            return null;
        }

        if (Guid.TryParse(userIdOrNull.Value, out Guid guid))
        {
            return guid;
        }

        return null;
    }
    
    public static Guid? FindUserId(this IIdentity identifier)
    {
        Check.NotNull(identifier, nameof(identifier));
        var claimsIdentity = identifier as ClaimsIdentity;

        var userIdOrNull = claimsIdentity?.Claims.FirstOrDefault(t => t.Type == ClaimType.UserId);
        if (userIdOrNull == null || userIdOrNull.Value.IsNullOrWhiteSpace())
        {
            return null;
        }

        if (Guid.TryParse(userIdOrNull.Value,out var guid))
        {
            return guid;
        }
        return null;
    }
}