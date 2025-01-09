using Hgzn.Mes.Domain.Entities.System.Base;
using Hgzn.Mes.Domain.Entities.System.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.System.Dictionary
{
    public class DictionaryInfo : UniversalEntity, ISoftDelete, IOrder, IState, IAudited
    {
        /// <summary>
        /// 排序
        /// </summary>
        public int OrderNum { get; set; } = 0;

        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; } = true;

        /// <summary>
        /// 描述
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// tag类型
        /// </summary>
        public string? ListClass { get; set; }

        /// <summary>
        /// tagClass
        /// </summary>
        public string? CssClass { get; set; }

        /// <summary>
        /// 字典类型
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// 字典标签
        /// </summary>
        public string? DictLabel { get; set; }

        /// <summary>
        /// 字典值
        /// </summary>
        public string DictValue { get; set; } = string.Empty;

        /// <summary>
        /// 是否为该类型的默认值
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        public Guid? CreatorId { get; set; }

        /// <summary>
        /// 最后修改者ID
        /// </summary>
        public Guid? LastModifierId { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 软删除标志
        /// </summary>
        public bool SoftDeleted { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeleteTime { get; set; }
    }
}
