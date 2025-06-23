using System.Security.Claims;
using Ericc.EntityFrameworkCore.OpenGauss.Infrastructure.Internal;
using Hgzn.Mes.Application.Main.Captchas;
using Hgzn.Mes.Application.Main.Captchas.Builder;
using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Application.Main.Dtos.Audit;
using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.Audit.IService;
using Hgzn.Mes.Application.Main.Services.Base;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Application.Main.Utilities;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Entities.System.Authority;
using Hgzn.Mes.Domain.Entities.System.Config;
using Hgzn.Mes.Domain.Services;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.Shared.Exceptions;
using Hgzn.Mes.Domain.Shared.Extensions;
using Hgzn.Mes.Domain.Shared.Utilities;
using Hgzn.Mes.Domain.Utilities;
using Hgzn.Mes.Infrastructure.Hub;
using Hgzn.Mes.Infrastructure.Utilities;
using Hgzn.Mes.Infrastructure.Utilities.CurrentUser;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Services.System
{
    public class UserService : SugarCrudAppService<
        User, Guid,
        UserReadDto, UserQueryDto, UserCreateDto, UserUpdateDto>,
        IUserService
    {
        public UserService(
            IUserDomainService userDomainService,
            ILogger<UserService> logger,
            IBaseConfigService baseConfigService,
            ILoginLogService loginLogService,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _userDomainService = userDomainService;
            _logger = logger;
            _loginLogService = loginLogService;
            _httpContextAccessor = httpContextAccessor;
            _baseConfigService = baseConfigService;
        }

        private  ILoginLogService  _loginLogService;
        private readonly IUserDomainService _userDomainService;
        private readonly ILogger<UserService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBaseConfigService _baseConfigService;

        public async Task<UserReadDto?> RegisterAsync(UserRegisterDto registerDto)
        {
            var targetRoleIds = (registerDto.RoleIds is null || !registerDto.RoleIds.Any()) ?
                [Role.MemberRole.Id] : registerDto.RoleIds;
            var roles = await DbContext.Queryable<Role>()
                .Where(r => targetRoleIds.Contains(r.Id))
                .ToListAsync();
            if (roles is null || roles.Count == 0)
                throw new BadRequestException("the choosing role not exist");
            var user = await DbContext.Queryable<User>()
                .Where(u => u.Username == registerDto.Username)
                .FirstAsync();
            if (user is not null) throw new BadRequestException("username exist");
            user = Mapper.Map<User>(registerDto);
            user.Roles = roles;
            var status = await DbContext.InsertNav(user)
                .Include(u => u.Roles)
                .ExecuteCommandAsync();
            return status ? null : Mapper.Map<UserReadDto>(user);
        }

        public override async Task<UserReadDto?> CreateAsync(UserCreateDto createDto)
        {
            var targetRoleIds = (createDto.RoleIds is null || !createDto.RoleIds.Any()) ?
                [Role.MemberRole.Id] : createDto.RoleIds;
            var roles = await DbContext.Queryable<Role>()
                .Where(r => targetRoleIds.Contains(r.Id))
                .ToListAsync();
            if(roles is null || roles.Count == 0)
                throw new BadRequestException("the choosing role not exist");
            if (await DbContext.Queryable<User>()
                .AnyAsync(u => u.Username == createDto.Username))
                throw new BadRequestException("username exist");
            var user = Mapper.Map<User>(createDto);
            var password = await _baseConfigService
                .GetValueByKeyAsync(BaseConfig.DefaultPassword.ConfigKey) ?? "12345678";
            var hash = CryptoUtil.Sha256(password);
            _userDomainService.WithSalt(ref user, hash);
            user.Roles = roles;
            var status = await DbContext.InsertNav(user)
                .Include(u => u.Roles)
                .ExecuteCommandAsync();
            return status ? null : Mapper.Map<UserReadDto>(user);
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

            if(user.Roles.Count == 0)
            {
                throw new NotAcceptableException("user must has at least one role");
            }

            var roleIds = string.Join(",", user.Roles.Select(r => r.Id));
            var minLevel = user.Roles.Min(r => r.Level);
            var token = JwtTokenUtil.GenerateJwtToken(SettingUtil.Jwt.Issuer, SettingUtil.Jwt.Audience,
                            SettingUtil.Jwt.ExpireMin,
                            new Claim(ClaimType.UserId, user.Id.ToString()),
                            new Claim(ClaimType.UserName, user.Username),
                            new Claim(ClaimType.RoleId, roleIds),
                            new Claim(ClaimType.Level, $"{minLevel}")) ??
                        throw new Exception("generate jwt token error");

            if (!await _userDomainService.VerifyTokenAsync(user.Id, token))
            {
                //throw new ForbiddenException("user was logged in elsewhere");
                _logger.LogWarning("user was logged in elsewhere");
            }

            // 登录成功后插入登录记录
            // 获取 HTTP 请求相关信息
            var httpContext = _httpContextAccessor.HttpContext;
            var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();
            var loginUser = _loginLogService.GetInfoByHttpContext(httpContext);
            
            await _loginLogService.CreateAsync(new LoginLogCreateDto() { 
                Browser= loginUser.Browser,
                Os = loginUser.Os,
                CreationTime = DateTime.UtcNow,
                LoginIp= ipAddress,
                LoginLocation= loginUser.LoginLocation,
                LoginUser= user.Username,
                LogMsg= user.Id.ToString()
            });

            await _userDomainService.CacheTokenAsync(user.Id, token);
            return token;
        }

        public async Task LogoutAsync(IEnumerable<Claim> claims)
        {
            var userId = claims.FirstOrDefault(c => c.Type == ClaimType.UserId)!.Value;
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

        public override async Task<UserReadDto?> UpdateAsync(Guid uid, UserUpdateDto dto)
        {
            await DbContext.Ado.BeginTranAsync();
            var count = 0;
            try
            {
                var user = await DbContext.Queryable<User>()
                    .Includes(u => u.Roles)
                    .FirstAsync(u => u.Id == uid) ??
                    throw new NotFoundException("uesr not found!");
                var entity = Mapper.Map(dto, user);
                count += await DbContext.Updateable(entity).ExecuteCommandAsync();

                if (dto.RoleIds != null && dto.RoleIds.Any() &&
                    !user.Roles.Select(r => r.Id).SequenceEqual(dto.RoleIds))
                {
                    if (!await DbContext.Queryable<Role>()
                        .AnyAsync(r => dto.RoleIds!.Contains(r.Id)))
                    {
                        throw new BadRequestException("one or more role not exist!");
                    }
                    var relation = dto.RoleIds.Select(rid =>
                        new UserRole { UserId = uid, RoleId = rid }).ToList();

                    count += await DbContext.Deleteable<UserRole>()
                        .Where(ur => ur.UserId == uid).ExecuteCommandAsync();

                    count += await DbContext.Insertable<UserRole>(relation).ExecuteCommandAsync();
                    user = await DbContext.Queryable<User>()
                        .Includes(u => u.Roles)
                        .FirstAsync();
                }
                await DbContext.Ado.CommitTranAsync();
                return Mapper.Map<UserReadDto>(entity);
            }
            catch
            {
                await DbContext.Ado.RollbackTranAsync();
                return null;
            }
        }

        public async Task<int> DeleteRangeAsync(IEnumerable<Guid> ids)
        {
            var count = await DbContext.Deleteable<User>()
                .Where(u => ids.Contains(u.Id))
                .ExecuteCommandAsync();
            return count;
        }

        public async Task<UserScopeReadDto?> GetCurrentUserAsync(IEnumerable<Claim> claims)
        {
            var userId = Guid.Parse(claims.FirstOrDefault(c => c.Type == ClaimType.UserId)!.Value);
            var user = await Queryable
                .Where(u => u.Id == userId)
                .Includes(u => u.Roles, r => r.Menus)
                .FirstAsync();
            var userDto = Mapper.Map<UserScopeReadDto>(user);
            
            if (user.Roles.Any(r => r.Id == Role.DevRole.Id))
            {
                userDto.ScopeCodes = (await DbContext.Queryable<Menu>()
                    .Where(t => t.Type == MenuType.Component)
                    .Select(t => t.ScopeCode)
                    .ToListAsync()).Where(t=>t != null)!;
            }
            else
            {
                userDto.ScopeCodes = (await user.Roles
                    .SelectMany(r => r.Menus ?? Enumerable.Empty<Menu>())
                    .Where(t => t.Type == MenuType.Component)
                    .Select(t => t.ScopeCode)
                    .ToListAsync()).Where(t=>t != null)!;;
            }
            
            return userDto;
        }

        public override async Task<IEnumerable<UserReadDto>> GetListAsync(UserQueryDto? query)
        {
            var users = await Queryable
                .WhereIF(!string.IsNullOrEmpty(query?.Phone), u => u.Phone == null || u.Phone.Contains(query!.Phone!))
                .WhereIF(!string.IsNullOrEmpty(query?.UserName) , u => u.Username.Contains(query!.UserName!))
                .WhereIF(query?.DeptId != null, u => u.DeptId == query!.DeptId)
                .WhereIF(query?.State != null, u => u.State == query!.State)
                .Includes(u => u.Roles)
                .ToListAsync();
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

        public async Task<IEnumerable<UserReadDto>> GetUserListByRoleId(Guid roleId)
        {
           var list = await Queryable.Where(t=>SqlFunc.Subqueryable<UserRole>().Where(m=>m.RoleId == roleId && m.UserId == t.Id).Any()).ToListAsync();
           return Mapper.Map<IEnumerable<UserReadDto>>(list);
        }

        public override async Task<PaginatedList<UserReadDto>> GetPaginatedListAsync(UserQueryDto query)
        {
            var users = await Queryable
                .Where(u => u.Phone != null && (string.IsNullOrEmpty(query.Phone) || u.Phone.Contains(query.Phone)))
                .Where(u => string.IsNullOrEmpty(query.UserName) || u.Username.Contains(query.UserName))
                .Where(u => query.DeptId == null || u.DeptId == query.DeptId)
                .Where(u =>query.State == null || u.State == query.State)
                .Includes(u => u.Roles)
                .ToPaginatedListAsync(query.PageIndex, query.PageSize);
            return Mapper.Map<PaginatedList<UserReadDto>>(users);
        }

        /// <summary>
        /// 用角色id查询用户列表  TODO：写到这里了接下来应该去写控制器然后对接接口了
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UserReadDto>> GetListByRoleIdAsync(Guid roleId)
        {
            var userids = await DbContext.Queryable<UserRole>()
           .Where(t => t.RoleId == roleId)
           .Select(a => a.UserId)
           .ToListAsync();

            var users = await DbContext.Queryable<User>()
                .Where(t => userids.Contains(t.Id))
                .ToListAsync();

            return Mapper.Map<IEnumerable<UserReadDto>>(users);
        }
    }
}