using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers
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
        ///     删除菜单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("id/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ResponseWrapper<int>> DeleteMenu(Guid id, bool force = false) =>
            (await _menuService.DeleteMenuAsync(id, force)).Wrap();

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
    }
}