using HgznMes.Application.Auth.Requirements;
using HgznMes.Application.Services.Base;
using HgznMes.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using HgznMes.Domain.Entities.Login;


namespace BcsJiaer.Application.Auth.AuthHandler
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
        private readonly ILogger _logger;

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRequireScope requirement)
        {
            var dict = context.User.Claims.ToDictionary(key => key.Type, value => value.Value);
            if (!dict.TryGetValue(CustomClaimsType.UserId, out var uId) ||
                !dict.TryGetValue(CustomClaimsType.RoleId, out var rId))
            {
                _logger.LogWarning("invalid token claims");
                context.Fail(new AuthorizationFailureReason(this, "invalid claims"));
                return;
            }
            var userId = Guid.Parse(uId);
            var roleId = Guid.Parse(rId);

            if (roleId == Role.SuperRole.Id)
            {
                context.Succeed(requirement);
                return;
            }

            var role = await _roleService.GetRoleAsync(roleId);
            if (role is null)
            {
                _logger.LogWarning("a token with invalid role");
                context.Fail(new AuthorizationFailureReason(this, "unknow role"));
                return;
            }

            if (role.Scopes.Any(s => requirement.Scope.Contains(s.Name)))
            {
                _logger.LogTrace("scope match success");
                context.Succeed(requirement);
                return;
            }
            context.Fail(new AuthorizationFailureReason(this, "no authorization"));
        }
    }
}