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

        /// <summary>
        /// 系统数据缓存 - 用于批量计算时避免重复查询
        /// </summary>
        private class SystemDataCache
        {
            public List<TestDataReadDto> AllTasks { get; set; } = new();
            public Dictionary<Guid, List<Guid>> RoomEquipmentMap { get; set; } = new();
            public Dictionary<string, SystemWorkingStats> SystemStatsCache { get; set; } = new();
            public Dictionary<string, decimal> AdjustedHoursCache { get; set; } = new();
            public Dictionary<string, (int count, decimal hours)> StaffDataCache { get; set; } = new();
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

            if (!entities.Any())
            {
                return Mapper.Map<IEnumerable<AssetDataReadDto>>(entities);
            }

            // 批量加载所有需要的数据并缓存
            var cache = await LoadSystemDataCacheAsync(entities.Select(e => e.SystemName).Distinct().ToList());

            // 重新计算每个实体的成本数据（只在内存中计算，不更新数据库）
            foreach (AssetData entity in entities)
            {
                RecalculateDynamicCostsInMemory(entity, cache);
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

            if (entities.Any())
            {
                // 批量加载所有需要的数据并缓存
                var cache = await LoadSystemDataCacheAsync(entities.Select(e => e.SystemName).Distinct().ToList());

                // 重新计算每个实体的成本数据（只在内存中计算，不更新数据库）
                foreach (AssetData entity in entities)
                {
                    RecalculateDynamicCostsInMemory(entity, cache);
                }
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
                // 单个实体查询时仍需更新数据库
                await RecalculateDynamicCosts(entity);
            }

            return Mapper.Map<AssetDataReadDto>(entity);
        }

        /// <summary>
        /// 批量加载系统数据缓存（优化性能，避免重复查询）
        /// </summary>
        private async Task<SystemDataCache> LoadSystemDataCacheAsync(List<string> systemNames)
        {
            var cache = new SystemDataCache();

            if (!systemNames.Any())
            {
                return cache;
            }

            // 1. 一次性获取所有试验任务数据
            var currentTasks = await _testDataService.GetCurrentListByTestAsync();
            var historyTasks = await _testDataService.GetHistoryListByTestAsync();
            cache.AllTasks.AddRange(currentTasks);
            cache.AllTasks.AddRange(historyTasks);

            // 2. 获取所有相关的房间ID和设备
            var allRoomIds = new List<Guid>();
            foreach (var systemName in systemNames)
            {
                var roomId = GetRoomIdBySystemName(systemName);
                if (roomId != Guid.Empty)
                {
                    allRoomIds.Add(roomId);
                }
            }

            if (allRoomIds.Any())
            {
                // 批量查询所有房间的设备
                var allEquips = await DbContext.Queryable<EquipLedger>()
                    .Where(x => allRoomIds.Contains(x.RoomId.Value) && !x.SoftDeleted)
                    .Select(x => new { x.Id, RoomId = x.RoomId.Value })
                    .ToListAsync();

                foreach (var roomId in allRoomIds.Distinct())
                {
                    cache.RoomEquipmentMap[roomId] = allEquips
                        .Where(e => e.RoomId == roomId)
                        .Select(e => e.Id)
                        .ToList();
                }
            }

            return cache;
        }

        /// <summary>
        /// 在内存中重新计算动态成本数据（不更新数据库，用于列表查询优化）
        /// </summary>
        private void RecalculateDynamicCostsInMemory(AssetData entity, SystemDataCache cache)
        {
            const decimal ELECTRICITY_UNIT_PRICE = 1m;

            // 获取缓存的系统工作统计数据
            if (!cache.SystemStatsCache.TryGetValue(entity.SystemName, out var systemStats))
            {
                systemStats = CalculateSystemWorkingStatsWithCache(entity.SystemName, cache);
                cache.SystemStatsCache[entity.SystemName] = systemStats;
            }

            // 获取缓存的调整后工作时长
            if (!cache.AdjustedHoursCache.TryGetValue(entity.SystemName, out var adjustedTotalWorkingHours))
            {
                adjustedTotalWorkingHours = CalculateAdjustedTotalWorkingHoursWithCache(entity.SystemName, cache);
                cache.AdjustedHoursCache[entity.SystemName] = adjustedTotalWorkingHours;
            }

            // 重新计算电费
            if (entity.SystemEnergyConsumption.HasValue)
            {
                if (adjustedTotalWorkingHours > 0)
                {
                    entity.ElectricityCost = Math.Round(adjustedTotalWorkingHours * entity.SystemEnergyConsumption.Value * ELECTRICITY_UNIT_PRICE, 2);
                }
                else
                {
                    entity.ElectricityCost = null;
                }
            }

            // 重新计算燃料动力费
            if (entity.FuelPowerCostPerHour.HasValue && entity.FuelPowerCostPerHour.Value > 0)
            {
                if (adjustedTotalWorkingHours > 0)
                {
                    entity.FuelPowerCost = Math.Round(entity.FuelPowerCostPerHour.Value * adjustedTotalWorkingHours, 2);
                }
                else
                {
                    entity.FuelPowerCost = null;
                }
            }
            else
            {
                entity.FuelPowerCost = null;
            }

            // 重新计算人力成本
            if (entity.LaborCostPerHour.HasValue && entity.LaborCostPerHour.Value > 0)
            {
                if (!cache.StaffDataCache.TryGetValue(entity.SystemName, out var staffData))
                {
                    staffData = CalculateStaffWorkingDataWithCache(entity.SystemName, cache);
                    cache.StaffDataCache[entity.SystemName] = staffData;
                }

                if (staffData.count > 0 && staffData.hours > 0)
                {
                    entity.LaborCost = Math.Round(staffData.count * staffData.hours * entity.LaborCostPerHour.Value, 2);
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
            else
            {
                entity.SystemIdleCost = null; // 没有闲置天数，空置成本为null
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
            else
            {
                entity.SystemExperimentCost = null; // 没有工作天数，试验成本为null
            }
        }

        /// <summary>
        /// 重新计算动态成本数据（基于最新的设备运行时长）
        /// </summary>
        /// <param name="entity">资产数据实体</param>
        private async Task RecalculateDynamicCosts(AssetData entity)
        {
            // 常量定义
            const decimal ELECTRICITY_UNIT_PRICE = 1m; // 电费单价，单位：元/千瓦时

            // 获取最新的系统工作统计数据
            SystemWorkingStats systemStats = await CalculateSystemWorkingStats(entity.SystemName);

            // 重新计算电费：按“计划周期存在真实运行则按真实运行时长；否则按8小时/天”策略
            if (entity.SystemEnergyConsumption.HasValue)
            {
                decimal adjustedTotalWorkingHours = await CalculateAdjustedTotalWorkingHours(entity.SystemName);
                if (adjustedTotalWorkingHours > 0)
                {
                    entity.ElectricityCost = Math.Round(adjustedTotalWorkingHours * entity.SystemEnergyConsumption.Value * ELECTRICITY_UNIT_PRICE, 2);
                }
                else
                {
                    entity.ElectricityCost = null;
                }
            }

            // 重新计算燃料动力费：按同一策略（真实运行或8小时/天）
            if (entity.FuelPowerCostPerHour.HasValue && entity.FuelPowerCostPerHour.Value > 0)
            {
                decimal adjustedTotalWorkingHours = await CalculateAdjustedTotalWorkingHours(entity.SystemName);
                if (adjustedTotalWorkingHours > 0)
                {
                    entity.FuelPowerCost = Math.Round(entity.FuelPowerCostPerHour.Value * adjustedTotalWorkingHours, 2);
                }
                else
                {
                    entity.FuelPowerCost = null; // 没有输入时不显示0，显示为null
                }
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
            else
            {
                entity.SystemIdleCost = null; // 没有闲置天数，空置成本为null
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
            else
            {
                entity.SystemExperimentCost = null; // 没有工作天数，试验成本为null
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
            const decimal ELECTRICITY_UNIT_PRICE = 1m; // 电费单价，单位：元/千瓦时

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
            else
            {
                entity.SystemIdleCost = null; // 没有闲置天数，空置成本为null
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
            else
            {
                entity.SystemExperimentCost = null; // 没有工作天数，试验成本为null
            }
        }

        /// <summary>
        /// 计算系统工作统计数据（按新算法：基于试验计划周期）
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

                // 获取试验任务
                List<TestDataReadDto> tasks = new List<TestDataReadDto>();
                IEnumerable<TestDataReadDto> currentTasks = await _testDataService.GetCurrentListByTestAsync();
                tasks.AddRange(currentTasks);
                IEnumerable<TestDataReadDto> historyTasks = await _testDataService.GetHistoryListByTestAsync();
                tasks.AddRange(historyTasks);
                List<TestDataReadDto> systemTasks = tasks.Where(t => t.SysName == systemName).ToList();

                if (!systemTasks.Any())
                {
                    return new SystemWorkingStats(); // 没有试验任务则返回空统计
                }

                DateTime earliestDate = DateTime.MaxValue;
                DateTime latestDate = DateTime.MinValue;
                int totalWorkingDays = 0;
                int totalIdleDays = 0;
                decimal totalWorkingHours = 0;

                // 遍历所有试验任务，计算工作天数
                foreach (var task in systemTasks)
                {
                    if (!DateTime.TryParse(task.TaskStartTime, out DateTime start) ||
                        !DateTime.TryParse(task.TaskEndTime, out DateTime end))
                    {
                        continue;
                    }

                    // 只统计过去或当前正在进行的计划周期
                    if (start.Date > DateTime.Now.Date) continue;

                    // 更新最早和最晚日期
                    if (start < earliestDate) earliestDate = start.Date;
                    if (end > latestDate) latestDate = end.Date > DateTime.Now.Date ? DateTime.Now.Date : end.Date;

                    // 计算该任务期间的天数（超过今天的任务只算到今天）
                    DateTime taskEndDate = end.Date > DateTime.Now.Date ? DateTime.Now.Date : end.Date;
                    int taskDays = (int)(taskEndDate - start.Date).TotalDays + 1;

                    // 查询该任务期间的真实运行数据
                    uint taskSeconds = 0;
                    if (systemEquips.Any())
                    {
                        taskSeconds = await DbContext.Queryable<EquipDailyRuntime>()
                            .Where(x => systemEquips.Select(e => e.Id).Contains(x.EquipId))
                            .Where(x => x.RecordDate >= start.Date && x.RecordDate <= taskEndDate)
                            .SumAsync(x => x.RunningSeconds);
                    }

                    if (taskSeconds > 0)
                    {
                        // 有真实运行数据，按实际运行计算
                        totalWorkingDays += taskDays;
                        totalWorkingHours += Math.Round((decimal)taskSeconds / 3600, 2);
                    }
                    else
                    {
                        // 无真实运行数据，按8小时/天计算
                        totalWorkingDays += taskDays;
                        totalWorkingHours += taskDays * 8;
                    }
                }

                if (earliestDate == DateTime.MaxValue)
                {
                    return new SystemWorkingStats(); // 没有有效任务
                }

                // 计算总天数（从最早任务开始到最晚任务结束，但不包含"未来"）
                // 如果latestDate是今天，说明有任务延续到今天或未来，今天还未结束，不计入闲置计算
                DateTime effectiveLatestDate = latestDate >= DateTime.Now.Date ? DateTime.Now.Date.AddDays(-1) : latestDate;
                
                int totalDays;
                // 确保有效的日期范围（至少要有一天）
                if (effectiveLatestDate < earliestDate)
                {
                    // 所有任务都是从今天开始的，没有过去的历史，不计算闲置
                    totalDays = 0;
                    totalIdleDays = 0;
                }
                else
                {
                    totalDays = (int)(effectiveLatestDate - earliestDate).TotalDays + 1;
                    totalIdleDays = totalDays - totalWorkingDays;
                    // 确保闲置天数不为负
                    if (totalIdleDays < 0) totalIdleDays = 0;
                }

                return new SystemWorkingStats
                {
                    EarliestDate = earliestDate,
                    TotalDays = totalDays,
                    WorkingDays = totalWorkingDays,
                    IdleDays = totalIdleDays,
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
                // 获取试验任务
                List<TestDataReadDto> tasks = new List<TestDataReadDto>();
                IEnumerable<TestDataReadDto> currentTasks = await _testDataService.GetCurrentListByTestAsync();
                tasks.AddRange(currentTasks);
                IEnumerable<TestDataReadDto> historyTasks = await _testDataService.GetHistoryListByTestAsync();
                tasks.AddRange(historyTasks);
                var systemTasks = tasks.Where(t => t.SysName == systemName).ToList();

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
                    if (!string.IsNullOrEmpty(task.SimuResp)) taskStaffCount += 1; // 专业代表
                    if (!string.IsNullOrEmpty(task.SimuStaff))
                    {
                        var staffNames = task.SimuStaff.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        taskStaffCount += staffNames.Length; // 参与人员
                    }
                    totalStaffCount += taskStaffCount;

                    // 计算人员年工时（按计划周期：有真实运行按真实，无则8小时/天）
                    if (DateTime.TryParse(task.TaskStartTime, out DateTime startTime) &&
                        DateTime.TryParse(task.TaskEndTime, out DateTime endTime))
                    {
                        // 计算任务的实际结束日期（超过今天的任务只算到今天）
                        DateTime taskEndDate = endTime.Date > DateTime.Now.Date ? DateTime.Now.Date : endTime.Date;

                        var roomId = GetRoomIdBySystemName(systemName);
                        if (roomId != Guid.Empty)
                        {
                            var systemEquips = await DbContext.Queryable<EquipLedger>()
                                .Where(x => x.RoomId == roomId && !x.SoftDeleted)
                                .Select(x => x.Id)
                                .ToListAsync();

                            if (systemEquips.Any())
                            {
                                uint totalSeconds = await DbContext.Queryable<EquipDailyRuntime>()
                                    .Where(x => systemEquips.Contains(x.EquipId))
                                    .Where(x => x.RecordDate >= startTime.Date && x.RecordDate <= taskEndDate)
                                    .SumAsync(x => x.RunningSeconds);

                                if (totalSeconds > 0)
                                {
                                    // 如果有真实的设备运行时间,那就按真实的设备运行时间算
                                    totalWorkingHours += Math.Round((decimal)totalSeconds / 3600, 2);
                                }
                                else
                                {
                                    // 如果没有真实的运行时间,就按照一天8小时算
                                    int days = (int)(taskEndDate - startTime.Date).TotalDays + 1;
                                    totalWorkingHours += days * 8;
                                }
                            }
                            else
                            {
                                // 如果没有真实的运行时间,就按照一天8小时算
                                int days = (int)(taskEndDate - startTime.Date).TotalDays + 1;
                                totalWorkingHours += days * 8;
                            }
                        }
                        else
                        {
                            int days = (int)(taskEndDate - startTime.Date).TotalDays + 1;
                            totalWorkingHours += days * 8;
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
        /// 计算系统的“调整后总工作时长”：
        /// - 对于每个过去或正在进行中的试验计划周期，如果存在真实设备运行时长，则使用真实运行时长；
        /// - 否则按该周期内每天8小时计算。
        /// </summary>
        private async Task<decimal> CalculateAdjustedTotalWorkingHours(string systemName)
        {
            try
            {
                // 获取试验任务
                List<TestDataReadDto> taskList = new List<TestDataReadDto>();
                IEnumerable<TestDataReadDto> currentTasks = await _testDataService.GetCurrentListByTestAsync();
                taskList.AddRange(currentTasks);
                IEnumerable<TestDataReadDto> historyTasks = await _testDataService.GetHistoryListByTestAsync();
                taskList.AddRange(historyTasks);
                Console.WriteLine($"AG - 调整后工作时长计算 - 获取到 {taskList.Count} 个总任务");

                var systemTasks = taskList.Where(t => t.SysName == systemName).ToList();
                if (!systemTasks.Any())
                {
                    Console.WriteLine($"AG - 调整后工作时长计算 - 系统 {systemName} 没有找到试验任务");
                    Console.WriteLine($"AG - 调整后工作时长计算 - 所有任务的系统名称: {string.Join(", ", currentTasks.Select(t => t.SysName).Distinct())}");
                    return 0;
                }

                Console.WriteLine($"AG - 调整后工作时长计算 - 系统 {systemName} 找到 {systemTasks.Count} 个试验任务");

                var roomId = GetRoomIdBySystemName(systemName);
                if (roomId == Guid.Empty) return 0;

                var systemEquipIds = await DbContext.Queryable<EquipLedger>()
                    .Where(x => x.RoomId == roomId && !x.SoftDeleted)
                    .Select(x => x.Id)
                    .ToListAsync();

                Console.WriteLine($"AG - 调整后工作时长计算 - 系统 {systemName} 找到 {systemEquipIds.Count} 个设备");

                // 即使没有设备，也可以按8小时/天计算，所以不直接返回0

                DateTime now = DateTime.Now;
                decimal totalHours = 0;
                foreach (var task in systemTasks)
                {
                    if (!DateTime.TryParse(task.TaskStartTime, out DateTime start) ||
                        !DateTime.TryParse(task.TaskEndTime, out DateTime end))
                    {
                        Console.WriteLine($"AG - 调整后工作时长计算 - 任务 {task.TaskName} 时间解析失败");
                        continue;
                    }

                    // 只统计过去或当前正在进行的计划周期（开始时间不晚于今天）
                    if (start.Date > DateTime.Now.Date)
                    {
                        Console.WriteLine($"AG - 调整后工作时长计算 - 任务 {task.TaskName} 开始时间 {start:yyyy-MM-dd} 晚于今天，跳过");
                        continue;
                    }

                    // 计算任务的实际结束日期（超过今天的任务只算到今天）
                    DateTime taskEndDate = end.Date > DateTime.Now.Date ? DateTime.Now.Date : end.Date;

                    uint seconds = 0;

                    // 如果有设备，查询真实运行数据
                    if (systemEquipIds.Any())
                    {
                        // 统计该周期的真实运行秒数
                        seconds = await DbContext.Queryable<EquipDailyRuntime>()
                            .Where(x => systemEquipIds.Contains(x.EquipId))
                            .Where(x => x.RecordDate >= start.Date && x.RecordDate <= taskEndDate)
                            .SumAsync(x => x.RunningSeconds);
                    }

                    Console.WriteLine($"AG - 调整后工作时长计算 - 任务 {task.TaskName} 真实运行秒数: {seconds}");

                    if (seconds > 0)
                    {
                        decimal taskHours = Math.Round((decimal)seconds / 3600, 2);
                        totalHours += taskHours;
                        Console.WriteLine($"AG - 调整后工作时长计算 - 任务 {task.TaskName} 使用真实运行时长: {taskHours} 小时");
                    }
                    else
                    {
                        int days = (int)(taskEndDate - start.Date).TotalDays + 1;
                        if (days > 0)
                        {
                            decimal taskHours = days * 8;
                            totalHours += taskHours;
                            Console.WriteLine($"AG - 调整后工作时长计算 - 任务 {task.TaskName} 无真实运行数据，按 {days} 天 × 8小时 = {taskHours} 小时计算");
                        }
                    }
                }

                Console.WriteLine($"AG - 调整后工作时长计算 - 系统 {systemName} 总工作时长: {totalHours} 小时");
                return totalHours;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AG - 调整后工作时长计算 - 系统 {systemName} 计算失败: {ex.Message}");
                return 0;
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

        /// <summary>
        /// 获取成本计算明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AssetDataCalculationDetailsDto> GetCalculationDetailsAsync(Guid id)
        {
            var entity = await DbContext.Queryable<AssetData>()
                .Includes(x => x.Projects)
                .InSingleAsync(id);

            if (entity == null)
                throw new KeyNotFoundException($"未找到ID为{id}的资产数据");

            // 获取系统工作统计数据和调整后工作时长
            var adjustedWorkingHours = await CalculateAdjustedTotalWorkingHours(entity.SystemName);
            var (staffCount, staffWorkingHours) = await CalculateStaffWorkingData(entity.SystemName);

            // 重新计算动态成本数据以获取最新值（这会调用CalculateSystemWorkingStats）
            await RecalculateDynamicCosts(entity);
            
            // 重新获取计算后的系统工作统计数据（确保与entity中的值一致）
            var systemStats = await CalculateSystemWorkingStats(entity.SystemName);

            return new AssetDataCalculationDetailsDto
            {
                SystemName = entity.SystemName,
                FactoryUsageFee = new CostDetailDto
                {
                    CostName = "厂房使用费",
                    CalculationFormula = "区域(面积) × 基建单价（年费用）",
                    DataSource = $"区域面积: {entity.Area}平方米, 基建单价: 1000元/平方米/年",
                    CalculationProcess = $"{entity.Area} × 1000 = {entity.FactoryUsageFee}元",
                    FinalResult = entity.FactoryUsageFee,
                    Parameters = new Dictionary<string, object>
                    {
                        { "Area", entity.Area ?? 0 },
                        { "InfrastructureUnitPrice", 1000m }
                    }
                },
                EquipmentUsageFee = new CostDetailDto
                {
                    CostName = "设备使用费",
                    CalculationFormula = "项目费用之和（年费用）",
                    DataSource = $"项目数量: {entity.Projects?.Count ?? 0}个, 项目费用: {string.Join(", ", entity.Projects?.Select(p => $"{p.ProjectType}:{p.Amount}元") ?? new string[0])}",
                    CalculationProcess = $"项目费用总和 = {entity.EquipmentUsageFee}元",
                    FinalResult = entity.EquipmentUsageFee,
                    Parameters = new Dictionary<string, object>
                    {
                        { "ProjectCount", entity.Projects?.Count ?? 0 },
                        { "TotalProjectAmount", entity.EquipmentUsageFee ?? 0 }
                    }
                },
                LaborCost = new CostDetailDto
                {
                    CostName = "人力成本",
                    CalculationFormula = "人员岗位数量 × 人员年工时 × 人力成本单价（年费用）",
                    DataSource = $"人员数量: {staffCount}人, 年工时: {staffWorkingHours}小时, 人力成本单价: {entity.LaborCostPerHour}元/小时",
                    CalculationProcess = $"{staffCount} × {staffWorkingHours} × {entity.LaborCostPerHour} = {entity.LaborCost}元",
                    FinalResult = entity.LaborCost,
                    Parameters = new Dictionary<string, object>
                    {
                        { "StaffCount", staffCount },
                        { "WorkingHours", staffWorkingHours },
                        { "LaborCostPerHour", entity.LaborCostPerHour ?? 0 }
                    }
                },
                ElectricityCost = new CostDetailDto
                {
                    CostName = "电费",
                    CalculationFormula = "调整后总工作时长 × 系统能耗 × 电费单价",
                    DataSource = $"调整后总工作时长: {adjustedWorkingHours}小时, 系统能耗: {entity.SystemEnergyConsumption}千瓦时, 电费单价: 1元/千瓦时",
                    CalculationProcess = $"{adjustedWorkingHours} × {entity.SystemEnergyConsumption} × 1 = {entity.ElectricityCost}元",
                    FinalResult = entity.ElectricityCost,
                    Parameters = new Dictionary<string, object>
                    {
                        { "AdjustedWorkingHours", adjustedWorkingHours },
                        { "SystemEnergyConsumption", entity.SystemEnergyConsumption ?? 0 },
                        { "ElectricityUnitPrice", 1m }
                    }
                },
                FuelPowerCost = new CostDetailDto
                {
                    CostName = "燃料动力费",
                    CalculationFormula = "燃料动力费单价 × 调整后总工作时长",
                    DataSource = $"燃料动力费单价: {entity.FuelPowerCostPerHour}万元/小时, 调整后总工作时长: {adjustedWorkingHours}小时",
                    CalculationProcess = $"{entity.FuelPowerCostPerHour} × {adjustedWorkingHours} = {entity.FuelPowerCost}元",
                    FinalResult = entity.FuelPowerCost,
                    Parameters = new Dictionary<string, object>
                    {
                        { "FuelPowerCostPerHour", entity.FuelPowerCostPerHour ?? 0 },
                        { "AdjustedWorkingHours", adjustedWorkingHours }
                    }
                },
                EquipmentMaintenanceCost = new CostDetailDto
                {
                    CostName = "设备保养费用",
                    CalculationFormula = "设备使用费 × 保养费率",
                    DataSource = $"设备使用费: {entity.EquipmentUsageFee}元, 保养费率: 5%",
                    CalculationProcess = $"{entity.EquipmentUsageFee} × 0.05 = {entity.EquipmentMaintenanceCost}元",
                    FinalResult = entity.EquipmentMaintenanceCost,
                    Parameters = new Dictionary<string, object>
                    {
                        { "EquipmentUsageFee", entity.EquipmentUsageFee ?? 0 },
                        { "MaintenanceRate", 0.05m }
                    }
                },
                SystemIdleCost = new CostDetailDto
                {
                    CostName = "系统空置成本",
                    CalculationFormula = "空置天数 × (厂房使用费/365 + 设备使用费/365 + 设备保养费/365)",
                    DataSource = $"空置天数: {systemStats.IdleDays}天, 日均厂房费: {(entity.FactoryUsageFee ?? 0) / 365}元, 日均设备费: {(entity.EquipmentUsageFee ?? 0) / 365}元, 日均保养费: {(entity.EquipmentMaintenanceCost ?? 0) / 365}元",
                    CalculationProcess = $"{systemStats.IdleDays} × ({(entity.FactoryUsageFee ?? 0) / 365} + {(entity.EquipmentUsageFee ?? 0) / 365} + {(entity.EquipmentMaintenanceCost ?? 0) / 365}) = {entity.SystemIdleCost}元",
                    FinalResult = entity.SystemIdleCost,
                    Parameters = new Dictionary<string, object>
                    {
                        { "IdleDays", systemStats.IdleDays },
                        { "DailyFactoryFee", (entity.FactoryUsageFee ?? 0) / 365 },
                        { "DailyEquipmentFee", (entity.EquipmentUsageFee ?? 0) / 365 },
                        { "DailyMaintenanceFee", (entity.EquipmentMaintenanceCost ?? 0) / 365 }
                    }
                },
                SystemExperimentCost = new CostDetailDto
                {
                    CostName = "系统试验成本",
                    CalculationFormula = "工作天数 × (厂房使用费/365 + 设备使用费/365 + 人力成本/365 + 电费/365 + 燃料动力费/365 + 设备保养费/365)",
                    DataSource = $"工作天数: {systemStats.WorkingDays}天, 各项日均费用: 厂房{(entity.FactoryUsageFee ?? 0) / 365}元, 设备{(entity.EquipmentUsageFee ?? 0) / 365}元, 人力{(entity.LaborCost ?? 0) / 365}元, 电费{(entity.ElectricityCost ?? 0) / 365}元, 燃料{(entity.FuelPowerCost ?? 0) / 365}元, 保养{(entity.EquipmentMaintenanceCost ?? 0) / 365}元",
                    CalculationProcess = $"{systemStats.WorkingDays} × ({(entity.FactoryUsageFee ?? 0) / 365} + {(entity.EquipmentUsageFee ?? 0) / 365} + {(entity.LaborCost ?? 0) / 365} + {(entity.ElectricityCost ?? 0) / 365} + {(entity.FuelPowerCost ?? 0) / 365} + {(entity.EquipmentMaintenanceCost ?? 0) / 365}) = {entity.SystemExperimentCost}元",
                    FinalResult = entity.SystemExperimentCost,
                    Parameters = new Dictionary<string, object>
                    {
                        { "WorkingDays", systemStats.WorkingDays },
                        { "DailyFactoryFee", (entity.FactoryUsageFee ?? 0) / 365 },
                        { "DailyEquipmentFee", (entity.EquipmentUsageFee ?? 0) / 365 },
                        { "DailyLaborFee", (entity.LaborCost ?? 0) / 365 },
                        { "DailyElectricityFee", (entity.ElectricityCost ?? 0) / 365 },
                        { "DailyFuelPowerFee", (entity.FuelPowerCost ?? 0) / 365 },
                        { "DailyMaintenanceFee", (entity.EquipmentMaintenanceCost ?? 0) / 365 }
                    }
                }
            };
        }

        /// <summary>
        /// 使用缓存计算系统工作统计数据（优化版本）
        /// </summary>
        private SystemWorkingStats CalculateSystemWorkingStatsWithCache(string systemName, SystemDataCache cache)
        {
            try
            {
                var roomId = GetRoomIdBySystemName(systemName);
                if (roomId == Guid.Empty)
                {
                    return new SystemWorkingStats();
                }

                // 从缓存获取系统设备
                if (!cache.RoomEquipmentMap.TryGetValue(roomId, out var systemEquipIds))
                {
                    return new SystemWorkingStats();
                }

                // 从缓存获取试验任务
                var systemTasks = cache.AllTasks.Where(t => t.SysName == systemName).ToList();
                if (!systemTasks.Any())
                {
                    return new SystemWorkingStats();
                }

                DateTime earliestDate = DateTime.MaxValue;
                DateTime latestDate = DateTime.MinValue;
                int totalWorkingDays = 0;
                int totalIdleDays = 0;
                decimal totalWorkingHours = 0;

                // 遍历所有试验任务
                foreach (var task in systemTasks)
                {
                    if (!DateTime.TryParse(task.TaskStartTime, out DateTime start) ||
                        !DateTime.TryParse(task.TaskEndTime, out DateTime end))
                    {
                        continue;
                    }

                    if (start.Date > DateTime.Now.Date) continue;

                    if (start < earliestDate) earliestDate = start.Date;
                    if (end > latestDate) latestDate = end.Date > DateTime.Now.Date ? DateTime.Now.Date : end.Date;

                    // 计算单个任务的实际天数（超过今天的任务只算到今天）
                    DateTime taskEndDate = end.Date > DateTime.Now.Date ? DateTime.Now.Date : end.Date;
                    int taskDays = (int)(taskEndDate - start.Date).TotalDays + 1;

                    // 使用缓存的运行时间数据计算
                    uint taskSeconds = CalculateTaskSecondsWithCache(systemEquipIds, start.Date, taskEndDate, cache);

                    if (taskSeconds > 0)
                    {
                        totalWorkingDays += taskDays;
                        totalWorkingHours += Math.Round((decimal)taskSeconds / 3600, 2);
                    }
                    else
                    {
                        totalWorkingDays += taskDays;
                        totalWorkingHours += taskDays * 8;
                    }
                }

                if (earliestDate == DateTime.MaxValue)
                {
                    return new SystemWorkingStats();
                }

                // 计算总天数（从最早任务开始到最晚任务结束，但不包含"未来"）
                // 如果latestDate是今天，说明有任务延续到今天或未来，今天还未结束，不计入闲置计算
                DateTime effectiveLatestDate = latestDate >= DateTime.Now.Date ? DateTime.Now.Date.AddDays(-1) : latestDate;
                
                int totalDays;
                // 确保有效的日期范围（至少要有一天）
                if (effectiveLatestDate < earliestDate)
                {
                    // 所有任务都是从今天开始的，没有过去的历史，不计算闲置
                    totalDays = 0;
                    totalIdleDays = 0;
                }
                else
                {
                    totalDays = (int)(effectiveLatestDate - earliestDate).TotalDays + 1;
                    totalIdleDays = totalDays - totalWorkingDays;
                    // 确保闲置天数不为负
                    if (totalIdleDays < 0) totalIdleDays = 0;
                }

                return new SystemWorkingStats
                {
                    EarliestDate = earliestDate,
                    TotalDays = totalDays,
                    WorkingDays = totalWorkingDays,
                    IdleDays = totalIdleDays,
                    TotalWorkingHours = totalWorkingHours,
                    EquipmentCount = systemEquipIds.Count
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"计算系统工作统计数据失败: {ex.Message}");
                return new SystemWorkingStats();
            }
        }

        /// <summary>
        /// 使用缓存计算调整后总工作时长（优化版本）
        /// </summary>
        private decimal CalculateAdjustedTotalWorkingHoursWithCache(string systemName, SystemDataCache cache)
        {
            try
            {
                var systemTasks = cache.AllTasks.Where(t => t.SysName == systemName).ToList();
                if (!systemTasks.Any())
                {
                    return 0;
                }

                var roomId = GetRoomIdBySystemName(systemName);
                if (roomId == Guid.Empty) return 0;

                if (!cache.RoomEquipmentMap.TryGetValue(roomId, out var systemEquipIds))
                {
                    systemEquipIds = new List<Guid>();
                }

                decimal totalHours = 0;
                foreach (var task in systemTasks)
                {
                    if (!DateTime.TryParse(task.TaskStartTime, out DateTime start) ||
                        !DateTime.TryParse(task.TaskEndTime, out DateTime end))
                    {
                        continue;
                    }

                    if (start.Date > DateTime.Now.Date) continue;

                    // 计算任务的实际结束日期（超过今天的任务只算到今天）
                    DateTime taskEndDate = end.Date > DateTime.Now.Date ? DateTime.Now.Date : end.Date;

                    uint seconds = CalculateTaskSecondsWithCache(systemEquipIds, start.Date, taskEndDate, cache);

                    if (seconds > 0)
                    {
                        decimal taskHours = Math.Round((decimal)seconds / 3600, 2);
                        totalHours += taskHours;
                    }
                    else
                    {
                        int days = (int)(taskEndDate - start.Date).TotalDays + 1;
                        if (days > 0)
                        {
                            totalHours += days * 8;
                        }
                    }
                }

                return totalHours;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"计算调整后工作时长失败: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// 使用缓存计算人员工作数据（优化版本）
        /// </summary>
        private (int staffCount, decimal workingHours) CalculateStaffWorkingDataWithCache(string systemName, SystemDataCache cache)
        {
            try
            {
                var systemTasks = cache.AllTasks.Where(t => t.SysName == systemName).ToList();
                if (!systemTasks.Any())
                {
                    return (0, 0);
                }

                int totalStaffCount = 0;
                decimal totalWorkingHours = 0;

                var roomId = GetRoomIdBySystemName(systemName);
                cache.RoomEquipmentMap.TryGetValue(roomId, out var systemEquipIds);

                foreach (var task in systemTasks)
                {
                    // 计算人员岗位数量
                    int taskStaffCount = 0;
                    if (!string.IsNullOrEmpty(task.SimuResp)) taskStaffCount += 1;
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
                        // 计算任务的实际结束日期（超过今天的任务只算到今天）
                        DateTime taskEndDate = endTime.Date > DateTime.Now.Date ? DateTime.Now.Date : endTime.Date;

                        if (systemEquipIds != null && systemEquipIds.Any())
                        {
                            uint totalSeconds = CalculateTaskSecondsWithCache(systemEquipIds, startTime.Date, taskEndDate, cache);

                            if (totalSeconds > 0)
                            {
                                totalWorkingHours += Math.Round((decimal)totalSeconds / 3600, 2);
                            }
                            else
                            {
                                int days = (int)(taskEndDate - startTime.Date).TotalDays + 1;
                                totalWorkingHours += days * 8;
                            }
                        }
                        else
                        {
                            int days = (int)(taskEndDate - startTime.Date).TotalDays + 1;
                            totalWorkingHours += days * 8;
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
        /// 使用缓存计算任务期间的运行秒数（辅助方法）
        /// </summary>
        private uint CalculateTaskSecondsWithCache(List<Guid> equipIds, DateTime startDate, DateTime endDate, SystemDataCache cache)
        {
            if (!equipIds.Any()) return 0;

            // 注意：这里需要实际查询数据库，因为缓存中只存储了汇总数据
            // 但我们可以优化为只查询必要的日期范围
            try
            {
                var seconds = DbContext.Queryable<EquipDailyRuntime>()
                    .Where(x => equipIds.Contains(x.EquipId))
                    .Where(x => x.RecordDate >= startDate && x.RecordDate <= endDate)
                    .Sum(x => x.RunningSeconds);
                return seconds;
            }
            catch
            {
                return 0;
            }
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
