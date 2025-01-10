using Hgzn.Mes.Application.Main.Dtos.Base;

namespace Hgzn.Mes.Application.Main.Dtos
{
    public class ExceptionReadDto : ReadDto
    {
        public string Info { get; set; } = null!;

        public string? StackTrace { get; set; }

        public string? Inner { get; set; }
    }
}