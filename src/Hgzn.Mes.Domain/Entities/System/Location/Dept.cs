using Hgzn.Mes.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.System.Location
{
    public class Dept : UniversalEntity
    {
        public Dept()
        {
        }

        public Dept(Guid Id) { this.Id = Id; ParentId = Guid.Empty; }

        public Dept(Guid Id, Guid parentId) { this.Id = Id; ParentId = parentId; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建者
        /// </summary>
        public Guid? CreatorId { get; set; }

        /// <summary>
        /// 最后修改者
        /// </summary>
        public Guid? LastModifierId { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderNum { get; set; } = 0;

        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; } = true;

        /// <summary>
        /// 部门名称 
        ///</summary>
        public string DeptName { get; set; } = string.Empty;
        /// <summary>
        /// 部门编码 
        ///</summary>
        [Description("DeptCode")]
        public string DeptCode { get; set; } = string.Empty;
        /// <summary>
        /// 负责人 
        ///</summary>
        [Description("Leader")]
        public string? Leader { get; set; }
        /// <summary>
        /// 父级id 
        ///</summary>
        [Description("ParentId")]
        public Guid ParentId { get; set; }

        /// <summary>
        /// 描述 
        ///</summary>
        [Description("Remark")]
        public string? Remark { get; set; }
    }
}
