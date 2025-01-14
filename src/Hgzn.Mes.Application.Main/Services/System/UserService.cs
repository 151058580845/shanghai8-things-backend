using Hgzn.Mes.Application.Main.Captchas;
using Hgzn.Mes.Application.Main.Captchas.Builder;
using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Application.Main.Services.Base;
using Hgzn.Mes.Application.Main.Utilities;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Services;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Exceptions;
using Hgzn.Mes.Domain.Shared.Utilities;
using Hgzn.Mes.Domain.Utilities;
using Hgzn.Mes.Infrastructure.DbContexts.Ef;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Hgzn.Mes.Application.Main.Services
{
    [ScopeDefinition("manage all user resources", ManagedResource.User)]
    public class UserService : BaseService, IUserService
    {
        public UserService(
            ApiDbContext pgDbContext,
            IUserDomainService userDomainService,
            ILogger<UserService> logger
            )
        {
            _userDomainService = userDomainService;
            _apiDbContext = pgDbContext;
            _logger = logger;
        }

        private readonly ApiDbContext _apiDbContext;
        private readonly IUserDomainService _userDomainService;
        private readonly ILogger<UserService> _logger;

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
            var user = await _apiDbContext.Users.Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Username == credential.Username);
            var bytes = Convert.FromBase64String(credential.Password);

            if (user is null || !user.Verify(bytes))
            {
                throw new NotAcceptableException("user not found or password error");
            }
            var roleIds = string.Join(",", user.Roles.Select(r => r.Id));
            var token = JwtTokenUtil.GenerateJwtToken(SettingUtil.Jwt.Issuer, SettingUtil.Jwt.Audience, SettingUtil.Jwt.ExpireMin,
                new Claim(CustomClaimsType.UserId, user.Id.ToString()), new Claim(CustomClaimsType.RoleId, roleIds)) ??
                throw new Exception("generate jwt token error");

            if (!await _userDomainService.VerifyTokenAsync(user.Id, token))
            {
                //throw new ForbiddenException("user was logged in elsewhere");
                _logger.LogWarning("user was logged in elsewhere");
            }
            await _userDomainService.CacheTokenAsync(user.Id, token);
            return token;
        }

        public async Task LogoutAsync(IEnumerable<Claim> claims)
        {
            var userId = claims.FirstOrDefault(c => c.Type == CustomClaimsType.UserId)!.Value;
            await _userDomainService.DeleteTokenAsync(Guid.Parse(userId));
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
                .Include(u => u.Roles)
                .ThenInclude(r => r.Menus)
                .FirstOrDefaultAsync();
            return Mapper.Map<UserReadDto>(user);
        }

        public async Task<UserScopeReadDto?> GetCurrentUserAsync(IEnumerable<Claim> claims)
        {
            var userId = Guid.Parse(claims.FirstOrDefault(c => c.Type == CustomClaimsType.UserId)!.Value);
            var user = await _apiDbContext.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Roles)
                .ThenInclude(r => r.Menus)
                .FirstOrDefaultAsync();
            return Mapper.Map<UserScopeReadDto>(user);
        }

        [ScopeDefinition("get users where", $"{ManagedResource.User}.{ManagedAction.Get}.Query")]
        public async Task<IEnumerable<UserReadDto>> GetUsersWhereAsync(UserQueryDto query)
        {
            var users = await _apiDbContext.Users
                .Where(u => query.Filter == null ||
                (u.Phone != null && u.Phone.Contains(query.Filter)) ||
                u.Username.Contains(query.Filter) ||
                        (u.Nick != null && u.Nick.Contains(query.Filter)))
                .Where(u => query == null || u.State == query.State)
                .Include(u => u.Roles)
                .ToArrayAsync();
            return Mapper.Map<IEnumerable<UserReadDto>>(users);
        }

        [ScopeDefinition("change user role", $"{ManagedResource.User}.{ManagedAction.Put}.Role")]
        public async Task<UserReadDto?> ChangeRoleAsync(Guid userId, IEnumerable<Guid> roleIds)
        {
            var user = (await _apiDbContext.Users.FindAsync(userId)) ??
                throw new NotFoundException("user not found");
            user.Roles = roleIds.Select(id => new Role { Id = id }).ToArray();
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
                Height = 40,
                Width = 120,
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