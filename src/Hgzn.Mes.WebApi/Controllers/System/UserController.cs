using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.System
{
    /// <summary>
    ///     用户资源
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        /// <summary>
        ///     注入服务
        /// </summary>
        /// <param name="userService">用户服务</param>
        public UserController(
            IUserService userService
            )
        {
            _userService = userService;
        }

        private readonly IUserService _userService;

        /// <summary>
        ///     获取当前的用户
        ///     auth: super
        /// </summary>
        /// <returns>用户详情</returns>
        [HttpGet]
        [Authorize]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ResponseWrapper<UserScopeReadDto>> GetCurrentUser() =>
            (await _userService.GetCurrentUserAsync(HttpContext.User.Claims)).Wrap();

        /// <summary>
        ///     获取指定Id的用户
        ///     auth: super
        /// </summary>
        /// <param name="userId">用户guid</param>
        /// <returns>用户详情</returns>
        [HttpGet]
        [Authorize]
        [Route("{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:user:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<UserReadDto>> GetUser(Guid userId) =>
            (await _userService.GetAsync(userId)).Wrap();

        // [HttpGet]
        // [Authorize]
        // [Route("role-id/{roleId:guid}")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [Authorize(Policy = $"system:user:{ScopeMethodType.Query}")]
        // public async Task<ResponseWrapper<IEnumerable<UserReadDto>>> GetUserListByRoleId(Guid roleId) =>
        //     (await _userService.GetListByRoleIdAsync(roleId)).Wrap();
        /// <summary>
        /// 获取用户列表根据角色id
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{roleId:guid}/ByRoleId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ResponseWrapper<IEnumerable<UserReadDto>>> GetMenuListByRoleidAsync(Guid roleId) =>
            (await _userService.GetListByRoleIdAsync(roleId)).Wrap();
        /// <summary>
        ///     模糊匹配用户名和昵称
        ///     auth: super
        /// </summary>
        /// <param name="dto">用户名，昵称手机号</param>
        /// <returns>匹配的用户列表</returns>
        [HttpPost]
        [Route("list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:user:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<IEnumerable<UserReadDto>>> GetUsers(UserQueryDto dto) =>
            (await _userService.GetListAsync(dto)).Wrap();
        /// <summary>
        ///     模糊匹配用户名和昵称
        ///     auth: super
        /// </summary>
        /// <param name="dto">用户名，昵称手机号</param>
        /// <returns>匹配的用户列表</returns>
        [HttpPost]
        [Route("page")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:user:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<PaginatedList<UserReadDto>>> GetPaginatedListAsync(UserQueryDto dto) =>
            (await _userService.GetPaginatedListAsync(dto)).Wrap();
        /// <summary>
        ///     删除用户
        ///     auth: super
        /// </summary>
        /// <param name="userId">用户guid</param>
        /// <returns>已删除用户数</returns>
        [HttpDelete]
        [Route("{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:user:{ScopeMethodType.Remove}")]
        public async Task<ResponseWrapper<int>> Delete(Guid userId) =>
            (await _userService.DeleteAsync(userId)).Wrap();
        /// <summary>
        ///     删除用户
        ///     auth: super
        /// </summary>
        /// <param name="userId">用户guid</param>
        /// <returns>已删除用户数</returns>
        [HttpPost]
        [Route("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:user:{ScopeMethodType.Remove}")]
        public async Task<ResponseWrapper<UserReadDto>> Delete(UserRegisterDto dto) =>
            (await _userService.CreateAsync(dto)).Wrap();
        /// <summary>
        ///     切换角色
        ///     auth: super
        /// </summary>
        /// <param name="userId">用户guid</param>
        /// <param name="roleIds">角色guid</param>
        /// <returns>用户更改后的详情</returns>
        [HttpPost]
        [Route("{userId:guid}/role")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:user:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<UserReadDto>> ModifyRole(Guid userId, IEnumerable<Guid> roleIds) =>
            (await _userService.ChangeRoleAsync(userId, roleIds)).Wrap();

        /// <summary>
        ///     更换自己的密码
        ///     auth: anonymous
        /// </summary>
        /// <param name="passwordDto">用于验证用户身份</param>
        /// <returns>更换密码状态</returns>
        [HttpPut]
        [Route("pwd")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:user:{ScopeMethodType.Edit}")]

        public async Task<ResponseWrapper<int>> ChangePassword(ChangePasswordDto passwordDto) =>
            (await _userService.ChangePasswordAsync(passwordDto)).Wrap();


        
        /// <summary>
        ///     重置某个用户的密码
        ///     auth: admin
        /// </summary>
        /// <param name="userId">用户guid</param>
        /// <param name="dto"></param>
        /// <returns>重置状态</returns>
        [HttpPut]
        [Route("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"system:user:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<UserReadDto>> ResetPassword(Guid userId,UserUpdateDto dto) =>
            (await _userService.UpdateAsync(userId,dto)).Wrap();
        
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
        [Authorize(Policy = $"system:user:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<UserReadDto>> UpdateStateAsync(Guid id, bool state) =>
            (await _userService.UpdateStateAsync(id, state)).Wrap();
    }
}