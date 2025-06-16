using System.Security.Claims;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Account;

namespace Hgzn.Mes.Application.Main.Services.System.IService
{
    public interface IUserService : ICrudAppService<
        User, Guid,
        UserReadDto, UserQueryDto, UserCreateDto, UserUpdateDto>
    {
        Task<UserReadDto?> RegisterAsync(UserRegisterDto registerDto);

        Task<string> LoginAsync(UserLoginDto credential);

        Task LogoutAsync(IEnumerable<Claim> claims);

        Task<UserScopeReadDto?> GetCurrentUserAsync(IEnumerable<Claim> claims);

        Task<UserReadDto?> ChangeRoleAsync(Guid userId, IEnumerable<Guid> roleIds);

        Task<CaptchaReadDto> GenerateCaptchaAsync();

        Task<int> ChangePasswordAsync(ChangePasswordDto passwordDto);

        Task<int> ResetPasswordAsync(Guid userId);

        /// <summary>
        /// 获取用户列表根据角色id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<IEnumerable<UserReadDto>> GetListByRoleIdAsync(Guid roleId);
    }
}