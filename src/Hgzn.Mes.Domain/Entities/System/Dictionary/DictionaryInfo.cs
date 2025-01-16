using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using System.ComponentModel;

namespace Hgzn.Mes.Domain.Entities.System.Dictionary
{
    public class DictionaryInfo : UniversalEntity, ISoftDelete, IOrder, IState, IAudited
    {
        [Description("排序")]
        public int OrderNum { get; set; } = 0;

        [Description("状态")]
        public bool State { get; set; } = true;

        [Description("描述")]
        public string? Remark { get; set; }

        [Description("tag类型")]
        public string? ListClass { get; set; }

        [Description("tagClass")]
        public string? CssClass { get; set; }

        [Description("字典类型")]
        public Guid ParentId { get; set; }

        [Description("字典标签")]
        public string DictLabel { get; set; } = null!;

        [Description("字典值")]
        public string DictValue { get; set; } = string.Empty;

        [Description("是否为该类型的默认值")]
        public bool IsDefault { get; set; }

        [Description("创建时间")]
        public DateTime CreationTime { get; set; }

        [Description("创建者ID")]
        public Guid? CreatorId { get; set; }

        [Description("最后修改者ID")]
        public Guid? LastModifierId { get; set; }

        [Description("最后修改时间")]
        public DateTime? LastModificationTime { get; set; }

        [Description("软删除标志")]
        public bool SoftDeleted { get; set; }

        [Description("删除时间")]
        public DateTime? DeleteTime { get; set; }
    }
}