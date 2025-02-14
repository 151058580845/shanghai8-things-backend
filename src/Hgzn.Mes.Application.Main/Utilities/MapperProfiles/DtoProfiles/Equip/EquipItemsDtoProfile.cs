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
    public class EquipItemsDtoProfile : Profile
    {
        public EquipItemsDtoProfile()
        {
            CreateMap<EquipItems, EquipItemsReadDto>()
                .ForMember(dest => dest.EquipMaintenanceType, opt => opt.MapFrom(src => ConvertStringToEnum(src.EquipMaintenanceType)));
            CreateMap<EquipItemsCreateDto, EquipItems>();
            CreateMap<EquipItemsUpdateDto, EquipItems>()
                .ForMember(dest => dest.EquipMaintenanceType, opt => opt.MapFrom(src => src.EquipMaintenanceType.ToString()));
        }

        private EquipMaintenanceType ConvertStringToEnum(string status)
        {
            if (Enum.TryParse(status, true, out EquipMaintenanceType result))
            {
                return result;
            }
            throw new ArgumentException($"Invalid status: {status}");
        }
    }
}
