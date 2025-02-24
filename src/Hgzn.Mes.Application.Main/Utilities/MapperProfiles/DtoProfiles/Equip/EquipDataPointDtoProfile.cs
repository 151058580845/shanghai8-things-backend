using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.Equip
{
    public class EquipDataPointDtoProfile : Profile
    {
        public EquipDataPointDtoProfile()
        {
            //CreateMap<EquipDataPoint, EquipDataPointReadDto>()
            //    .ForMember(dest => dest.CollectStatus, opts => opts.MapFrom(x => x.Connection!.ConnectState ? "Pending" : "None"));
        }
    }
}
