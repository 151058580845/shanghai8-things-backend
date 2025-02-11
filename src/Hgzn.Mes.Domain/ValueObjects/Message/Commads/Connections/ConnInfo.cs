using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message.Base;

namespace Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections
{
    public class ConnInfo : CommandBase, IConnInfo
    {
        public ConnType ConnType { get; set; }

        public string EquipType { get; set; } = null!;

        public string? ConnString { get; set; }
    }
}
