using System.ComponentModel.DataAnnotations.Schema;
using Hgzn.Mes.Domain.Entities.System.Base;
using Hgzn.Mes.Domain.Entities.System.Base.Audited;
using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Domain.Entities.System.Notice
{
    [Table("Notice")]
    public class Notice : UniversalEntity, ISoftDelete, IAudited, IState, IOrder
    {

        /// <summary>
        /// 公告标题
        /// </summary>
        public string Title { get; set; } = null!;

        /// <summary>
        /// 类型
        /// </summary>
        public NoticeShow NoticeShowType { get; set; }

        /// <summary>
        /// 公告类型（例如：普通公告、物料告警、设备告警）
        /// </summary>
        public NoticeType NoticeType { get; set; }

        /// <summary>
        /// 发送消息目标源
        /// </summary>
        public Guid? ObjectId { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Column(TypeName = "varchar(max)")]
        public string Content { get; set; } = null!;

        /// <summary>
        /// 实际发布时间
        /// </summary>
        public DateTime? PublishTime { get; set; }

        /// <summary>
        /// 定时发送时间（如果为空则立即发送）
        /// </summary>
        public DateTime? SendTime { get; set; }

        /// <summary>
        /// 发送时机设置
        /// </summary>
        public NoticeTimeType NoticeTimeType { get; set; } = NoticeTimeType.Now;

        /// <summary>
        /// 优先级
        /// </summary>
        public Priority Priority { get; set; } = Priority.Normal;

        /// <summary>
        /// 公告状态（草稿、审核中、已发布、已撤销）
        /// </summary>
        public NoticeStatus Status { get; set; }

        /// <summary>
        /// 接收范围（部门、角色、特定用户列表）
        /// </summary>
        public NoticeTargetType Target { get; set; }

        /// <summary>
        /// 忽略列，避免映射到数据库
        /// </summary>
        [NotMapped]
        public List<Guid>? TargetId { get; set; }

        /// <summary>
        /// 未读消息列表
        /// </summary>
        [NotMapped]
        public NoticeTarget? TargetsEntity { get; set; }

        /// <summary>
        /// 软删除标识
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public Guid? CreatorId { get; set; }

        /// <summary>
        /// 最后修改人ID
        /// </summary>
        public Guid? LastModifierId { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderNum { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }


        #region delete filter

        public bool SoftDeleted { get; set; } = false;
        public DateTime? DeleteTime { get; set; } = null;

        #endregion delete filter
    }
}