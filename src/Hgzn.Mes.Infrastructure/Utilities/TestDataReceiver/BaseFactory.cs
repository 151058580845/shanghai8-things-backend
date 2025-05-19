using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver
{
    public enum LocalOrOnline
    {
        Local,
        Online,
    }


    public class BaseFactory
    {
        public Dictionary<string, IReceive> Receives = new Dictionary<string, IReceive>();
        public BaseFactory() { }
        public string GetKey(LocalOrOnline localOrOnline, byte simuTestSysId, byte devTypeId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(localOrOnline.ToString());
            sb.Append(simuTestSysId);
            sb.Append(devTypeId);
            return sb.ToString();
        }
    }
}
