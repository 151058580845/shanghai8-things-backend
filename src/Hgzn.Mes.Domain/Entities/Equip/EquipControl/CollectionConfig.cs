using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipControl;

/// <summary>
/// 采集模式设置
/// </summary>
[Table("CollectionConfig")]
[Description("设备采集模式设置")]
public class CollectionConfigEntity : UniversalEntity,ICreationAudited
{
    /// <summary>
    /// 采集间隔时间（多久采集一次）
    /// </summary>
    public int IntervalTime { get; set; } = 1000;
    /// <summary>
    /// 开始时间(日期时间)
    /// </summary>
    public DateTime StartTime { get; set; }
    /// <summary>
    /// 结束时间(日期时间)
    /// </summary>
    public DateTime EndTime { get; set; }
    /// <summary>
    /// 一天中的时间，采集开始时间
    /// </summary>
    public IEnumerable<CollectTime> CollectionTime { get; set; } = [];

    public IEnumerable<DayOfWeek> CollectDays { get; set; } = [];
    
    /// <summary>
    /// 传输模式
    /// </summary>
    public TransmissionMode TransmissionMode { get; set; }
    /// <summary>
    /// 传输目的地
    /// </summary>
    public TranDestination TranDestination { get; set; }
    /// <summary>
    /// 是否启用日志记录
    /// </summary>
    public bool? EnableLogging { get; set; }
    /// <summary>
    /// 日志文件路径
    /// </summary>
    public string? LogFilePath { get; set; }
    
    public class CollectTime
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
    public bool IsValid()
    {
        // 检查采集时间设置是否有效
        if (StartTime > EndTime)
        {
            return false;  // 开始时间不能晚于结束时间
        }

        return true;
    }

    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
}