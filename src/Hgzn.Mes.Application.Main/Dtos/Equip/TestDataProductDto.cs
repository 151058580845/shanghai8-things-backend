using System.ComponentModel;
using Hgzn.Mes.Application.Main.Dtos.Base;

namespace Hgzn.Mes.Application.Main.Dtos.Equip;

public class TestDataProductReadDto : ReadDto
{
    [Description("实验计划主键")]
    public Guid TestDataId { get; set; }
    [Description("名称")]
    public string Name { get; set; }
    [Description("编号")]
    public string Code { get; set; }
    [Description("技术状")]
    public string TechnicalStatus { get; set; }
}

public class TestDataProductCreateDto : CreateDto
{
    [Description("实验计划主键")]
    public Guid TestDataId { get; set; }
    [Description("名称")]
    public string Name { get; set; }
    [Description("编号")]
    public string Code { get; set; }
    [Description("技术状")]
    public string TechnicalStatus { get; set; }
}

public class TestDataProductUpdateDto : UpdateDto
{
    [Description("实验计划主键")]
    public Guid TestDataId { get; set; }
    [Description("名称")]
    public string Name { get; set; }
    [Description("编号")]
    public string Code { get; set; }
    [Description("技术状")]
    public string TechnicalStatus { get; set; }
}

public class TestDataProductQueryDto : PaginatedQueryDto
{
    [Description("实验计划主键")]
    public Guid? TestDataId { get; set; }
}