
namespace Hgzn.Mes.Domain.Shared.Enums
{
    public enum CmdType
    {
        Conn,
    }

    public enum ConnType
    {
        Scoket,
        Com,
        ModbusRtu,
        Http
    }

    public enum ConnOperationType
    {
        Open,
        Close,
        Start
    }
}
