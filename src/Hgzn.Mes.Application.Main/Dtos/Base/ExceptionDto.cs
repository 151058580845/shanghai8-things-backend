namespace Hgzn.Mes.Application.Main.Dtos.Base
{
    public class ExceptionReadDto : ReadDto
    {
        public string Info { get; set; } = null!;

        public string? StackTrace { get; set; }

        public string? Inner { get; set; }
    }
}