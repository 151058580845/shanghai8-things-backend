namespace Hgzn.Mes.Application.Main.Dtos.Base;

public class TreeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid? ParentId { get; set; }
}