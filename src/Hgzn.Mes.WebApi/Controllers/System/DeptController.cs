using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.System;

[Route("api/[controller]")]
[ApiController]
public class DeptController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DeptController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    /// <summary>
    ///     获取部门列表
    /// </summary>
    /// <param name="queryDto">注册必要字段</param>
    /// <returns>成功注册的用户/null</returns>
    [HttpPost]
    [Route("list")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:user:{ScopeMethodType.List}")]
    public async Task<ResponseWrapper<IEnumerable<DepartmentReadDto>>> GetListAsync([FromQuery]DepartmentQueryDto queryDto) =>
        (await _departmentService.GetListAsync(queryDto)).Wrap();

    /// <summary>
    ///     获取下级部门列表
    /// </summary>
    /// <param name="id">注册必要字段</param>
    /// <returns>deptId/null</returns>
    [HttpGet]
    [Route("child-list/{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:user:{ScopeMethodType.List}")]
    public async Task<ResponseWrapper<List<Guid>>> GetChildListAsync(Guid id) =>
        (await _departmentService.GetChildListAsync(id)).Wrap();

    /// <summary>
    ///     分页获取部门列表
    /// </summary>
    /// <param name="queryDto">注册必要字段</param>
    /// <returns>成功注册的用户/null</returns>
    [HttpPost]
    [Route("page")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:dept:{ScopeMethodType.List}")]
    public async Task<ResponseWrapper<PaginatedList<DepartmentReadDto>>> GetPaginatedListAsync(DepartmentQueryDto queryDto) =>
        (await _departmentService.GetPaginatedListAsync(queryDto)).Wrap();

    /// <summary>
    /// 修改数据
    /// </summary>
    /// <param name="id">主键</param>
    /// <param name="dto">修改实体</param>
    /// <returns></returns>
    [HttpPut]
    [Route("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:dept:{ScopeMethodType.Edit}")]
    public async Task<ResponseWrapper<DepartmentReadDto>> UpdateAsync(Guid id, DepartmentUpdateDto dto) =>
        (await _departmentService.UpdateAsync(id, dto)).Wrap();

    /// <summary>
    /// 修改数据
    /// </summary>
    /// <param name="dto">修改实体</param>
    /// <returns></returns>
    [HttpPost]
    [Route("")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:dept:{ScopeMethodType.Add}")]
    public async Task<ResponseWrapper<DepartmentReadDto>> CreateAsync(DepartmentCreateDto dto) =>
        (await _departmentService.CreateAsync(dto)).Wrap();
    
    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="id">主键</param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:dept:{ScopeMethodType.Remove}")]
    public async Task<ResponseWrapper<int>> DeleteAsync(Guid id) =>
        (await _departmentService.DeleteAsync(id)).Wrap();
    
    /// <summary>
    /// 获取实体
    /// </summary>
    /// <param name="id">主键</param>
    /// <returns></returns>
    [HttpGet]
    [Route("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = $"system:dept:{ScopeMethodType.Query}")]
    public async Task<ResponseWrapper<DepartmentReadDto>> GetAsync(Guid id) =>
        (await _departmentService.GetAsync(id)).Wrap();
}