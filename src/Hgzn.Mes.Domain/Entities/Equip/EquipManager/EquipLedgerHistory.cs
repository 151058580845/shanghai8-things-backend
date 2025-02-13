using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipManager;

public class EquipLedgerHistory: UniversalEntity,ICreationAudited
{
    [Description("设备主键")]
    public Guid EquipId { get; set; }

    [Description("设备名称")]
    public string EquipCode { get; set; } = null!;

    [Description("所在房间")]
    public Guid? RoomId { get; set; }
    
    [Description("操作时间")]
    public DateTime OperatorTime { get; set; }

    /*
     * 搜索为1
     * 盘点为2
     * */
    [Description("操作类型")]
    public int Type { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
}