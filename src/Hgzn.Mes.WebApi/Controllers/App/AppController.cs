using Hgzn.Mes.Application.Main.Dtos.App;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.App.IService;
using Hgzn.Mes.Application.Main.Services.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.Shared.Extensions;
using Hgzn.Mes.Infrastructure.Utilities;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.HPSF;
using NPOI.HSSF.Util;
using NPOI.SS.Formula.Functions;
using SkiaSharp;
using SqlSugar;
using StackExchange.Redis;
using System;
using static NPOI.HSSF.Util.HSSFColor;

namespace Hgzn.Mes.WebApi.Controllers.App;


/// <summary>
/// 大屏使用
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AppController : ControllerBase
{
    private IAppService _appService;
    public AppController(IAppService appService)
    {
        _appService = appService;
    }

    /// <summary>
    /// 获取试验系统主页数据
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("test/list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ResponseWrapper<ShowSystemHomeDataDto>> GetTestListAsync()
    {
        ShowSystemHomeDataDto testRead = await _appService.GetTestListAsync();
        return testRead.Wrap();
    }

    /// <summary>
    /// 获取试验系统详情页数据
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("test/detail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ResponseWrapper<ShowSystemDetailDto>> GetTestDetailAsync(ShowSystemDetailQueryDto showSystemDetailQueryDto)
    {
        ShowSystemDetailDto read = await _appService.GetTestDetailAsync(showSystemDetailQueryDto);
        return read.Wrap();
    }
}