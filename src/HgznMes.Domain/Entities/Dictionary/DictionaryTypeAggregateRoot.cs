using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using HgznMes.Domain.Entities.Base;
using HgznMes.Domain.Entities.Dictionary;

namespace MES.SystemManagement.Domain.Entities
{
    [Table("DictionaryType")]
    public class DictionaryTypeAggregateRoot : IAggregateRoot, ISoftDelete
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [Column("Id")] 
        public Guid Id { get; protected set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        [Column("IsDeleted")]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Column("OrderNum")]
        public int OrderNum { get; set; } = 0;

        /// <summary>
        /// 状态
        /// </summary>
        [Column("State")]
        public bool? State { get; set; } = true;

        /// <summary>
        /// 字典名称
        /// </summary>
        [Column("DictName")]
        public string DictName { get; set; } = string.Empty;

        /// <summary>
        /// 字典类型
        /// </summary>
        [Column("DictType")]
        public string DictType { get; set; } = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        [Column("Remark")]
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
        /// 软删除标志
        /// </summary>
        [NotMapped] // EF 不会映射此字段到数据库
        public bool SoftDeleted { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        [Column("DeleteTime")]
        public DateTime? DeleteTime { get; set; }

        /// <summary>
        /// 与 DictionaryEntity 的一对多关系
        /// </summary>
        public List<DictionaryEntity> DictionaryEntities { get; set; }
    }
}
