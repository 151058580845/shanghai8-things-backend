using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.Equip
{
    public class EquipItemsEnumDtoProfile : Profile
    {
        public EquipItemsEnumDtoProfile()
        {
            CreateMap<EquipItems, EquipItemsEnumReadDto>();
        }
    }
}
