using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Domain.Entities.System.Account;
using System.Security.Claims;

namespace Hgzn.Mes.Application.Main.Services.Base
{
    public interface IUserService : ICrudAppService<
        User, Guid,
        UserReadDto, UserQueryDto>
    {
        Task<UserReadDto?> RegisterAsync(UserRegisterDto registerDto);

        Task<string> LoginAsync(UserLoginDto credential);

        Task LogoutAsync(IEnumerable<Claim> claims);

        Task<UserScopeReadDto?> GetCurrentUserAsync(IEnumerable<Claim> claims);

        Task<UserReadDto?> ChangeRoleAsync(Guid userId, IEnumerable<Guid> roleIds);

        Task<CaptchaReadDto> GenerateCaptchaAsync();

        Task<int> ChangePasswordAsync(ChangePasswordDto passwordDto);

        Task<int> ResetPasswordAsync(Guid userId);
    }
}