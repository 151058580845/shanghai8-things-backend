using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Shared.Enums
{
    /*
 *  ABCD：按顺序排列，高字节在前，高字在前，适用于大端模式的设备。
    BADC：字节交换，字的顺序不变，适用于字节序需要调整的情况。
    CDAB：字交换，字内字节顺序不变，适用于小端模式的设备。
    DCBA：全部字节交换，适用于字节和字的顺序都需要调整的情况。
 */
    public enum DataOrderType
    {
        ABCD,
        BADC,
        CDAB,
        DCBA
    }
}
