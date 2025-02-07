using System.Security.Claims;

namespace Hgzn.Mes.Infrastructure.Utilities.CurrentUser;

public abstract class CurrentPrincipalAccessorBase : ICurrentPrincipalAccessor
{
    public ClaimsPrincipal Principal => _currentPrincipal.Value ?? GetClaimsPrincipal();
    private readonly AsyncLocal<ClaimsPrincipal> _currentPrincipal = new AsyncLocal<ClaimsPrincipal>();
    protected abstract ClaimsPrincipal GetClaimsPrincipal();

    public virtual IDisposable Change(ClaimsPrincipal principal)
    {
        return SetCurrentPrincipal(principal);
    }

    private IDisposable SetCurrentPrincipal(ClaimsPrincipal principal)
    {
        var parent = principal;
        _currentPrincipal.Value = principal;
        return new DisposeAction<ValueTuple<AsyncLocal<ClaimsPrincipal>, ClaimsPrincipal>>(static (state) =>
        {
            var (currentPrincipal, parent) = state;
            currentPrincipal.Value = parent;
        }, (_currentPrincipal, parent));
    }
}