using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Dtos.Equip
{
    public class EquipDataPointReadDto : ReadDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }

        public string EquipName { get; set; }

        public string EquipCode { get; set; }

        public string? Remark { get; set; }

        public Guid? CollectionConfigId { get; set; }

        public string? CollectionAddressStr { get; set; }

        public Guid? CheckId { get; set; }

        public Guid? Trigger { get; set; }

        public bool State { get; set; }
        public Guid? ConnectionId { get; set; }

        public bool ConnectState { get; set; } = false;

        public DataPointStatus CollectStatus { get; set; }
        /// <summary>
        /// 当前的数据值
        /// </summary>
        public object? Data { get; set; }
    }

    public class EquipDataPointCreateDto : CreateDto
    {
        public string Code { get; set; }

        public Guid ConnectionId { get; set; }

        public string? Remark { get; set; }
        public Guid? CollectionConfigId { get; set; }

        public string? CollectionAddressStr { get; set; }

        public Guid? CheckId { get; set; }

        public Guid? Trigger { get; set; }
    }

    public class EquipDataPointUpdateDto : UpdateDto
    {
        public string Code { get; set; }

        public Guid ConnectionId { get; set; }

        public string? Remark { get; set; }
        public Guid? CollectionConfigId { get; set; }

        public string? CollectionAddressStr { get; set; }

        public Guid? CheckId { get; set; }

        public Guid? Trigger { get; set; }
    }

    public class EquipDataPointQueryDto : PaginatedQueryDto
    {
        public string? Code { get; set; }

        public Guid? ConnectionId { get; set; }

        public string? Remark { get; set; }
        public bool? State { get; set; }
    }
}
