using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message.Base;

namespace Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections
{
    public class ConnInfo<TContent> : ConnInfoBase
        where TContent : IConnInfo
    {
        public TContent? Content { get; set; }
    }
}
