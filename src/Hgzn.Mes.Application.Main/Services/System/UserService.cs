using System.Security.Claims;
using Hgzn.Mes.Application.Main.Captchas;
using Hgzn.Mes.Application.Main.Captchas.Builder;
using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.Base;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Application.Main.Utilities;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Services;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Exceptions;
using Hgzn.Mes.Domain.Shared.Utilities;
using Hgzn.Mes.Domain.Utilities;
using Hgzn.Mes.Infrastructure.Utilities;
using Microsoft.Extensions.Logging;

namespace Hgzn.Mes.Application.Main.Services.System
{
    public class UserService : SugarCrudAppService<
            User, Guid,
            UserReadDto, UserQueryDto,UserRegisterDto,UserUpdateDto>,
        IUserService
    {
        public UserService(
            IUserDomainService userDomainService,
            ILogger<UserService> logger
        )
        {
            _userDomainService = userDomainService;
            _logger = logger;
        }

        private readonly IUserDomainService _userDomainService;
        private readonly ILogger<UserService> _logger;

        public async Task<UserReadDto?> RegisterAsync(UserRegisterDto registerDto)
        {
            var user = Mapper.Map<User>(registerDto);
            var count = await DbContext.Insertable(user).ExecuteCommandAsync();
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

            var user = await Queryable.Includes(u => u.Roles)
                .FirstAsync(u => u.Username == credential.Username);
            var bytes = Convert.FromBase64String(credential.Password);

            if (user is null || !user.Verify(bytes))
            {
                throw new NotAcceptableException("user not found or password error");
            }

            var roleIds = string.Join(",", user.Roles.Select(r => r.Id));
            var token = JwtTokenUtil.GenerateJwtToken(SettingUtil.Jwt.Issuer, SettingUtil.Jwt.Audience,
                            SettingUtil.Jwt.ExpireMin,
                            new Claim(CustomClaimsType.UserId, user.Id.ToString()),
                            new Claim(CustomClaimsType.RoleId, roleIds)) ??
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

        public override async Task<UserReadDto?> GetAsync(Guid id)
        {
            var user = await Queryable
                .Where(u => u.Id == id)
                .Includes(u => u.Roles, r => r.Menus)
                .FirstAsync();
            return Mapper.Map<UserReadDto>(user);
        }

        public async Task<UserScopeReadDto?> GetCurrentUserAsync(IEnumerable<Claim> claims)
        {
            var userId = Guid.Parse(claims.FirstOrDefault(c => c.Type == CustomClaimsType.UserId)!.Value);
            var user = await Queryable
                .Where(u => u.Id == userId)
                .Includes(u => u.Roles, r => r.Menus)
                .FirstAsync();
            return Mapper.Map<UserScopeReadDto>(user);
        }

        public override async Task<IEnumerable<UserReadDto>> GetListAsync(UserQueryDto? query)
        {
            var users = await Queryable
                .Where(u => query == null || (query.Filter == null ||
                                              (u.Phone != null && u.Phone.Contains(query.Filter)) ||
                                              u.Username.Contains(query.Filter) ||
                                              (u.Nick != null && u.Nick.Contains(query.Filter)))
                    && u.State == query.State)
                .Includes(u => u.Roles)
                .ToArrayAsync();
            return Mapper.Map<IEnumerable<UserReadDto>>(users);
        }

        public async Task<UserReadDto?> ChangeRoleAsync(Guid userId, IEnumerable<Guid> roleIds)
        {
            var user = (await Queryable.FirstAsync(u => u.Id == userId)) ??
                       throw new NotFoundException("user not found");
            user.Roles = roleIds.Select(id => new Role { Id = id }).ToList();
            var count = await DbContext.Updateable(user).ExecuteCommandAsync();
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

            var user = await Queryable.FirstAsync(u => u.Username == passwordDto.Username) ??
                       throw new NotFoundException("user not found");
            var bytes = Convert.FromBase64String(passwordDto.OldPassword);
            if (!user.Verify(bytes))
            {
                throw new NotAcceptableException("password error");
            }

            _userDomainService.WithSalt(ref user, passwordDto.NewPassword);
            return await DbContext.Updateable(user).ExecuteCommandAsync();
        }

        public async Task<int> ResetPasswordAsync(Guid userId)
        {
            var user = await Queryable.FirstAsync(u => u.Id == userId) ??
                       throw new NotFoundException("user not found");
            var hash = CryptoUtil.Sha256("12345678");
            _userDomainService.WithSalt(ref user, hash);
            return await DbContext.Updateable(user).ExecuteCommandAsync();
        }

        public override async Task<PaginatedList<UserReadDto>> GetPaginatedListAsync(UserQueryDto query)
        {
            var users = await Queryable
                .Where(u => u.Phone != null && (string.IsNullOrEmpty(query.Phone) || u.Phone.Contains(query.Phone)))
                .Where(u => string.IsNullOrEmpty(query.UserName) || u.Username.Contains(query.UserName))
                .Where(u => query.DeptId == null || u.DeptId == query.DeptId)
                .Where(u => u.State == query.State)
                .Includes(u => u.Roles)
                .ToPaginatedListAsync(query.PageIndex, query.PageSize);
            return Mapper.Map<PaginatedList<UserReadDto>>(users);
        }
    }
}