using System.Security.Claims;

namespace Hgzn.Mes.Infrastructure.Utilities.CurrentUser;

public interface ICurrentPrincipalAccessor
{
    ClaimsPrincipal Principal { get; }

    IDisposable Change(ClaimsPrincipal principal);
}