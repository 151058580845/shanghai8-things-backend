using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_307_ReceiveDatas;
using Hgzn.Mes.Domain.Services;
using Hgzn.Mes.Domain.Shared.Enums;
using SqlSugar;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData;


[SplitTable(SplitType.Month,typeof(SplitTableService))]//按月分表
[SugarTable("SplitTestTable_{year}{month}{day}")]
public class Receive:AggregateRoot
{
    [SugarColumn(IsPrimaryKey =true)]
    public Guid Id { get; set; }
    
    [Required]
    [SugarColumn(ColumnDataType = "json",IsJson = true)]
    public Object Content { get; set; } = null!;

    [Services.SplitField(Priority=1)]
    public byte? SimuTestSysld { get; set; }
    
    [Services.SplitField(Priority=2)]
    public byte? DevTypeld { get; set; }
    
    [Services.SplitField(Priority=3)]
    public string? Compld { get; set; }
    
    [Services.SplitField(Priority=4,Format="yyyyMM")]
    public DateTime? CreateTime { get; set; }
    
    [SugarColumn(IsIgnore = true)]
    public string Time { get; set; }
}