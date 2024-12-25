using HgznMes.Application.Auth;
using HgznMes.Application.Dtos;
using HgznMes.Application.Services.Base;
using HgznMes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HgznMes.WebApi.Controllers
{
    /// <summary>
    ///     角色资源
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = ManagedResource.Role)]
    public class RoleController : ControllerBase
    {
        /// <summary>
        ///     注入
        /// </summary>
        /// <param name="roleService"></param>
        public RoleController(
            IRoleService roleService
            )
        {
            _roleService = roleService;
        }

        private readonly IRoleService _roleService;

        /// <summary>
        ///     获取角色信息
        ///     auth: super
        /// </summary>
        /// <param name="roleId">角色guid</param>
        /// <returns>角色信息</returns>
        [HttpGet]
        [Route("{roleId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"{ManagedResource.Role}.{ManagedAction.Get}.Id")]
        public async Task<ResponseWrapper<RoleReadDto?>> GetRole(Guid roleId) =>
            (await _roleService.GetRoleAsync(roleId)).Wrap();

        /// <summary>
        ///     获取所有角色
        ///     auth: super
        /// </summary>
        /// <returns>角色列表</returns>
        [HttpGet]
        [Route("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"{ManagedResource.Role}.{ManagedAction.Get}.All")]
        public async Task<ResponseWrapper<IEnumerable<RoleReadDto>>> GetRoles() =>
            (await _roleService.GetRolesAsync()).Wrap();

        /// <summary>
        ///     创建新角色
        ///     auth: super
        /// </summary>
        /// <param name="roleDto">角色必填字段</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"{ManagedResource.Role}.{ManagedAction.Add}.New")]
        public async Task<ResponseWrapper<RoleReadDto?>> CreateRole(RoleCreateDto roleDto) =>
            (await _roleService.CreateRoleAsync(roleDto)).Wrap();

        /// <summary>
        ///     编辑角色权限范围
        ///     auth: super
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <param name="menus">权限列表</param>
        /// <returns>已编辑权限数</returns>
        [HttpPut]
        [Route("{roleId:guid}/menus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"{ManagedResource.Role}.{ManagedAction.Put}.Scopes")]
        public async Task<ResponseWrapper<int>> ModifyRoleMenu(Guid roleId, List<Guid> menus) =>
            (await _roleService.ModifyRoleMenuAsync(roleId, menus)).Wrap();

        /// <summary>
        ///     获取支持的权限范围
        ///     auth: admin
        /// </summary>
        /// <returns>权限列表</returns>
        [HttpGet]
        [Route("scopes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"{ManagedResource.Role}.{ManagedAction.Get}.Scopes")]
        public ResponseWrapper<IEnumerable<ScopeDefReadDto>> GetSupportedScopes() =>
            _roleService.GetScopes().Wrap();
    }
}