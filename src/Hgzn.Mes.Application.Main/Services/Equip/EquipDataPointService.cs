using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Services.Equip
{
    internal class EquipDataPointService : SugarCrudAppService<
    EquipDataPoint, Guid,
    EquipDataPointReadDto, EquipDataPointQueryDto,
    EquipDataPointCreateDto, EquipDataPointUpdateDto>, IEquipDataPointService
    {
        private IEquipConnService _equipConnService;
        public EquipDataPointService(IEquipConnService equipConnService)
        {
            _equipConnService = equipConnService;

        }

        public async override Task<IEnumerable<EquipDataPointReadDto>> GetListAsync(EquipDataPointQueryDto? queryDto = null)
        {
            RefAsync<int> total = 0;
            var entites = await Queryable
                .WhereIF(queryDto != null && queryDto.State == null, t => t.State == queryDto!.State)
                .WhereIF(queryDto != null && !string.IsNullOrEmpty(queryDto.Code), t => t.Code.Contains(queryDto!.Code!))
                .ToListAsync();
            IEnumerable<EquipDataPointReadDto> outputs = Mapper.Map<IEnumerable<EquipDataPointReadDto>>(entites);
            foreach (EquipDataPointReadDto entity in outputs)
            {
                if (entity.ConnectionId != null)
                {
                    //获取连接状态
                    // entity.Data = await RedieService.GetRedisDataAsync(entity.Code);
                }
            }

            return outputs;
        }

        public async override Task<PaginatedList<EquipDataPointReadDto>> GetPaginatedListAsync(EquipDataPointQueryDto queryDto)
        {
            RefAsync<int> total = 0;
            var entites = await Queryable
                .WhereIF(queryDto.State == null, t => t.State == queryDto.State)
                .WhereIF(!string.IsNullOrEmpty(queryDto.Code), t => t.Code.Contains(queryDto!.Code!))
                .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
            IEnumerable<EquipDataPointReadDto> outputs = Mapper.Map<IEnumerable<EquipDataPointReadDto>>(entites);
            foreach (EquipDataPointReadDto entity in outputs)
            {
                if (entity.ConnectionId != null)
                {
                    //获取连接状态
                    // entity.Data = await RedieService.GetRedisDataAsync(entity.Code);
                }
            }

            return new PaginatedList<EquipDataPointReadDto>(outputs, total, queryDto.PageIndex, queryDto.PageSize);
        }

        /// <summary>
        /// 开始采集
        /// </summary>
        /// <param name="id">点位Id</param>
        /// <returns></returns>
        public async Task PutStartConnect(Guid id)
        {
            //try
            //{
            //    var dataPoint = await GetEntityByIdAsync(id);
            //    dataPoint.Connection = await _equipConnect.GetEntityByIdAsync(dataPoint.ConnectionId);
            //    if (await _equipConnService.IsConnectedAsync(dataPoint.ConnectionId))
            //    {
            //        await _equipDataPointManager.StartCollectAsync(dataPoint);
            //        //将点位加入队列中
            //        await _collectionJob.AddDataPoint(dataPoint);
            //    }
            //    else
            //    {
            //        throw new ApplicationException("找不到该连接");
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    throw;
            //}
        }

        /// <summary>
        /// 停止采集
        /// </summary>
        /// <param name="id">点位Id</param>
        /// <returns></returns>
        public async Task PutStopConnect(Guid id)
        {
            //var dataPoint = await GetEntityByIdAsync(id);
            //if (await _equipConnService.IsConnectedAsync(dataPoint.ConnectionId))
            //{
            //    await _equipDataPointManager.StopCollectAsync(dataPoint);
            //}
            //else
            //{
            //    throw new ApplicationException("找不到该连接");
            //}
        }
    }
}
