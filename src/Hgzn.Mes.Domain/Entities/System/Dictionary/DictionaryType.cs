using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Hgzn.Mes.Domain.Entities.Base;

namespace Hgzn.Mes.Domain.Entities.System.Dictionary
{
    public class DictionaryType : UniversalEntity, ISoftDelete, IOrder, IState
    {
        [Description("排序")]
        public int OrderNum { get; set; } = 0;

        [Description("状态")]
        public bool State { get; set; } = true;

        [Description("字典名称")]
        public string DictName { get; set; } = string.Empty;

        [Description("字典类型")]
        public string DictType { get; set; } = string.Empty;

        [Description("描述")]
        public string? Remark { get; set; }

        [Description("创建时间")]
        public DateTime CreationTime { get; set; }

        [Description("创建者ID")]
        public Guid? CreatorId { get; set; }

        [Description("最后修改者ID")]
        public Guid? LastModifierId { get; set; }

        [Description("最后修改时间")]
        public DateTime? LastModificationTime { get; set; }

        [Description("删除时间")]
        public DateTime? DeleteTime { get; set; }

        [Description("逻辑删除")]
        public bool SoftDeleted { get; set; }

        [Description("与 DictionaryEntity 的一对多关系")]
        [NotMapped]
        public ICollection<DictionaryInfo>? DictionaryEntities { get; set; }
    }
}
