using System.Security.Claims;

namespace Hgzn.Mes.Infrastructure.Utilities.CurrentUser;

public class ThreadCurrentPrincipalAccessor : CurrentPrincipalAccessorBase
{
    protected override ClaimsPrincipal GetClaimsPrincipal()
    {
        return (Thread.CurrentPrincipal as ClaimsPrincipal)!;
    }
}
