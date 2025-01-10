using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Hgzn.Mes.Domain.Entities.System.Base;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipManager;

[Table("EquipType")]
[Description("设备类型")]
public class EquipType : UniversalEntity,ISoftDelete, IState
{

    [Description("设备类型编号")]
    public required string TypeCode { get; set; }

    [Description("设备类型名称")]
    public required string TypeName { get; set; }

    [Description("设备描述")]
    public string? Description { get; set; }

    [Description("设备图标")]
    public string? Icon { get; set; }

    [Description("排序号")]
    public int OrderNum { get; set; } = 0;

    [Description("上级Id")]
    public Guid? ParentId { get; set; }

    [Description("设备创建时间")]
    public DateTime CreationTime { get; set; }

    [Description("设备最后修改时间")]
    public DateTime? LastModificationTime { get; set; }

    [Description("软删除")]
    public bool IsDeleted { get; set; }

    public bool State { get; set; }

    public Guid? CreatorId { get; set; }

    /// <summary>
    /// 这个设备类型提前配置的可以采集的数据列表
    /// 采集数据列表
    /// </summary>
    // [Navigate(NavigateType.OneToMany, nameof(EquipDataEntity.TypeId))]
    // public List<EquipDataEntity>? EquipTypeDataEntities { get; set; }
    public bool SoftDeleted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime? DeleteTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}