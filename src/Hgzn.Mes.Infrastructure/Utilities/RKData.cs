using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities
{
    public static class RKData
    {
        public static Dictionary<Guid, Tuple<float, float>> RoomId_TemperatureAndHumidness = new Dictionary<Guid, Tuple<float, float>>();
    }
}
