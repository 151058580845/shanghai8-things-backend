using HgznMes.Application.Dtos;
using System.Security.Claims;

namespace HgznMes.Application.Services.Base
{
    public interface IUserService : IBaseService
    {
        public Task<UserReadDto?> RegisterAsync(UserRegisterDto registerDto);

        public Task<string> LoginAsync(UserLoginDto credential);

        public Task LogoutAsync(IEnumerable<Claim> claims);

        public Task<int> DeleteAsync(Guid id);

        public Task<UserReadDto?> GetUserAsync(Guid id);

        public Task<IEnumerable<UserReadDto>> GetUsersWhereAsync(UserQueryDto query);

        public Task<UserReadDto?> ChangeRoleAsync(Guid userId, IEnumerable<Guid> roleIds);

        public Task<CaptchaReadDto> GenerateCaptchaAsync();

        public Task<int> ChangePasswordAsync(ChangePasswordDto passwordDto);

        public Task<int> ResetPasswordAsync(Guid userId);
    }
}