using HgznMes.Application.Auth;
using HgznMes.Application.Captchas;
using HgznMes.Application.Captchas.Builder;
using HgznMes.Application.Dtos;
using HgznMes.Application.Services.Base;
using HgznMes.Application.Utilities;
using HgznMes.Domain.Shared;
using HgznMes.Domain.Shared.Exceptions;
using HgznMes.Domain.Shared.Utilities;
using HgznMes.Domain.Services;
using HgznMes.Domain.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HgznMes.Infrastructure.DbContexts;
using HgznMes.Domain.Entities.Login;

namespace HgznMes.Application.Services
{
    [ScopeDefinition("manage all user resources", ManagedResource.User)]
    public class UserService : BaseService, IUserService
    {
        public UserService(
            ApiDbContext pgDbContext,
            IUserDomainService userDomainService
            )
        {
            _userDomainService = userDomainService;
            _apiDbContext = pgDbContext;
        }
        
        private readonly ApiDbContext _apiDbContext;
        private readonly IUserDomainService _userDomainService;

        public async Task<UserReadDto?> RegisterAsync(UserRegisterDto registerDto)
        {
            var user = Mapper.Map<User>(registerDto);
            await _apiDbContext.Users.AddAsync(user);
            var count = await _apiDbContext.SaveChangesAsync();
            return count == 0 ? null : Mapper.Map<UserReadDto>(user);
        }

        public async Task<string> LoginAsync(UserLoginDto credential)
        {
            var answer = Mapper.Map<Captcha>(credential.Captcha);

            if (!SettingUtil.IsDevelopment
                && (answer is null || !await _userDomainService.VerifyCaptchaAnswerAsync(answer)))
            {
                throw new NotAcceptableException("captcha not found or not correct");
            }
            var user = await _apiDbContext.Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == credential.Username);
            var bytes = Convert.FromBase64String(credential.Password);

            if (user is null || !user.Verify(bytes))
            {
                throw new NotAcceptableException("user not found or password error");
            }
            var token = JwtTokenUtil.GenerateJwtToken(SettingUtil.Jwt.Issuer, SettingUtil.Jwt.Audience, SettingUtil.Jwt.ExpireMin,
                new Claim(CustomClaimsType.UserId, user.Id.ToString()), new Claim(CustomClaimsType.RoleId, user.Role.Id.ToString())) ??
                throw new Exception("generate jwt token error");

            if (!await _userDomainService.VerifyTokenAsync(user.Id, token))
            {
                throw new ForbiddenException("user was logged in elsewhere");
            }
            await _userDomainService.CacheTokenAsync(user.Id, token);
            return token;
        }

        public async Task LogoutAsync(IEnumerable<Claim> claims)
        {
            var userId = claims.FirstOrDefault(c => c.Type == CustomClaimsType.UserId)!.Value;
            await _userDomainService.DeleteTokenAsync(Guid.Parse(userId)!);
        }

        [ScopeDefinition("delete user by id", $"{ManagedResource.User}.{ManagedAction.Delete}.Id")]
        public async Task<int> DeleteAsync(Guid id)
        {
            var user = await _apiDbContext.Users.FindAsync(id);
            if (user is not null)
                _apiDbContext.Users.Remove(user);
            return await _apiDbContext.SaveChangesAsync();
        }

        [ScopeDefinition("get single user by id", $"{ManagedResource.User}.{ManagedAction.Get}.Id")]
        public async Task<UserReadDto?> GetUserAsync(Guid id)
        {
            var user = await _apiDbContext.Users
                .Where(u => u.Id == id)
                .Include(u => u.Role)
                .FirstOrDefaultAsync();
            return Mapper.Map<UserReadDto>(user);
        }

        [ScopeDefinition("get users where", $"{ManagedResource.User}.{ManagedAction.Get}.Query")]
        public async Task<IEnumerable<UserReadDto>> GetUsersWhereAsync(string? name = null)
        {
            var users = await _apiDbContext.Users
                .Where(u => string.IsNullOrEmpty(name) || u.Username.Contains(name) ||
                u.DisplayName == null || u.DisplayName.Contains(name))
                .Include(u => u.Role)
                .ToArrayAsync();
            return Mapper.Map<IEnumerable<UserReadDto>>(users);
        }

        [ScopeDefinition("change user role", $"{ManagedResource.User}.{ManagedAction.Put}.Role")]
        public async Task<UserReadDto?> ChangeRoleAsync(Guid userId, Guid roleId)
        {
            var user = (await _apiDbContext.Users.FindAsync(userId)) ??
                throw new NotFoundException("user not found");
            if (await _apiDbContext.Roles.FindAsync(roleId) is null)
                throw new NotFoundException("role not found");
            user.RoleId = roleId;
            _apiDbContext.Users.Update(user);
            var count = await _apiDbContext.SaveChangesAsync();
            return count == 0 ? null : Mapper.Map<UserReadDto>(user);
        }

        public async Task<CaptchaReadDto> GenerateCaptchaAsync()
        {
            //var builder = CaptchaBuilder.Create<CharacterCaptchaBuilder>()
            //    .WithLowerCase()
            //    .WithUpperCase();
            var builder = CaptchaBuilder.Create<QuestionCaptchaBuilder>();
            var captcha = builder.WithGenOption(new CaptchaGenOptions
            {
                FontFamily = "consolas",
                Height = 80,
                Width = 200,
            }).WithNoise().WithLines().WithCircles().Build();
            await _userDomainService.CacheCaptchaAnswerAsync(captcha, 180);
            return Mapper.Map<CaptchaReadDto>(captcha);
        }

        public async Task<int> ChangePasswordAsync(ChangePasswordDto passwordDto)
        {
            var answer = Mapper.Map<Captcha>(passwordDto.Captcha);

            if (!SettingUtil.IsDevelopment
                && (answer is null || !await _userDomainService.VerifyCaptchaAnswerAsync(answer)))
            {
                throw new NotAcceptableException("captcha not exist or not correct");
            }
            var user = await _apiDbContext.Users.FindAsync(passwordDto.Username) ??
                throw new NotFoundException("user not found");
            var bytes = Convert.FromBase64String(passwordDto.OldPassword);
            if (!user.Verify(bytes))
            {
                throw new NotAcceptableException("password error");
            }

            _userDomainService.WithSalt(ref user, passwordDto.NewPassword);
            _apiDbContext.Users.Update(user);
            return await _apiDbContext.SaveChangesAsync();
        }

        [ScopeDefinition("reset someone's password", $"{ManagedResource.User}.{ManagedAction.Put}.ResetPwd")]
        public async Task<int> ResetPasswordAsync(Guid userId)
        {
            var user = await _apiDbContext.Users.FindAsync(userId) ??
                throw new NotFoundException("user not found");
            var hash = CryptoUtil.Sha256("12345678");
            _userDomainService.WithSalt(ref user, hash);
            _apiDbContext.Users.Update(user);
            return await _apiDbContext.SaveChangesAsync();
        }
    }
}