using System.Text;
using System.Text.Json;
using Hangfire;
using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Hgzn.Mes.Infrastructure.Utilities;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlSugar;
using StackExchange.Redis;

namespace Hgzn.Mes.WebApi.Controllers.System;

/// <summary>
///     测试路由
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AAAtestContriller:ControllerBase
{
    private readonly ISqlSugarClient _client;
    private readonly IMqttExplorer _mqttExplorer;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public AAAtestContriller(ISqlSugarClient client, IMqttExplorer mqttExplorer , IConnectionMultiplexer connectionMultiplexer)
    {
        _client = client;
        _mqttExplorer = mqttExplorer;
        _connectionMultiplexer = connectionMultiplexer;
    }

    [HttpGet]
    [Route("equip_start")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task equipStart()
    {
        TestController tt = new TestController(_client, _mqttExplorer,_connectionMultiplexer);
        await tt.equipStart();
    }
}