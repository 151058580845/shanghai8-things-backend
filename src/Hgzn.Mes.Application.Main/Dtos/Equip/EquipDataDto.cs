using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Entities.System.Equip.EquipData;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Dtos.Equip
{
    public class EquipDataReadDto : ReadDto
    {
        public DateTime CreationTime { get; set; }

        public Guid TypeId { get; set; }

        [Description("采集数据")]
        public ReceiveData? ReceiveData { get; set; }

        [Description("最大值")]
        public double? MaxValue { get; set; }

        [Description("最小值")]
        public double? MinValue { get; set; }

        [Description("备注")]
        public string? Remark { get; set; }

        public bool IsDeleted { get; set; } = false;
    }

    public class EquipDataCreateDto : CreateDto
    {
        public DateTime CreationTime { get; set; }

        public Guid TypeId { get; set; }

        [Description("采集数据")]
        public ReceiveData? ReceiveData { get; set; }

        [Description("最大值")]
        public double? MaxValue { get; set; }

        [Description("最小值")]
        public double? MinValue { get; set; }

        [Description("备注")]
        public string? Remark { get; set; }

        public bool IsDeleted { get; set; } = false;
    }

    public class EquipDataUpdateDto : UpdateDto
    {
        public DateTime CreationTime { get; set; }

        public Guid TypeId { get; set; }

        [Description("采集数据")]
        public ReceiveData? ReceiveData { get; set; }

        [Description("最大值")]
        public double? MaxValue { get; set; }

        [Description("最小值")]
        public double? MinValue { get; set; }

        [Description("备注")]
        public string? Remark { get; set; }

        public bool IsDeleted { get; set; } = false;
    }

    public class EquipDataQueryDto : PaginatedQueryDto
    {
        public Guid? TypeId { get; set; }
    }

}
