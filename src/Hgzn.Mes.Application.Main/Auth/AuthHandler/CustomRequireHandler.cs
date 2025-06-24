using Hgzn.Mes.Application.Main.Auth.Requirements;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Extensions;
using Hgzn.Mes.Infrastructure.Utilities.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Hgzn.Mes.Application.Main.Auth.AuthHandler
{
    public class CustomRequireHandler : AuthorizationHandler<CustomRequireScope>
    {
        public CustomRequireHandler(
            IRoleService roleService,
            ILogger<CustomRequireHandler> logger
            )
        {
            _roleService = roleService;
            _logger = logger;
        }

        private readonly IRoleService _roleService;
        private readonly ILogger<CustomRequireHandler> _logger;

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRequireScope requirement)
        {
            var dict = context.User.Claims.ToDictionary(key => key.Type, value => value.Value);
            if (!dict.TryGetValue(ClaimType.UserId, out var uId) ||
                !dict.TryGetValue(ClaimType.RoleId, out var rId))
            {
                _logger.LogWarning("invalid token claims");
                context.Fail(new AuthorizationFailureReason(this, "invalid claims"));
                return;
            }
            var userId = Guid.Parse(uId);
            var roleId = await rId.Split(",").Select(Guid.Parse).ToListAsync();
            //var roleId = Guid.Parse(rId);

            if (roleId.Contains(Role.SuperRole.Id) || roleId.Contains(Role.DevRole.Id) ||
                requirement.Scope.Contains("list") || requirement.Scope.Contains("query"))
            {
                context.Succeed(requirement);
                return;
            }
            //使用缓存
            
            var scopeCode = await _roleService.GetRoleMenuCodeAsync(roleId);
            // if (role is null)
            // {
            //     _logger.LogWarning("a token with invalid role");
            //     context.Fail(new AuthorizationFailureReason(this, "unknow role"));
            //     return;
            // }
            if (scopeCode.Any(s => requirement.Scope.Contains(s)))
            {
                _logger.LogTrace("scope match success");
                context.Succeed(requirement);
                return;
            }
            context.Fail(new AuthorizationFailureReason(this, "no authorization"));
        }
    }
}