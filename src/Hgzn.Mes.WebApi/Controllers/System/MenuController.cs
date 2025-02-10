using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.System
{
    /// <summary>
    ///     菜单
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        private readonly IMenuService _menuService;

        /// <summary>
        ///     获取树形菜单
        ///     auth: anonymous
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("tree/curr-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ResponseWrapper<IEnumerable<MenuReaderRouterDto>>> GetCurrentUserMenusAsTree() =>
            (await _menuService.GetCurrentUserMenusAsTreeAsync(HttpContext.User.Claims)).Wrap();

        /// <summary>
        ///     获取树形菜单
        ///     auth: anonymous
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("tree/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ResponseWrapper<IEnumerable<MenuReadDto>>> GetRootMenusAsTree() =>
            (await _menuService.GetRootMenusAsTreeAsync()).Wrap();
        /// <summary>
        ///     获取树形菜单
        ///     auth: anonymous
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ResponseWrapper<IEnumerable<MenuReadDto>>> GetListAsync() =>
            (await _menuService.GetListAsync()).Wrap();
        /// <summary>
        ///     删除菜单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ResponseWrapper<int>> DeleteMenu(Guid id, bool force = false) =>
            (await _menuService.DeleteAsync(id)).Wrap();

        /// <summary>
        ///     添加菜单
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ResponseWrapper<MenuReadDto>> CreateMenuAsync(MenuCreateDto dto) =>
            (await _menuService.CreateAsync(dto)).Wrap()!;
        
        /// <summary>
        ///     获取菜单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ResponseWrapper<MenuReadDto>> GetMenuAsync(Guid id) =>
            (await _menuService.GetAsync(id)).Wrap();
        /// <summary>
        ///     获取菜单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ResponseWrapper<MenuReadDto>> UpdateMenuAsync(Guid id,MenuUpdateDto dto) =>
            (await _menuService.UpdateAsync(id,dto)).Wrap();
        
        /// <summary>
        ///     获取菜单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("role-id/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ResponseWrapper<IEnumerable<MenuReadDto>>> GetMenuByRoleIdAsync(Guid id) =>
            (await _menuService.GetMenuByRoleIdAsync(id)).Wrap();
    }
}