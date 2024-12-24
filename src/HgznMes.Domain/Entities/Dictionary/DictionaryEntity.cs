using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HgznMes.Domain.Entities.Base;

namespace HgznMes.Domain.Entities.Dictionary
{
    [Table("Dictionary")]
    public class DictionaryEntity :  ISoftDelete
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid Id { get; protected set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Column("OrderNum")]
        public int OrderNum { get; set; } = 0;

        /// <summary>
        /// 状态
        /// </summary>
        [Column("State")]
        public bool State { get; set; } = true;

        /// <summary>
        /// 描述
        /// </summary>
        [Column("Remark")]
        public string? Remark { get; set; }

        /// <summary>
        /// tag类型
        /// </summary>
        [Column("ListClass")]
        public string? ListClass { get; set; }

        /// <summary>
        /// tagClass
        /// </summary>
        [Column("CssClass")]
        public string? CssClass { get; set; }

        /// <summary>
        /// 字典类型
        /// </summary>
        [Column("ParentId")]
        public Guid ParentId { get; set; }

        /// <summary>
        /// 字典标签
        /// </summary>
        [Column("DictLabel")]
        public string? DictLabel { get; set; }

        /// <summary>
        /// 字典值
        /// </summary>
        [Column("DictValue")]
        public string DictValue { get; set; } = string.Empty;

        /// <summary>
        /// 是否为该类型的默认值
        /// </summary>
        [Column("IsDefault")]
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
        [NotMapped]  // Not directly mapped to database, handled through IsDeleted
        public bool SoftDeleted { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        [Column("DeleteTime")]
        public DateTime? DeleteTime { get; set; }
    }
}
