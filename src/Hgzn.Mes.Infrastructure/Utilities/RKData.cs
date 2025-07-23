using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities
{
    public static class RKData
    {
        // 这个温湿度计不需要持久化数据,我直接用静态的内存
        public static Dictionary<Guid, Tuple<float, float>> RoomId_TemperatureAndHumidness = new Dictionary<Guid, Tuple<float, float>>();
    }
}
