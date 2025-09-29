using System.ComponentModel;
using Hgzn.Mes.Application.Main.Dtos.Base;

namespace Hgzn.Mes.Application.Main.Dtos.Equip;

public class TestDataUSTReadDto : ReadDto
{
    [Description("试验计划主键")]
    public string? TestDataId { get; set; }
    
    [Description("编号")]
    public string? Code { get; set; }
    
    [Description("型号规格")]
    public string? ModelSpecification { get; set; }
    
    [Description("名称")]
    public string? Name { get; set; }
    
    [Description("有效期")]
    public string? ValidityPeriod { get; set; }
}

public class TestDataUSTCreateDto : CreateDto
{
    [Description("试验计划主键")]
    public string? TestDataId { get; set; }
    
    [Description("编号")]
    public string? Code { get; set; }
    
    [Description("型号规格")]
    public string? ModelSpecification { get; set; }
    
    [Description("名称")]
    public string? Name { get; set; }
    
    [Description("有效期")]
    public string? ValidityPeriod { get; set; }
}

public class TestDataUSTUpdateDto : UpdateDto
{
    [Description("试验计划主键")]
    public string? TestDataId { get; set; }
    
    [Description("编号")]
    public string? Code { get; set; }
    
    [Description("型号规格")]
    public string? ModelSpecification { get; set; }
    
    [Description("名称")]
    public string? Name { get; set; }
    
    [Description("有效期")]
    public string? ValidityPeriod { get; set; }
}

public class TestDataUSTQueryDto : PaginatedQueryDto
{
    [Description("试验计划主键")]
    public string? TestDataId { get; set; }
}
