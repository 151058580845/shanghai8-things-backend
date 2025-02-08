using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message.Base;

namespace Hgzn.Mes.Domain.ValueObjects.Message.Commads
{
    public class CommandBase : ICommand
    {
        public CmdType Type { get; set; }
    }
}
