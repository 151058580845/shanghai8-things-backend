using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.System
{
    /// <summary>
    ///     角色资源
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
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
        [Authorize(Policy = $"system:role:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<RoleReadDto>> GetRole(Guid roleId) =>
            (await _roleService.GetAsync(roleId)).Wrap();

        /// <summary>
        ///     获取所有角色
        ///     auth: super
        /// </summary>
        /// <returns>角色列表</returns>
        [HttpPost]
        [Route("list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:role:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<IEnumerable<RoleReadDto>>> GetRolesList(RoleQueryDto? queryDto) =>
            (await _roleService.GetListAsync(queryDto)).Wrap();
        /// <summary>
        ///     获取所有角色
        ///     auth: super
        /// </summary>
        /// <returns>角色列表</returns>
        [HttpPost]
        [Route("page")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:role:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<PaginatedList<RoleReadDto>>> GetPaginatedListAsync(RoleQueryDto queryDto) =>
            (await _roleService.GetPaginatedListAsync(queryDto)).Wrap();
        /// <summary>
        ///     创建新角色
        ///     auth: super
        /// </summary>
        /// <param name="roleDto">角色必填字段</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:role:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<RoleReadDto>> CreateRole(RoleCreateDto roleDto) =>
            (await _roleService.CreateAsync(roleDto)).Wrap();

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
        [Authorize(Policy = $"system:role:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<bool>> ModifyRoleMenu(Guid roleId, List<Guid> menus) =>
            (await _roleService.ModifyRoleMenuAsync(roleId, menus)).Wrap();
        [HttpPut]
        [Route("{roleId:guid}/users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:role:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<bool>> ModifyRoleUser(Guid roleId, List<Guid> users) =>
            (await _roleService.ModifyRoleUserAsync(roleId, users)).Wrap();
        /// <summary>
        ///     获取支持的权限范围
        ///     auth: admin
        /// </summary>
        /// <returns>权限列表</returns>
        [HttpGet]
        [Route("scopes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[Authorize(Policy = $"system:role:{ScopeMethodType.Query}")]
        public ResponseWrapper<IEnumerable<ScopeDefReadDto>> GetSupportedScopes() =>
            _roleService.GetScopes().Wrap();


        /// <summary>
        ///     删除角色
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("{roleId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:role:{ScopeMethodType.Remove}")]
        public async Task<ResponseWrapper<int>> DeleteAsync(Guid roleId) =>
          ( await _roleService.DeleteAsync(roleId)).Wrap();


        /// <summary>
        /// 修改角色
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("{roleId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:role:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<RoleReadDto>> UpdateAsync(Guid roleId, RoleUpdateDto input) =>
              (await _roleService.UpdateAsync(roleId, input)).Wrap();
        
        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:guid}/{state:bool}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:role:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<RoleReadDto>> UpdateStateAsync(Guid id, bool state) =>
            (await _roleService.UpdateStateAsync(id, state)).Wrap();

        /// <summary>
        /// 获取菜单列表根据角色id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}/menus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ResponseWrapper<IEnumerable<MenuReadDto>>> GetRoleMenus(Guid id) =>
        (await _roleService.GetRoleMenusAsync(id)).Wrap();


    }
}