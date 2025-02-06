using GDotnet.Reader.Api.DAL;
using GDotnet.Reader.Api.Protocol.Gx;
using Hgzn.Mes.Iot.ProtocolManager.Protocols;
using Hgzn.Mes.Iot.Redis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

namespace Hgzn.Mes.Iot.ProtocolManager.RfidReader;

public class RfidReaderManages : BaseProtocolManager, IEquipManager
{
    /// <summary>
    /// 连接参数
    /// </summary>
    private ProtocolRfidClient? _parameters;
    private GClient? _client;
    private string? RfidNumber { get; set; }
    private readonly ILogger _logger;
    private readonly RedisService _redisService;
    private const string RedisCode = "EncapedTagEquipId";
    public RfidReaderManages(string parameters,RedisService redisService)
    {
        _logger = NullLogger.Instance;
        this._redisService = redisService;
        UpdateConnectionParameter(parameters).Wait();
    }
    public sealed override async Task UpdateConnectionParameter(string parameter)
    {
        _parameters = JsonConvert.DeserializeObject<ProtocolRfidClient>(parameter);
    }

    protected override async Task BaseConnectionAsync()
    {
        try
        {
            if (_parameters == null)
                throw new NullReferenceException("RfidReaderManager is null");
            _client = new GClient();
            await Task.Run(() =>
            {
                if (_client.OpenTcp(_parameters.Address + ":" + _parameters.Port, 5000,
                        out var status) && status == eConnectionAttemptEventStatusType.OK)
                {
                    _client.OnEncapedTagEpcLog = GetEncapedTagEpcLog;
                    // 获得读写器信息
                    var readerInfo = new MsgAppGetReaderInfo();
                    _client.SendSynMsg(readerInfo);
                    if (readerInfo.RtCode == 0)
                    {
                        Console.WriteLine("ger reader info success");
                        RfidNumber = readerInfo.Imei;
                        StartReadingTag(_client);
                    }
                }
            }, CancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            _client = null;
            Console.WriteLine(e);
            await DisConnectionAsync();
            throw new ApplicationException(e.Message);
        }
    }

    protected override async Task BaseDisConnectionAsync()
    {
        foreach (var cancelToken in CancelTokens)
        {
            await cancelToken.Value.CancelAsync();
        }

        _client?.Close();
    }

    public override async Task StartAsync()
    {
        await ConnectionAsync();
    }

    public override Task StartAsync(Guid dataPointId)
    {
        return Task.CompletedTask;
    }


    public override Task UpdateEquipConnectForward(List<Guid> targetIds)
    {
        return Task.CompletedTask;
    }

    public override Task<bool> IsConnectedAsync()
    {
        return Task.FromResult(_client != null);
    }
    
    
    private void StartReadingTag(GClient client)
    {
        // 停止指令，空闲态
        var msgBaseStop = new MsgBaseStop();
        client.SendSynMsg(msgBaseStop);
        if (0 != msgBaseStop.RtCode)
        {
            throw new Exception("epc stop error.");
        }

        // 4个天线读卡, 读取EPC数据区以及TID数据区
        var msgBaseInventoryEpc = new MsgBaseInventoryEpc
        {
            AntennaEnable = (ushort)(eAntennaNo._1 | eAntennaNo._2 |
                                     eAntennaNo._3 | eAntennaNo._4),
            InventoryMode = (byte)eInventoryMode.Inventory,
            ReadTid = new ParamEpcReadTid
            {
                Mode = (byte)eParamTidMode.Auto,
                Len = 6
            }
        };
        client.SendSynMsg(msgBaseInventoryEpc);
        if (0 != msgBaseInventoryEpc.RtCode)
        {
            throw new Exception("inventory epc error.");
        }
    }
    
    
    private async void GetEncapedTagEpcLog(EncapedLogBaseEpcInfo msg)
    {
        try
        {
            //当前连接状态是写入标签还是读数据
            var tid = msg.logBaseEpcInfo.Tid;
            var equipId = msg.logBaseEpcInfo.Userdata;
            var state = EquipConnect.CollectionExtension;
            Guid? equip = string.IsNullOrEmpty(equipId) ? null : Guid.Parse(equipId);
            if (int.TryParse(state.ToString(), out var result) && result != 0)
            {
                equip = EquipConnect.EquipId;
            }

            //如果设备Id不存在
            if (equip == null)
            {
                throw new AggregateException("equip id is invalid.");
            }
            //只在采集数据的时候进行多标签控制
            if (result == 0)
            {
                // 获取缓存中的设备数据
                var redisData = await _redisService.GetRedisDataAsync(RedisCode + equip);

                // 如果缓存中有数据并且能够解析为1，则直接返回
                if (redisData != null && 
                    int.TryParse((string?)redisData, out var equipResult) && equipResult == 1)
                {
                    return; // 设备已经有效，不做进一步处理
                }
                //设置过期时间,5秒内，相同设备的标签，只会取其中一个
                await _redisService.SetRedisDataOptionsAsync(RedisCode+equip, 1, TimeSpan.FromMinutes(5));
            }
            
            //发布设备移动的事件
            // await LocalEventBus.PublishAsync(new EquipMoveRfidEventArgs()
            // {
            //     RfidNumber = SerialNumber,
            //     RfidTid = tid,
            //     EquipId = equip.Value,
            //     Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            //     State = result
            // });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _logger.LogError(e, "GetEncapedTagEpcLog");
        }
    }
}