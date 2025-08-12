using Hgzn.Mes.Application.Main.Dtos.App;
using Hgzn.Mes.Application.Main.Services.App.IService;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemIOFile = System.IO.File;

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

    /// <summary>
    /// 下载大屏需要的摄像头插件
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("download/cameraplugin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<IActionResult> DownloadCameraPlugin()
    {
        byte[] content = [];
        string fileName = "";
        string filePath = Path.Combine(Environment.CurrentDirectory, "Resources", "HCWebSDKPluginsUserSetup.exe");
        if (SystemIOFile.Exists(filePath))
        {
            content = await SystemIOFile.ReadAllBytesAsync(filePath);
            fileName = "HCWebSDKPluginsUserSetup.exe";
        }
        Response.Headers.ContentDisposition = $"inline; filename={fileName}";
        return File(content, "application/octet-stream");
    }
}