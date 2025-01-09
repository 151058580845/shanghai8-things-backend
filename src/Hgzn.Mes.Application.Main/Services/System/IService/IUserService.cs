using Hgzn.Mes.Application.Dtos;
using System.Security.Claims;

namespace Hgzn.Mes.Application.Services.Base
{
    public interface IUserService : IBaseService
    {
        Task<UserReadDto?> RegisterAsync(UserRegisterDto registerDto);

        Task<string> LoginAsync(UserLoginDto credential);

        Task LogoutAsync(IEnumerable<Claim> claims);

        Task<int> DeleteAsync(Guid id);

        Task<UserReadDto?> GetUserAsync(Guid id);

        Task<UserScopeReadDto?> GetCurrentUserAsync(IEnumerable<Claim> claims);

        Task<IEnumerable<UserReadDto>> GetUsersWhereAsync(UserQueryDto query);

        Task<UserReadDto?> ChangeRoleAsync(Guid userId, IEnumerable<Guid> roleIds);

        Task<CaptchaReadDto> GenerateCaptchaAsync();

        Task<int> ChangePasswordAsync(ChangePasswordDto passwordDto);

        Task<int> ResetPasswordAsync(Guid userId);
    }
}