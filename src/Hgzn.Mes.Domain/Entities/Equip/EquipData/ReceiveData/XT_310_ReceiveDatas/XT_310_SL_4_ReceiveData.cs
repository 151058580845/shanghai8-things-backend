using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_310_ReceiveDatas
{
    /// <summary>
    /// 310_固定电源
    /// </summary>
    public class XT_310_SL_4_ReceiveData : UniversalEntity, IAudited
    {
        public Guid? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }

        [Description("仿真试验系统识别编码")]
        public byte SimuTestSysld { get; set; }

        [Description("设备类型识别编码")]
        public byte DevTypeld { get; set; }

        [Description("本机识别编码")]
        public string? Compld { get; set; }

        #region 工作模式信息

        [Description("本地还是远程")]
        public byte LocalOrRemote { get; set; }

        [Description("电源数量")]
        public byte PowerSupplyCount { get; set; }

        [Description("电源类型1")]
        public byte PowerSupplyType1 { get; set; }

        [Description("电源类型2")]
        public byte PowerSupplyType2 { get; set; }

        [Description("是否上电")]
        public byte IsPoweredOn { get; set; }

        [Description("预留")]
        public byte Reserved { get; set; }

        #endregion

        #region 健康状态信息

        [Description("状态类型")]
        public byte StatusType { get; set; }

        [Description("工作状态")]
        public byte OperationStatus { get; set; }

        #endregion

        #region 物理量

        [Description("物理量参数数量")]
        public uint PhysicalParameterCount { get; set; }

        [Description("电源1电压设置值")]
        public float Power1VoltageSet { get; set; }

        [Description("电源1电流设置值")]
        public float Power1CurrentSet { get; set; }

        [Description("电源2电压设置值")]
        public float Power2VoltageSet { get; set; }

        [Description("电源2电流设置值")]
        public float Power2CurrentSet { get; set; }

        [Description("电源3电压设置值")]
        public float Power3VoltageSet { get; set; }

        [Description("电源3电流设置值")]
        public float Power3CurrentSet { get; set; }

        [Description("电源4电压设置值")]
        public float Power4VoltageSet { get; set; }

        [Description("电源4电流设置值")]
        public float Power4CurrentSet { get; set; }

        [Description("电源5电压设置值")]
        public float Power5VoltageSet { get; set; }

        [Description("电源5电流设置值")]
        public float Power5CurrentSet { get; set; }

        [Description("电源6电压设置值")]
        public float Power6VoltageSet { get; set; }

        [Description("电源6电流设置值")]
        public float Power6CurrentSet { get; set; }

        [Description("电源7电压设置值")]
        public float Power7VoltageSet { get; set; }

        [Description("电源7电流设置值")]
        public float Power7CurrentSet { get; set; }

        [Description("电源8电压设置值")]
        public float Power8VoltageSet { get; set; }

        [Description("电源8电流设置值")]
        public float Power8CurrentSet { get; set; }

        [Description("电源1电压采集值")]
        public float Power1VoltageRead { get; set; }

        [Description("电源1电流采集值")]
        public float Power1CurrentRead { get; set; }

        [Description("电源2电压采集值")]
        public float Power2VoltageRead { get; set; }

        [Description("电源2电流采集值")]
        public float Power2CurrentRead { get; set; }

        [Description("电源3电压采集值")]
        public float Power3VoltageRead { get; set; }

        [Description("电源3电流采集值")]
        public float Power3CurrentRead { get; set; }

        [Description("电源4电压采集值")]
        public float Power4VoltageRead { get; set; }

        [Description("电源4电流采集值")]
        public float Power4CurrentRead { get; set; }

        [Description("电源5电压采集值")]
        public float Power5VoltageRead { get; set; }

        [Description("电源5电流采集值")]
        public float Power5CurrentRead { get; set; }

        [Description("电源6电压采集值")]
        public float Power6VoltageRead { get; set; }

        [Description("电源6电流采集值")]
        public float Power6CurrentRead { get; set; }

        [Description("电源7电压采集值")]
        public float Power7VoltageRead { get; set; }

        [Description("电源7电流采集值")]
        public float Power7CurrentRead { get; set; }

        [Description("电源8电压采集值")]
        public float Power8VoltageRead { get; set; }

        [Description("电源8电流采集值")]
        public float Power8CurrentRead { get; set; }

        #endregion

        [Description("运行时间")]
        public uint? RunTime { get; set; }

        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int CreatorLevel { get; set; } = 0;

        public static XT_310_SL_4_ReceiveData[] Seeds { get; } = new XT_310_SL_4_ReceiveData[]
        {
            new XT_310_SL_4_ReceiveData() // 凌晨数据（低负载）
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-4789-abc1-123456789001"),
                CreationTime = DateTime.Today.AddHours(2.5), // 02:30
                SimuTestSysld = 1,
                DevTypeld = 2,
                Compld = "123456789987654321",
                LocalOrRemote = 1,
                PowerSupplyCount = 1,
                PowerSupplyType1 = 1,
                PowerSupplyType2 = 2,
                IsPoweredOn = 1,
                Reserved = 0,
                StatusType = 1,
                OperationStatus = 1,
                PhysicalParameterCount = 32,
                // 电源1-8的设置值（V/A）
                Power1VoltageSet = 12, Power1CurrentSet = 1.2f,
                Power2VoltageSet = 5,  Power2CurrentSet = 0.8f,
                Power3VoltageSet = 3.3f, Power3CurrentSet = 1.5f,
                Power4VoltageSet = 24, Power4CurrentSet = 0.5f,
                Power5VoltageSet = 12, Power5CurrentSet = 1.0f,
                Power6VoltageSet = 5,  Power6CurrentSet = 2.0f,
                Power7VoltageSet = 3.3f, Power7CurrentSet = 0.3f,
                Power8VoltageSet = 48, Power8CurrentSet = 0.2f,
                // 读取值（±10%浮动）
                Power1VoltageRead = 11.8f, Power1CurrentRead = 1.18f,
                Power2VoltageRead = 4.9f,  Power2CurrentRead = 0.82f,
                Power3VoltageRead = 3.28f, Power3CurrentRead = 1.52f,
                Power4VoltageRead = 23.7f, Power4CurrentRead = 0.48f,
                Power5VoltageRead = 12.1f, Power5CurrentRead = 0.98f,
                Power6VoltageRead = 5.2f,  Power6CurrentRead = 1.95f,
                Power7VoltageRead = 3.35f, Power7CurrentRead = 0.31f,
                Power8VoltageRead = 47.5f, Power8CurrentRead = 0.19f,
                RunTime = 1200,
                LastModificationTime = DateTime.Today.AddHours(2.6)
            },
            // 上午8:25数据（中等负载）
            new XT_310_SL_4_ReceiveData()
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-4789-abc1-123456789002"),
                CreationTime = DateTime.Today.AddHours(8).AddMinutes(25), // 08:25
                SimuTestSysld = 1,
                DevTypeld = 2,
                Compld = "123456789987654321",
                LocalOrRemote = 1,
                PowerSupplyCount = 1,
                PowerSupplyType1 = 1,
                PowerSupplyType2 = 2,
                IsPoweredOn = 1,
                Reserved = 0,
                StatusType = 1,
                OperationStatus = 1,
                PhysicalParameterCount = 32,
                // 电源设置值（V/A）
                Power1VoltageSet = 12f,   Power1CurrentSet = 2.5f,
                Power2VoltageSet = 5f,    Power2CurrentSet = 1.2f,
                Power3VoltageSet = 3.3f,  Power3CurrentSet = 2.0f,
                Power4VoltageSet = 24f,   Power4CurrentSet = 1.0f,
                Power5VoltageSet = 12f,   Power5CurrentSet = 1.8f,
                Power6VoltageSet = 5f,    Power6CurrentSet = 0.9f,
                Power7VoltageSet = 3.3f,  Power7CurrentSet = 1.5f,
                Power8VoltageSet = 48f,   Power8CurrentSet = 0.3f,
                // 读取值（±5%浮动）
                Power1VoltageRead = 11.8f, Power1CurrentRead = 2.45f,
                Power2VoltageRead = 4.9f,  Power2CurrentRead = 1.22f,
                Power3VoltageRead = 3.28f, Power3CurrentRead = 2.05f,
                Power4VoltageRead = 23.7f, Power4CurrentRead = 0.98f,
                Power5VoltageRead = 12.1f, Power5CurrentRead = 1.77f,
                Power6VoltageRead = 5.2f, Power6CurrentRead = 0.88f,
                Power7VoltageRead = 3.35f, Power7CurrentRead = 1.48f,
                Power8VoltageRead = 47.5f, Power8CurrentRead = 0.29f,
                RunTime = 5600,
                LastModificationTime = DateTime.Today.AddHours(8).AddMinutes(30)
            },
            // 上午10:00数据（峰值负载）
            new XT_310_SL_4_ReceiveData()
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-4789-abc1-123456789003"),
                CreationTime = DateTime.Today.AddHours(10), // 10:00
                SimuTestSysld = 1,
                DevTypeld = 2,
                Compld = "123456789987654321",
                LocalOrRemote = 1,
                PowerSupplyCount = 1,
                PowerSupplyType1 = 1,
                PowerSupplyType2 = 2,
                IsPoweredOn = 1,
                Reserved = 0,
                StatusType = 1,
                OperationStatus = 1,
                PhysicalParameterCount = 32,
                // 电源设置值（V/A）- 电流显著增大
                Power1VoltageSet = 12f,   Power1CurrentSet = 4.0f,
                Power2VoltageSet = 5f,    Power2CurrentSet = 2.5f,
                Power3VoltageSet = 3.3f,  Power3CurrentSet = 3.0f,
                Power4VoltageSet = 24f,   Power4CurrentSet = 1.8f,
                Power5VoltageSet = 12f,   Power5CurrentSet = 3.2f,
                Power6VoltageSet = 5f,    Power6CurrentSet = 1.5f,
                Power7VoltageSet = 3.3f,  Power7CurrentSet = 2.8f,
                Power8VoltageSet = 48f,   Power8CurrentSet = 0.5f,
                // 读取值（±5%浮动，部分电源电压略有下降）
                Power1VoltageRead = 11.6f, Power1CurrentRead = 3.92f,
                Power2VoltageRead = 4.8f,  Power2CurrentRead = 2.45f,
                Power3VoltageRead = 3.25f, Power3CurrentRead = 2.95f,
                Power4VoltageRead = 23.5f, Power4CurrentRead = 1.77f,
                Power5VoltageRead = 11.9f, Power5CurrentRead = 3.15f,
                Power6VoltageRead = 4.9f,  Power6CurrentRead = 1.48f,
                Power7VoltageRead = 3.28f, Power7CurrentRead = 2.75f,
                Power8VoltageRead = 47.2f, Power8CurrentRead = 0.49f,
                RunTime = 7200,
                LastModificationTime = DateTime.Today.AddHours(10).AddMinutes(5)
            }
        };
    }
}
