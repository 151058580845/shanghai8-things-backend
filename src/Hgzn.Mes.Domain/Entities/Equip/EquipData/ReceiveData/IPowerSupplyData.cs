using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData
{
    public interface IPowerSupplyData
    {
        byte PowerSupplyCount { get; }
        DateTime CreationTime { get; }

        // 电压读取属性
        float Power1VoltageRead { get; }
        float Power2VoltageRead { get; }
        float Power3VoltageRead { get; }
        float Power4VoltageRead { get; }
        float Power5VoltageRead { get; }
        float Power6VoltageRead { get; }
        float Power7VoltageRead { get; }
        float Power8VoltageRead { get; }

        // 电流读取属性
        float Power1CurrentRead { get; }
        float Power2CurrentRead { get; }
        float Power3CurrentRead { get; }
        float Power4CurrentRead { get; }
        float Power5CurrentRead { get; }
        float Power6CurrentRead { get; }
        float Power7CurrentRead { get; }
        float Power8CurrentRead { get; }
    }
}
