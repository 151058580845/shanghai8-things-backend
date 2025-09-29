using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Entities.System.Location;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.Equip
{
    public class AssetDataServer : SugarCrudAppService<AssetData, Guid, AssetDataReadDto, AssetDataQueryDto, AssetDataCreateDto, AssetDataUpdateDto>, IAssetDataServer
    {
        private readonly ITestDataService _testDataService;

        public AssetDataServer(ITestDataService testDataService)
        {
            _testDataService = testDataService;
        }
        public override async Task<IEnumerable<AssetDataReadDto>> GetListAsync(AssetDataQueryDto? queryDto = null)
        {
            SqlSugar.ISugarQueryable<AssetData> query = DbContext.Queryable<AssetData>();

            if (queryDto != null)
            {
                if (!string.IsNullOrEmpty(queryDto.SystemName))
                {
                    query = query.Where(x => x.SystemName.Contains(queryDto.SystemName));
                }
            }

            List<AssetData> entities = await query.Includes(x => x.Projects).OrderByDescending(x => x.CreationTime).ToListAsync();
            
            // 重新计算每个实体的成本数据
            foreach (AssetData entity in entities)
            {
                await RecalculateDynamicCosts(entity);
            }
            
            return Mapper.Map<IEnumerable<AssetDataReadDto>>(entities);
        }

        public override async Task<PaginatedList<AssetDataReadDto>> GetPaginatedListAsync(AssetDataQueryDto queryDto)
        {
            SqlSugar.ISugarQueryable<AssetData> query = DbContext.Queryable<AssetData>();

            if (!string.IsNullOrEmpty(queryDto.SystemName))
            {
                query = query.Where(x => x.SystemName.Contains(queryDto.SystemName));
            }

            int totalCount = await query.CountAsync();
            List<AssetData> entities = await query
                .Includes(x => x.Projects)
                .OrderByDescending(x => x.CreationTime)
                .ToPageListAsync(queryDto.PageIndex, queryDto.PageSize);

            // 重新计算每个实体的成本数据
            foreach (AssetData entity in entities)
            {
                await RecalculateDynamicCosts(entity);
            }

            IEnumerable<AssetDataReadDto> dtos = Mapper.Map<IEnumerable<AssetDataReadDto>>(entities);
            return new PaginatedList<AssetDataReadDto>(dtos, totalCount, queryDto.PageIndex, queryDto.PageSize);
        }

        public override async Task<AssetDataReadDto?> CreateAsync(AssetDataCreateDto dto)
        {
            AssetData entity = Mapper.Map<AssetData>(dto);
            
            // 计算各项成本
            await CalculateCosts(entity, dto);

            // 插入主记录
            int insertResult = await DbContext.Insertable(entity).ExecuteCommandAsync();
            if (insertResult == 0) return null;

            // 处理项目数据
            if (dto.Projects != null && dto.Projects.Any())
            {
                List<AssetDataProjectItem> projectEntities = dto.Projects.Select(p => new AssetDataProjectItem
                {
                    Id = Guid.NewGuid(),
                    SystemId = entity.Id,
                    ProjectType = p.ProjectType,
                    Amount = p.Amount,
                    StartDate = p.StartDate,
                    Remark = p.Remark,
                    Responsible = p.Responsible
                }).ToList();

                await DbContext.Insertable(projectEntities).ExecuteCommandAsync();
            }

            // 重新查询包含项目数据的完整记录
            AssetData savedEntity = await DbContext.Queryable<AssetData>()
                .Includes(x => x.Projects)
                .InSingleAsync(entity.Id);

            return Mapper.Map<AssetDataReadDto>(savedEntity);
        }

        public override async Task<AssetDataReadDto?> UpdateAsync(Guid key, AssetDataUpdateDto dto)
        {
            AssetData entity = await DbContext.Queryable<AssetData>().InSingleAsync(key);
            if (entity == null) return null;

            // 更新主记录
            Mapper.Map(dto, entity);
            
            // 重新计算各项成本
            await CalculateCosts(entity, new AssetDataCreateDto 
            { 
                Area = dto.Area,
                LaborCostPerHour = dto.LaborCostPerHour,
                FuelPowerCostPerHour = dto.FuelPowerCostPerHour,
                Projects = dto.Projects
            });
            
            await DbContext.Updateable(entity).ExecuteCommandAsync();

            // 删除旧的项目数据
            await DbContext.Deleteable<AssetDataProjectItem>()
                .Where(p => p.SystemId == key)
                .ExecuteCommandAsync();

            // 插入新的项目数据
            if (dto.Projects != null && dto.Projects.Any())
            {
                List<AssetDataProjectItem> projectEntities = dto.Projects.Select(p => new AssetDataProjectItem
                {
                    Id = Guid.NewGuid(),
                    SystemId = key,
                    ProjectType = p.ProjectType,
                    Amount = p.Amount,
                    StartDate = p.StartDate,
                    Remark = p.Remark,
                    Responsible = p.Responsible
                }).ToList();

                await DbContext.Insertable(projectEntities).ExecuteCommandAsync();
            }

            // 重新查询包含项目数据的完整记录
            AssetData updatedEntity = await DbContext.Queryable<AssetData>()
                .Includes(x => x.Projects)
                .InSingleAsync(key);

            return Mapper.Map<AssetDataReadDto>(updatedEntity);
        }

        public override async Task<AssetDataReadDto?> GetAsync(Guid key)
        {
            AssetData entity = await DbContext.Queryable<AssetData>()
                .Includes(x => x.Projects)
                .InSingleAsync(key);
                
            if (entity != null)
            {
                // 重新计算动态成本数据
                await RecalculateDynamicCosts(entity);
            }
            
            return Mapper.Map<AssetDataReadDto>(entity);
        }

        /// <summary>
        /// 重新计算动态成本数据（基于最新的设备运行时长）
        /// </summary>
        /// <param name="entity">资产数据实体</param>
        private async Task RecalculateDynamicCosts(AssetData entity)
        {
            // 常量定义
            const decimal ELECTRICITY_UNIT_PRICE = 0.8m; // 电费单价，单位：元/千瓦时

            // 获取最新的系统工作统计数据
            SystemWorkingStats systemStats = await CalculateSystemWorkingStats(entity.SystemName);

            // 重新计算电费：系统工作时长 × 系统能耗 × 电费单价
            if (entity.SystemEnergyConsumption.HasValue && systemStats.TotalWorkingHours > 0)
            {
                entity.ElectricityCost = Math.Round(systemStats.TotalWorkingHours * entity.SystemEnergyConsumption.Value * ELECTRICITY_UNIT_PRICE, 2);
            }

            // 重新计算燃料动力费：前端输入的燃料动力费(万元/小时) × 系统总工作时长(小时)
            if (entity.FuelPowerCostPerHour.HasValue && entity.FuelPowerCostPerHour.Value > 0 && systemStats.TotalWorkingHours > 0)
            {
                entity.FuelPowerCost = Math.Round(entity.FuelPowerCostPerHour.Value * systemStats.TotalWorkingHours, 2);
            }
            else
            {
                entity.FuelPowerCost = null; // 没有输入时不显示0，显示为null
            }

            // 重新计算人力成本
            if (entity.LaborCostPerHour.HasValue && entity.LaborCostPerHour.Value > 0)
            {
                var (staffCount, staffWorkingHours) = await CalculateStaffWorkingData(entity.SystemName);
                if (staffCount > 0 && staffWorkingHours > 0)
                {
                    entity.LaborCost = Math.Round(staffCount * staffWorkingHours * entity.LaborCostPerHour.Value, 2);
                }
            }

            // 重新计算各项日均费用
            decimal dailyFactoryFee = (entity.FactoryUsageFee ?? 0) / 365;
            decimal dailyEquipmentFee = (entity.EquipmentUsageFee ?? 0) / 365;
            decimal dailyLaborFee = (entity.LaborCost ?? 0) / 365;
            decimal dailyElectricityFee = (entity.ElectricityCost ?? 0) / 365;
            decimal dailyFuelPowerFee = (entity.FuelPowerCost ?? 0) / 365;
            decimal dailyMaintenanceFee = (entity.EquipmentMaintenanceCost ?? 0) / 365;

            // 重新计算系统空置成本
            if (systemStats.IdleDays > 0)
            {
                entity.SystemIdleCost = Math.Round(systemStats.IdleDays * (dailyFactoryFee + dailyEquipmentFee + dailyMaintenanceFee), 2);
            }

            // 重新计算系统试验成本
            if (systemStats.WorkingDays > 0)
            {
                entity.SystemExperimentCost = Math.Round(systemStats.WorkingDays * (
                    dailyFactoryFee + 
                    dailyEquipmentFee + 
                    dailyLaborFee + 
                    dailyElectricityFee + 
                    dailyFuelPowerFee + 
                    dailyMaintenanceFee
                ), 2);
            }

            // 保存重新计算的数据到数据库
            await DbContext.Updateable(entity).ExecuteCommandAsync();
        }

        /// <summary>
        /// 成本计算方法（用于创建和更新时的完整计算）
        /// </summary>
        private async Task CalculateCosts(AssetData entity, AssetDataCreateDto dto)
        {
            // 常量定义
            const decimal INFRASTRUCTURE_UNIT_PRICE = 1000m; // 基建单价，单位：元/平方米/年
            const decimal MAINTENANCE_RATE = 0.05m; // 设备保养费率
            const decimal ELECTRICITY_UNIT_PRICE = 0.8m; // 电费单价，单位：元/千瓦时

            // 1. 厂房使用费 = 区域(面积) × 基建单价（年费用）
            entity.FactoryUsageFee = Math.Round((entity.Area ?? 0) * INFRASTRUCTURE_UNIT_PRICE, 2);

            // 2. 设备使用费 = 项目费用之和（年费用）
            if (dto.Projects != null && dto.Projects.Any())
            {
                entity.EquipmentUsageFee = Math.Round(dto.Projects.Sum(p => p.Amount ?? 0), 2);
            }

            // 3. 人力成本 = 人员岗位数量 × 人员年工时 × 人力成本单价（年费用）
            if (entity.LaborCostPerHour.HasValue && entity.LaborCostPerHour.Value > 0)
            {
                var (staffCount, staffWorkingHours) = await CalculateStaffWorkingData(entity.SystemName);
                if (staffCount > 0 && staffWorkingHours > 0)
                {
                    entity.LaborCost = Math.Round(staffCount * staffWorkingHours * entity.LaborCostPerHour.Value, 2);
                }
            }

            // 6. 设备保养费用：按设备使用费的一定比例计算
            entity.EquipmentMaintenanceCost = Math.Round((entity.EquipmentUsageFee ?? 0) * MAINTENANCE_RATE, 2);

            // 获取系统工作统计数据
            SystemWorkingStats systemStats = await CalculateSystemWorkingStats(entity.SystemName);

            // 4. 电费：系统工作时长 × 系统能耗 × 电费单价
            if (entity.SystemEnergyConsumption.HasValue && systemStats.TotalWorkingHours > 0)
            {
                entity.ElectricityCost = Math.Round(systemStats.TotalWorkingHours * entity.SystemEnergyConsumption.Value * ELECTRICITY_UNIT_PRICE, 2);
            }

            // 5. 燃料动力费：前端输入的燃料动力费(万元/小时) × 系统总工作时长(小时)
            if (entity.FuelPowerCostPerHour.HasValue && entity.FuelPowerCostPerHour.Value > 0 && systemStats.TotalWorkingHours > 0)
            {
                entity.FuelPowerCost = Math.Round(entity.FuelPowerCostPerHour.Value * systemStats.TotalWorkingHours, 2);
            }
            else
            {
                entity.FuelPowerCost = null; // 没有输入时不显示0，显示为null
            }

            // 7. 系统空置成本 = 空置天数 × (厂房使用费/365 + 设备使用费/365 + 设备保养费/365)
            decimal dailyFactoryFee = (entity.FactoryUsageFee ?? 0) / 365;
            decimal dailyEquipmentFee = (entity.EquipmentUsageFee ?? 0) / 365;
            decimal dailyLaborFee = (entity.LaborCost ?? 0) / 365;
            decimal dailyElectricityFee = (entity.ElectricityCost ?? 0) / 365;
            decimal dailyFuelPowerFee = (entity.FuelPowerCost ?? 0) / 365;
            decimal dailyMaintenanceFee = (entity.EquipmentMaintenanceCost ?? 0) / 365;

            if (systemStats.IdleDays > 0)
            {
                entity.SystemIdleCost = Math.Round(systemStats.IdleDays * (dailyFactoryFee + dailyEquipmentFee + dailyMaintenanceFee), 2);
            }

            // 8. 系统试验成本 = 工作天数 × (厂房使用费/365 + 设备使用费/365 + 人力成本/365 + 电费/365 + 燃料动力费/365 + 设备保养费/365)
            if (systemStats.WorkingDays > 0)
            {
                entity.SystemExperimentCost = Math.Round(systemStats.WorkingDays * (
                    dailyFactoryFee + 
                    dailyEquipmentFee + 
                    dailyLaborFee + 
                    dailyElectricityFee + 
                    dailyFuelPowerFee + 
                    dailyMaintenanceFee
                ), 2);
            }
        }

        /// <summary>
        /// 计算系统工作统计数据
        /// </summary>
        /// <param name="systemName">系统名称</param>
        /// <returns>系统工作统计信息</returns>
        private async Task<SystemWorkingStats> CalculateSystemWorkingStats(string systemName)
        {
            try
            {
                // 根据系统名称获取对应的房间ID
                Guid roomId = GetRoomIdBySystemName(systemName);
                if (roomId == Guid.Empty)
                {
                    return new SystemWorkingStats(); // 返回空统计
                }

                // 获取该系统下的所有设备
                List<EquipLedger> systemEquips = await DbContext.Queryable<EquipLedger>()
                    .Where(x => x.RoomId == roomId && !x.SoftDeleted)
                    .ToListAsync();

                if (!systemEquips.Any())
                {
                    return new SystemWorkingStats(); // 没有设备则返回空统计
                }

                // 获取最早的运行时长记录日期
                DateTime earliestDate = await DbContext.Queryable<EquipDailyRuntime>()
                    .Where(x => systemEquips.Select(e => e.Id).Contains(x.EquipId))
                    .MinAsync(x => x.RecordDate);

                if (earliestDate == default)
                {
                    // 如果没有运行时长记录，使用设备最早的创建时间
                    earliestDate = systemEquips.Min(x => x.CreationTime).Date;
                }

                // 计算从最早记录到现在的所有日期
                DateTime today = DateTime.Now.Date;
                int totalDays = (int)(today - earliestDate).TotalDays + 1;

                // 获取所有有运行记录的日期（去重）
                List<DateTime> workingDates = await DbContext.Queryable<EquipDailyRuntime>()
                    .Where(x => systemEquips.Select(e => e.Id).Contains(x.EquipId))
                    .Where(x => x.RecordDate >= earliestDate && x.RecordDate <= today)
                    .Where(x => x.RunningSeconds > 0) // 只统计有实际运行时间的日期
                    .Select(x => x.RecordDate.Date)
                    .Distinct()
                    .ToListAsync();

                // 计算总工作时长（所有设备在所有工作日的运行时长总和）
                uint totalWorkingSeconds = await DbContext.Queryable<EquipDailyRuntime>()
                    .Where(x => systemEquips.Select(e => e.Id).Contains(x.EquipId))
                    .Where(x => x.RecordDate >= earliestDate && x.RecordDate <= today)
                    .SumAsync(x => x.RunningSeconds);

                int workingDays = workingDates.Count;
                int idleDays = totalDays - workingDays;
                decimal totalWorkingHours = Math.Round((decimal)totalWorkingSeconds / 3600, 2);

                return new SystemWorkingStats
                {
                    EarliestDate = earliestDate,
                    TotalDays = totalDays,
                    WorkingDays = workingDays,
                    IdleDays = idleDays,
                    TotalWorkingHours = totalWorkingHours,
                    EquipmentCount = systemEquips.Count
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"计算系统工作统计数据失败: {ex.Message}");
                return new SystemWorkingStats(); // 异常时返回空统计
            }
        }

        /// <summary>
        /// 计算人员工作数据
        /// </summary>
        /// <param name="systemName">系统名称</param>
        /// <returns>人员数量和年工时</returns>
        private async Task<(int staffCount, decimal workingHours)> CalculateStaffWorkingData(string systemName)
        {
            try
            {
                // 获取当前任务列表
                var currentTasks = await _testDataService.GetCurrentListByTestAsync();
                var systemTasks = currentTasks.Where(t => t.SysName == systemName).ToList();

                if (!systemTasks.Any())
                {
                    return (0, 0);
                }

                int totalStaffCount = 0;
                decimal totalWorkingHours = 0;

                foreach (var task in systemTasks)
                {
                    // 计算人员岗位数量
                    int taskStaffCount = 0;
                    
                    // 1. 仿真试验专业代表（通常1人）
                    if (!string.IsNullOrEmpty(task.SimuResp))
                    {
                        taskStaffCount += 1;
                    }

                    // 2. 仿真试验参与人员（可能多人，用逗号分隔）
                    if (!string.IsNullOrEmpty(task.SimuStaff))
                    {
                        var staffNames = task.SimuStaff.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        taskStaffCount += staffNames.Length;
                    }

                    totalStaffCount += taskStaffCount;

                    // 计算人员年工时
                    if (DateTime.TryParse(task.TaskStartTime, out DateTime startTime) &&
                        DateTime.TryParse(task.TaskEndTime, out DateTime endTime))
                    {
                        var taskDuration = endTime - startTime;
                        var workingDays = taskDuration.Days;

                        // 获取系统工作统计数据来计算实际工作时长
                        var roomId = GetRoomIdBySystemName(systemName);
                        if (roomId != Guid.Empty)
                        {
                            var systemEquips = await DbContext.Queryable<EquipLedger>()
                                .Where(x => x.RoomId == roomId && !x.SoftDeleted)
                                .ToListAsync();

                            if (systemEquips.Any())
                            {
                                // 获取任务期间的实际运行时长
                                var runningData = await DbContext.Queryable<EquipDailyRuntime>()
                                    .Where(x => systemEquips.Select(e => e.Id).Contains(x.EquipId))
                                    .Where(x => x.RecordDate >= startTime.Date && x.RecordDate <= endTime.Date)
                                    .Where(x => x.RunningSeconds > 0)
                                    .ToListAsync();

                                // 按日期分组计算每日工作时长（取每天运行时间最长的设备）
                                var dailyWorkingHours = runningData
                                    .GroupBy(x => x.RecordDate.Date)
                                    .Select(g => Math.Max(8, Math.Round((decimal)g.Max(x => x.RunningSeconds) / 3600, 2))) // 最少8小时，最多当天运行时间最长的设备运行时间
                                    .Sum();

                                totalWorkingHours += dailyWorkingHours;
                            }
                            else
                            {
                                // 没有设备数据时，按每天8小时计算
                                totalWorkingHours += workingDays * 8;
                            }
                        }
                        else
                        {
                            // 找不到房间时，按每天8小时计算
                            totalWorkingHours += workingDays * 8;
                        }
                    }
                }

                return (totalStaffCount, totalWorkingHours);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"计算人员工作数据失败: {ex.Message}");
                return (0, 0);
            }
        }

        /// <summary>
        /// 根据系统名称获取对应的房间ID
        /// </summary>
        /// <param name="systemName">系统名称</param>
        /// <returns>房间ID</returns>
        private Guid GetRoomIdBySystemName(string systemName)
        {
            // 根据TestEquipData中的系统名称映射获取房间ID
            for (int systemId = 1; systemId <= 10; systemId++)
            {
                string mappedSystemName = TestEquipData.GetSystemName(systemId);
                if (string.Equals(mappedSystemName, systemName, StringComparison.OrdinalIgnoreCase))
                {
                    string roomIdString = TestEquipData.GetRoomId(systemId);
                    if (Guid.TryParse(roomIdString, out Guid roomId))
                    {
                        return roomId;
                    }
                }
            }
            
            return Guid.Empty; // 未找到对应的房间ID
        }
    }

    /// <summary>
    /// 系统工作统计信息
    /// </summary>
    public class SystemWorkingStats
    {
        /// <summary>
        /// 最早记录日期
        /// </summary>
        public DateTime EarliestDate { get; set; }

        /// <summary>
        /// 总天数（从最早记录到现在）
        /// </summary>
        public int TotalDays { get; set; }

        /// <summary>
        /// 工作天数（有设备运行的天数）
        /// </summary>
        public int WorkingDays { get; set; }

        /// <summary>
        /// 空置天数（没有设备运行的天数）
        /// </summary>
        public int IdleDays { get; set; }

        /// <summary>
        /// 总工作时长（小时）
        /// </summary>
        public decimal TotalWorkingHours { get; set; }

        /// <summary>
        /// 设备数量
        /// </summary>
        public int EquipmentCount { get; set; }

        /// <summary>
        /// 平均每日工作时长（小时）
        /// </summary>
        public decimal AverageHoursPerWorkingDay => WorkingDays > 0 ? Math.Round(TotalWorkingHours / WorkingDays, 2) : 0;

        /// <summary>
        /// 系统利用率（工作天数/总天数）
        /// </summary>
        public decimal UtilizationRate => TotalDays > 0 ? Math.Round((decimal)WorkingDays / TotalDays * 100, 2) : 0;
    }
}
