using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Shared.Enums
{
    /// <summary>
    /// modbus写入寄存器
    /// </summary>
    public enum ModbusWriteType
    {
        [Description("写入线圈状态")]
        WriteCoil = 1,
        [Description("写入离散输入状态")]
        WriteDiscrete = 2,
        [Description("写入保持寄存器")]
        WriteInput = 3,
        [Description("写入输入寄存器")]
        WriteHoldingRegister = 4,
    }
}
