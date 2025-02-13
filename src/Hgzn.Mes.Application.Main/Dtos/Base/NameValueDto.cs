namespace Hgzn.Mes.Application.Main.Dtos.Base;

public class NameValueDto
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Value { get; set; }
}

public class NameValueListDto
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Value { get; set; }
    public Guid? ParentId { get; set; }
    public List<NameValueListDto> Children { get; set; }=new List<NameValueListDto>();
}