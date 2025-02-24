using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipDataPoint
{
    public class ModbusTcpDataPoint
    {

        [Description("读取长度")]
        public ushort Address { get; set; } = 1;

        [Description("读取长度")]
        public ushort ReadLength { get; set; } = 1;

        [Description("读取类型")]
        public DataReadTypeEnum ReadTypeEnum { get; set; }

        [Description("modbus读取类型")]
        public ModbusReadType? ModbusReadType { get; set; }

        [Description("读取线圈")]
        public int ReadCoil { get; set; }

        [Description("modbus写入类型")]
        public ModbusWriteType? ModbusWriteType { get; set; }

        [Description("采集模式设置")]
        public CollectionConfig? CollectionConfig { get; set; }
    }
}
