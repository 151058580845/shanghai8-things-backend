using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Entities.Equip.EquipDataPoint;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Hgzn.Mes.Infrastructure.Utilities;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Text;
using System.Text.Json;

namespace Hgzn.Mes.Application.Main.Services.Equip
{
    internal class EquipDataPointService : SugarCrudAppService<
        EquipDataPoint, Guid,
        EquipDataPointReadDto, EquipDataPointQueryDto,
        EquipDataPointCreateDto, EquipDataPointUpdateDto>, IEquipDataPointService
    {
        private IMqttExplorer _mqttExplorer;
        private ISqlSugarClient _sugarClient;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public EquipDataPointService(IMqttExplorer mqttExplorer,
            IConnectionMultiplexer connectionMultiplexer,
            ISqlSugarClient sugarClient)
        {
            this._mqttExplorer = mqttExplorer;
            this._connectionMultiplexer = connectionMultiplexer;
            this._sugarClient = sugarClient;
        }

        public override async Task<IEnumerable<EquipDataPointReadDto>> GetListAsync(
            EquipDataPointQueryDto? queryDto = null)
        {
            var entities = await Queryable
                .WhereIF(queryDto != null && queryDto.State == null, t => t.State == queryDto!.State)
                .WhereIF(queryDto != null && !string.IsNullOrEmpty(queryDto.Code),
                    t => t.Code.Contains(queryDto!.Code!))
                .ToListAsync();

            var outputs = Mapper.Map<IEnumerable<EquipDataPointReadDto>>(entities);

            return outputs;
        }

        public override async Task<PaginatedList<EquipDataPointReadDto>> GetPaginatedListAsync(
            EquipDataPointQueryDto queryDto)
        {
            PaginatedList<EquipDataPoint> entites = await Queryable
                .Includes(edp => edp.Connection)
                .WhereIF(queryDto.State != null, t => t.State == queryDto.State)
                .WhereIF(!string.IsNullOrEmpty(queryDto.Code), t => t.Code.Contains(queryDto!.Code!))
                .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
            foreach (EquipDataPoint entity in entites.Items)
            {
                //获取连接状态
                IDatabase database = _connectionMultiplexer.GetDatabase();
                bool isConnect = await database.StringGetAsync(string.Format(CacheKeyFormatter.EquipDataPointOperationStatus, entity.Id)) == 1 ? true : false;
                entity.CollectStatus = isConnect ? "Pending" : "None";
            }
            PaginatedList<EquipDataPointReadDto> outputs = Mapper.Map<PaginatedList<EquipDataPointReadDto>>(entites);

            return outputs;
        }

        /// <summary>
        /// 开始采集
        /// </summary>
        /// <param name="id">点位Id</param>
        /// <returns></returns>
        public async Task PutStartConnect(Guid id)
        {
            EquipDataPoint equipDataPoint = await Queryable.Where(it => it.Id == id)
           .Includes(x => x.Connection, c => c!.EquipLedger, el => el.EquipType)
           .FirstAsync();

            if (equipDataPoint == null) return;
            EquipConnect connect = equipDataPoint.Connection!;

            var startInfo = new ConnInfo
            {
                ConnType = connect.ProtocolEnum,
                ConnString = connect.ConnectStr,
                Type = CmdType.Collection,
                StateType = ConnStateType.Run,
            };

            var topic = IotTopicBuilder.CreateIotBuilder()
                    .WithPrefix(TopicType.Iot)
                    .WithDirection(MqttDirection.Down)
                    .WithTag(MqttTag.Cmd)
                    .WithDeviceType(connect.EquipLedger?.EquipType?.ProtocolEnum ??
            throw new ArgumentNullException("equip type not exist"))
                    .WithUri(id.ToString()).Build();
            if (await _mqttExplorer.IsConnectedAsync())
            {
                var payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(startInfo));
                
                // 使用支持断点续传的发布方法
                if (_mqttExplorer is Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport.IMqttExplorerWithOffline mqttWithOffline)
                {
                    await mqttWithOffline.PublishWithOfflineSupportAsync(topic, payload, priority: 0, maxRetryCount: 3);
                }
                else
                {
                    await _mqttExplorer.PublishAsync(topic, payload);
                }
            }
        }

        /// <summary>
        /// 停止采集
        /// </summary>
        /// <param name="id">点位Id</param>
        /// <returns></returns>
        public async Task PutStopConnect(Guid id)
        {
            EquipDataPoint equipDataPoint = await Queryable.Where(it => it.Id == id)
           .Includes(x => x.Connection, c => c!.EquipLedger, el => el.EquipType)
           .FirstAsync();
            if (equipDataPoint == null) return;
            EquipConnect connect = equipDataPoint.Connection!;

            IotTopicBuilder iotTopicBuilder = IotTopicBuilder.CreateIotBuilder()
                    .WithPrefix(TopicType.Iot)
                    .WithDirection(MqttDirection.Down)
                    .WithTag(MqttTag.Cmd)
                    .WithDeviceType(connect.EquipLedger?.EquipType?.ProtocolEnum ??
                        throw new ArgumentNullException("equip type not exist"))
                    .WithUri(id.ToString());

            string topic = iotTopicBuilder.Build();

            var stopInfo = new ConnInfo
            {
                ConnType = connect.ProtocolEnum,
                ConnString = connect.ConnectStr,
                Type = CmdType.Collection,
                StateType = ConnStateType.Stop,
            };

            var payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(stopInfo));
            
            // 使用支持断点续传的发布方法
            if (_mqttExplorer is Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport.IMqttExplorerWithOffline mqttWithOffline)
            {
                await mqttWithOffline.PublishWithOfflineSupportAsync(topic, payload, priority: 0, maxRetryCount: 3);
            }
            else
            {
                await _mqttExplorer.PublishAsync(topic, payload);
            }
        }
    }
}