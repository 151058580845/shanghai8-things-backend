using Hgzn.Mes.Application.Dtos.Base;

namespace Hgzn.Mes.Application.Dtos
{
    public class ExceptionReadDto : ReadDto
    {
        public string Info { get; set; } = null!;

        public string? StackTrace { get; set; }

        public string? Inner { get; set; }
    }
}