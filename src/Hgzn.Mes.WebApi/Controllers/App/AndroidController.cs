using System.Linq.Expressions;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization.Json;
using System.Text.Json.Nodes;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Services;
using SqlSugar;

namespace Hgzn.Mes.WebApi.Controllers.App;

/// <summary>
/// 安卓设备使用
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AndroidController : ControllerBase
{
    private readonly IEquipLedgerService _equipLedgerService;
    private readonly IEquipLedgerHistoryService _equipLedgerHistoryService;
    private readonly ILocationLabelService _locationLabelService;
    private readonly ILogger<AndroidController> _logger;
    private readonly ISqlSugarClient _sqlSugarClient;
    private readonly ISplitTableQueryService _splitTableQueryService;

    public AndroidController(
        IEquipLedgerService equipLedgerService,
        IEquipLedgerHistoryService equipLedgerHistoryService,
        ILocationLabelService locationLabelService,
        ILogger<AndroidController> logger,
        ISqlSugarClient sqlSugarClient,
        ISplitTableQueryService splitTableQueryService)
    {
        _equipLedgerService = equipLedgerService;
        _equipLedgerHistoryService = equipLedgerHistoryService;
        _locationLabelService = locationLabelService;
        _logger = logger;
        _sqlSugarClient = sqlSugarClient;
        _splitTableQueryService = splitTableQueryService;
    }

    /// <summary>
    ///     手持端获取设备rfid绑定关系
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("label/equip")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ResponseWrapper<PaginatedList<EquipLocationLabelReadDto>>> GetEquipLabelAsync(int pageIndex = 1,
        int pageSize = 100)
    {
        var list = await _locationLabelService.GetEquipLabelAsync(pageIndex, pageSize);
        return list.Wrap();
    }

    /// <summary>
    ///     获取所有绑定过资产编号的数据
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("label/equip-all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ResponseWrapper<PaginatedList<EquipLedgerSearchReadDto>>> GetEquipAllAsync(int pageIndex = 1,
        int pageSize = 100)
    {
        var list = await _locationLabelService.GetEquipBindLabelAsync(pageIndex, pageSize);
        return list.Wrap();
    }

    /// <summary>
    ///     手持端获取房间rfid绑定关系
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("label/room")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ResponseWrapper<PaginatedList<RoomLocationLabelReadDto>>> GetRoomLabelAsync(int pageIndex = 1,
        int pageSize = 100) =>
        (await _locationLabelService.GetRoomLabelAsync(pageIndex, pageSize)).Wrap();

    /// <summary>
    /// 手持端获取搜索目标
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ResponseWrapper<PaginatedList<EquipLedgerSearchReadDto>>> GetAppSearchAsync(int pageIndex = 1,
        int pageSize = 100) =>
        (await _equipLedgerService.GetAppSearchAsync(pageIndex, pageSize)).Wrap();

    /// <summary>
    /// 手持端巡检记录上传
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("store")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ResponseWrapper<int>> PostAppStoreAsync(List<EquipLedgerHistoryCreateDto> list)
    {
        int equipList = 0;
        try
        {
            var tasks = list.Select(async item =>
            {
                _logger.LogInformation($"导入盘点设备信息:{ObjectToJson(item)}");
                var ledgers = await _equipLedgerService.GetListByAssetNumberAsync(item.AssetNumber);
                foreach (var el in ledgers)
                {
                    item.EquipCode = el.EquipCode;
                    item.EquipId = el.Id;
                }

                _logger.LogInformation($"获取到设备的code是:{item.EquipCode}");
                _logger.LogInformation($"获取到设备的Id是:{item.EquipId}");
            });
            await Task.WhenAll(tasks);

            var result = (await _equipLedgerHistoryService.CreateAsync(list)).Wrap();
            _logger.LogInformation($"添加历史记录成功");
            var dictionary = list.Where(t => t.RoomId != null).ToDictionary(t => t.AssetNumber, s => s.RoomId.Value);
            equipList = await _equipLedgerService.UpdateEquipRoomId(dictionary);
            _logger.LogInformation($"盘点设备,更新设备房间ID成功");
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"盘点失败,原因是{ex.Message}");
        }

        return equipList.Wrap();
    }

    /// <summary>
    /// 手持端上传搜索结果
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ResponseWrapper<int>> PostAppSearchAsync(List<EquipLedgerHistoryCreateDto> list)
    {
        var tasks = list.Select(async item =>
        {
            _logger.LogInformation($"导入找到丢失的设备信息:{ObjectToJson(item)}");
            await _equipLedgerService.SetEquipExistByAssetNumber(item.AssetNumber);
            var ledgers = await _equipLedgerService.GetListByAssetNumberAsync(item.AssetNumber);
            foreach (var el in ledgers)
            {
                el.Id = new Guid();
                item.EquipCode = el.EquipCode;
                item.EquipId = el.Id;
                item.RoomId = el.RoomId;
            }

            _logger.LogInformation($"获取到设备的code是:{item.EquipCode}");
            _logger.LogInformation($"获取到设备的Id是:{item.EquipId}");
        });
        await Task.WhenAll(tasks);
        _logger.LogInformation($"修改丢失为正常状态成功");
        ResponseWrapper<int> ret = default;
        try
        {
            ret = (await _equipLedgerHistoryService.CreateAsync(list)).Wrap();
            _logger.LogInformation($"添加历史记录成功");
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"添加历史记录失败,原因是{ex.Message}");
        }

        return ret;
    }

    public static string ObjectToJson(object obj)
    {
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
        using (MemoryStream stream = new MemoryStream())
        {
            serializer.WriteObject(stream, obj);
            return global::System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    [HttpPost]
    [Route("test")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<int> PostAppStoreAsync(Receive data)
    {
        return await _sqlSugarClient.Insertable(new Receive
        {
            Content = data.Content,
            SimuTestSysld = data.SimuTestSysld,
            DevTypeld = data.DevTypeld,
            Compld = data.Compld,
            CreateTime = DateTime.Parse(data.Time)
        }).SplitTable().ExecuteCommandAsync();
    }

    [HttpGet]
    [Route("test1")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<object?> GetAppStoreAsync()
    {
        // 创建查询条件
        var startTime = new DateTime(2085, 8, 1);
        var endTime = new DateTime(2085, 9, 30, 23, 59, 59);

        var text = new Receive()
        {
            SimuTestSysld = 9,
            DevTypeld = 9,
            Compld = "laboris5168"
        };
        // 执行查询
        return await _splitTableQueryService.QueryByTimeRangeAsync(
            startTime,
            null,
            text
        );
    }
}