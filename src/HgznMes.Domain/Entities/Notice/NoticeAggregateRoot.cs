using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HgznMes.Domain.Entities.Base;
using HgznMes.Domain.Shared.Enum;

namespace HgznMes.Domain.Entities.Notice
{
    [Table("Notice")]
    public class NoticeAggregateRoot : IAggregateRoot, ISoftDelete
    {
        [Key] public Guid Id { get; protected set; }

        /// <summary>
        /// 公告标题
        /// </summary>
        [Column("Title", TypeName = "nvarchar(255)")]
        public string Title { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [Column("NoticeShowType")]
        public NoticeShowEnum NoticeShowType { get; set; }

        /// <summary>
        /// 公告类型（例如：普通公告、物料告警、设备告警）
        /// </summary>
        [Column("NoticeType")]
        public NoticeTypeEnum NoticeType { get; set; }

        /// <summary>
        /// 发送消息目标源
        /// </summary>
        [Column("ObjectId")]
        public Guid? ObjectId { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Column("Content", TypeName = "nvarchar(max)")]
        public string Content { get; set; }

        /// <summary>
        /// 实际发布时间
        /// </summary>
        [Column("PublishTime")]
        public DateTime? PublishTime { get; set; }

        /// <summary>
        /// 定时发送时间（如果为空则立即发送）
        /// </summary>
        [Column("SendTime")]
        public DateTime? SendTime { get; set; }

        /// <summary>
        /// 发送时机设置
        /// </summary>
        [Column("NoticeTimeType")]
        public NoticeTimeTypeEnum NoticeTimeType { get; set; } = NoticeTimeTypeEnum.Now;

        /// <summary>
        /// 优先级
        /// </summary>
        [Column("Priority")]
        public PriorityEnum Priority { get; set; } = PriorityEnum.Normal;

        /// <summary>
        /// 公告状态（草稿、审核中、已发布、已撤销）
        /// </summary>
        [Column("Status")]
        public NoticeStatusEnum Status { get; set; }

        /// <summary>
        /// 接收范围（部门、角色、特定用户列表）
        /// </summary>
        [Column("Target")]
        public NoticeTargetEnum Target { get; set; }

        /// <summary>
        /// 忽略列，避免映射到数据库
        /// </summary>
        [NotMapped]
        public List<Guid> TargetId { get; set; }

        /// <summary>
        /// 未读消息列表
        /// </summary>
        [NotMapped]
        public NoticeTargetsEntity TargetsEntity { get; set; }

        /// <summary>
        /// 软删除标识
        /// </summary>
        [Column("IsDeleted")]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("CreationTime")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        [Column("CreatorId")]
        public Guid? CreatorId { get; set; }

        /// <summary>
        /// 最后修改人ID
        /// </summary>
        [Column("LastModifierId")]
        public Guid? LastModifierId { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [Column("LastModificationTime")]
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Column("OrderNum")]
        public int OrderNum { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Column("State")]
        public bool State { get; set; }


        public bool SoftDeleted { get; set; }
        public DateTime? DeleteTime { get; set; }
    }
}