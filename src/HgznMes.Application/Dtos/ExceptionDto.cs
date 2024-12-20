using HgznMes.Application.Dtos.Base;

namespace HgznMes.Application.Dtos
{
    public class ExceptionReadDto : ReadDto
    {
        public string Info { get; set; } = null!;

        public string? StackTrace { get; set; }

        public string? Inner { get; set; }
    }
}