using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;

namespace Hgzn.Mes.Infrastructure.Utilities.CurrentUser;

public class CurrentUser:ICurrentUser
{
    private readonly ICurrentPrincipalAccessor _principalAccessor;

    public CurrentUser(ICurrentPrincipalAccessor principalAccessor)
    {
        _principalAccessor = principalAccessor;
    }

    public virtual bool IsAuthenticated => Id.HasValue;
    public Guid? Id => _principalAccessor.Principal?.FindUserId();
    public virtual string? UserName => this.FindClaimValue(ClaimType.UserName);

    public virtual string? Name => this.FindClaimValue(ClaimType.Name);

    public virtual string? SurName => this.FindClaimValue(ClaimType.SurName);

    public virtual string? PhoneNumber => this.FindClaimValue(ClaimType.PhoneNumber);

    public virtual bool PhoneNumberVerified => string.Equals(this.FindClaimValue(ClaimType.PhoneNumberVerified), "true", StringComparison.InvariantCultureIgnoreCase);

    public virtual string? Email => this.FindClaimValue(ClaimType.Email);

    public virtual bool EmailVerified => string.Equals(this.FindClaimValue(ClaimType.EmailVerified), "true", StringComparison.InvariantCultureIgnoreCase);
    
    public string[] Roles  => FindClaims(ClaimType.RoleId).Select(c => c.Value).Distinct().ToArray();
    public Claim? FindClaim(string claimType)
    {
        return _principalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == claimType);
    }

    public Claim[] FindClaims(string claimType)
    {
        return _principalAccessor.Principal?.Claims.Where(c => c.Type == claimType).ToArray() ?? [];
    }

    public Claim[] GetAllClaims()
    {
        return _principalAccessor.Principal?.Claims.ToArray() ?? [];
    }

    public bool IsInRole(string roleName)
    {
        return FindClaims(ClaimType.RoleId).Any(c => c.Value == roleName);
    }
}