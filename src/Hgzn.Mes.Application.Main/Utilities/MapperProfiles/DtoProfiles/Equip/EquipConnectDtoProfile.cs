using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.Equip
{
    public class EquipConnectDtoProfile : Profile
    {
        public EquipConnectDtoProfile()
        {
            CreateMap<EquipConnect, EquipConnectReadDto>();
            CreateMap<EquipConnectCreateDto, EquipConnect>();
            CreateMap<EquipConnect, EquipConnectQueryDto>()
                .ForMember(dest => dest.EquipCode, src => src.MapFrom(x => x.Code))
                .ForMember(dest => dest.EquipName, src => src.MapFrom(x => x.Name));
            CreateMap<EquipConnectCreateDto, EquipConnect>()
                .ForMember(dest => dest.ForwardEntities, src => src.MapFrom(x => x.ForwardIds.Select(item => new EquipConnectForward { TargetId = item })));
            CreateMap<EquipConnectCreateDto, EquipConnect>()
                .ForMember(dest => dest.ProtocolEnum, src => src.MapFrom(x => string.IsNullOrEmpty(x.ProtocolEnum) ? Protocol.ModbusTcp : ConvertStringToEnum(x.ProtocolEnum)));
            CreateMap<EquipConnectUpdateDto, EquipConnect>()
                .ForMember(dest => dest.ProtocolEnum, src => src.MapFrom(x => string.IsNullOrEmpty(x.ProtocolEnum) ? Protocol.ModbusTcp : ConvertStringToEnum(x.ProtocolEnum)));
            CreateMap<EquipConnectQueryDto, EquipConnect>()
                .ForMember(dest => dest.ProtocolEnum, src => src.MapFrom(x => string.IsNullOrEmpty(x.ProtocolEnum) ? Protocol.ModbusTcp : ConvertStringToEnum(x.ProtocolEnum)));
        }

        private Protocol ConvertStringToEnum(string status)
        {
            if (Enum.TryParse(status, true, out Protocol result))
            {
                return result;
            }
            throw new ArgumentException($"Invalid status: {status}");
        }
    }
}
