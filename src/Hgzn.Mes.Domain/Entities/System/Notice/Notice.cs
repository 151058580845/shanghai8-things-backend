using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Shared.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hgzn.Mes.Domain.Entities.System.Notice
{
    public class Notice : UniversalEntity, ISoftDelete, IAudited, IState, IOrder
    {
        [Description("公告标题")]
        public string Title { get; set; } = null!;

        [Description("类型")]
        public NoticeShow NoticeShowType { get; set; }

        [Description("公告类型（例如：普通公告、物料告警、设备告警）")]
        public NoticeType NoticeType { get; set; }

        [Description("发送消息目标源")]
        public Guid? ObjectId { get; set; }

        [Description("内容")]
        public string Content { get; set; } = null!;

        [Description("实际发布时间")]
        public DateTime? PublishTime { get; set; }

        [Description("定时发送时间（如果为空则立即发送）")]
        public DateTime? SendTime { get; set; }

        [Description("发送时机设置")]
        public NoticeTimeType NoticeTimeType { get; set; } = NoticeTimeType.Now;

        [Description("优先级")]
        public Priority Priority { get; set; } = Priority.Normal;

        [Description("公告状态（草稿、审核中、已发布、已撤销）")]
        public NoticeStatus Status { get; set; }

        [Description("接收范围（部门、角色、特定用户列表）")]
        public NoticeTargetType Target { get; set; }

        [NotMapped]
        [Description("通知目标列表")]
        public List<NoticeTarget>? NoticeTargets { get; set; }

        [Description("创建时间")]
        public DateTime CreationTime { get; set; }

        [Description("创建人ID")]
        public Guid? CreatorId { get; set; }

        [Description("最后修改人ID")]
        public Guid? LastModifierId { get; set; }

        [Description("最后修改时间")]
        public DateTime? LastModificationTime { get; set; }

        [Description("排序")]
        public int OrderNum { get; set; }

        [Description("状态")]
        public bool State { get; set; }

        #region delete filter

        [Description("软删除标志")]
        public bool SoftDeleted { get; set; } = false;

        [Description("删除时间")]
        public DateTime? DeleteTime { get; set; } = null;

        #endregion delete filter
    }
}