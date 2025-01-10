using System.ComponentModel.DataAnnotations.Schema;
using Hgzn.Mes.Domain.Entities.Base;

namespace Hgzn.Mes.Domain.Entities.System.Dictionary
{
    [Table("DictionaryType")]
    public class DictionaryType : UniversalEntity, ISoftDelete, IOrder, IState
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
        /// 字典名称
        /// </summary>
        public string DictName { get; set; } = string.Empty;

        /// <summary>
        /// 字典类型
        /// </summary>
        public string DictType { get; set; } = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        public string? Remark { get; set; }

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
        /// 删除时间
        /// </summary>
        public DateTime? DeleteTime { get; set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool SoftDeleted { get; set; }

        /// <summary>
        /// 与 DictionaryEntity 的一对多关系
        /// </summary>
        public ICollection<DictionaryInfo>? DictionaryEntities { get; set; }
    }
}
