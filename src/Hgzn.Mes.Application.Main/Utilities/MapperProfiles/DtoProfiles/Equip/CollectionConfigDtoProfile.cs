using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.Equip
{
    public class CollectionConfigDtoProfile : Profile
    {
        public CollectionConfigDtoProfile()
        {
            CreateMap<CollectionConfigCreateDto, CollectionConfig>();
            CreateMap<CollectionConfig, CollectionConfigReadDto>();
        }
    }
}
