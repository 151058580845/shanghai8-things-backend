using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Shared.Enums
{
    /// <summary>
    /// modbus读取寄存器修改
    /// </summary>
    public enum ModbusReadType
    {
        [Description("读取线圈状态")]
        ReadCoil = 1,
        [Description("读取离散输入状态")]
        ReadDiscrete = 2,
        [Description("读取保持寄存器")]
        ReadInput = 3,
        [Description("读取输入寄存器")]
        ReadHoldingRegister = 4,
    }
}
