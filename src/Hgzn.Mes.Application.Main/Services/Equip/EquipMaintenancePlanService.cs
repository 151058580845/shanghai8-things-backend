using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipMaintenance;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.Equip
{
    public class EquipMaintenancePlanService : SugarCrudAppService<
    EquipMaintenancePlan, Guid,
    EquipMaintenancePlanReadDto, EquipMaintenancePlanQueryDto,
    EquipMaintenancePlanCreateDto, EquipMaintenancePlanUpdateDto>,
    IEquipMaintenancePlanService
    {
        private readonly IEquipMaintenancePlan _equipMaintenancePlan;
        private readonly IPlanToTaskJob _planToTaskJob;

        public EquipMaintenancePlanService(IPlanToTaskJob planToTaskJob, IEquipMaintenancePlan equipMaintenancePlan)
        {
            _equipMaintenancePlan = equipMaintenancePlan;
            _planToTaskJob = planToTaskJob;
        }

        public async override Task<IEnumerable<EquipMaintenancePlanReadDto>> GetListAsync(EquipMaintenancePlanQueryDto? queryDto = null)
        {
            RefAsync<int> total = 0;
            var entities = await Queryable
                .WhereIF(queryDto != null && !string.IsNullOrEmpty(queryDto.PlanCode), x => x.PlanCode!.Contains(queryDto.PlanCode))
                .WhereIF(queryDto != null && !string.IsNullOrEmpty(queryDto.PlanName), x => x.PlanName!.Contains(queryDto.PlanName))
                .OrderBy(x => x.OrderNum)
                .ToListAsync();
            return Mapper.Map<IEnumerable<EquipMaintenancePlanReadDto>>(entities);
        }

        public async override Task<PaginatedList<EquipMaintenancePlanReadDto>> GetPaginatedListAsync(EquipMaintenancePlanQueryDto queryDto)
        {
            RefAsync<int> total = 0;
            var entities = await Queryable
                .WhereIF(!string.IsNullOrEmpty(queryDto.PlanCode), x => x.PlanCode.Contains(queryDto.PlanCode))
                .WhereIF(!string.IsNullOrEmpty(queryDto.PlanName), x => x.PlanName.Contains(queryDto.PlanName))
                .OrderBy(x => x.OrderNum)
                .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
            return Mapper.Map<PaginatedList<EquipMaintenancePlanReadDto>>(entities);
        }

        public async override Task<EquipMaintenancePlanReadDto> CreateAsync(EquipMaintenancePlanCreateDto createDto)
        {
            EquipMaintenancePlanReadDto entity = await base.CreateAsync(createDto);
            await _equipMaintenancePlan.PlanSetEquipAsync([entity.Id], createDto.PlanEquipEntities);
            await _equipMaintenancePlan.PlanSetItemsAsync([entity.Id], createDto.PlanItemEntities);
            await _planToTaskJob.SchedulePlanInsertAsync(entity.Id, entity.StartTime, entity.EndTime,
                entity.FrequencyNumber);
            return entity;
        }

        public async override Task<EquipMaintenancePlanReadDto> UpdateAsync(Guid id, EquipMaintenancePlanUpdateDto updateDto)
        {
            var entity = await base.UpdateAsync(id, updateDto);
            await _equipMaintenancePlan.PlanSetEquipAsync([entity.Id], updateDto.PlanEquipEntities);
            await _equipMaintenancePlan.PlanSetItemsAsync([entity.Id], updateDto.PlanItemEntities);

            await _planToTaskJob.ScheduleNoticeDeleteAsync(entity.Id);
            await _planToTaskJob.SchedulePlanInsertAsync(entity.Id, entity.StartTime, entity.EndTime,
                entity.FrequencyNumber);
            return entity;
        }
    }
}
