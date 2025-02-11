using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.Equip
{
    public class EquipConnectDtoProfile : Profile
    {
        public EquipConnectDtoProfile()
        {
            CreateMap<EquipConnect, EquipConnectReadDto>()
                .ForMember(dest => dest.EquipName, opts => opts.MapFrom(src => src.EquipLedger!.EquipName))
                .ForMember(dest => dest.EquipCode, opts => opts.MapFrom(src => src.EquipLedger!.EquipCode))
                .ForMember(dest => dest.TypeName, opts => opts.MapFrom(src => src.EquipLedger!.EquipType == null ? null : src.EquipLedger!.EquipType!.TypeName))
                .AfterMap((src, dest) =>
                {
                    // 判断是否为 RFID 设备，并填充状态
                    if (src.ProtocolEnum == ConnType.Socket)
                    {
                        dest.CollectionModel = src.CollectionExtension switch
                        {
                            1 => "绑定Rfid标签",
                            2 => "解绑Rfid标签",
                            _ => "采集数据"
                        };
                    }
                });


            CreateMap<EquipConnect, EquipConnectQueryDto>()
                .ForMember(dest => dest.EquipCode, src => src.MapFrom(x => x.Code))
                .ForMember(dest => dest.EquipName, src => src.MapFrom(x => x.Name));
            CreateMap<EquipConnectCreateDto, EquipConnect>()
                .ForMember(dest => dest.ForwardEntities, src => src.MapFrom(x => x.ForwardIds.Select(item => new EquipConnectForward { TargetId = item })))
                .ForMember(dest => dest.ProtocolEnum, src => src.MapFrom(x => string.IsNullOrEmpty(x.ProtocolEnum) ? ConnType.ModbusTcp : ConvertStringToEnum(x.ProtocolEnum)));
            CreateMap<EquipConnectUpdateDto, EquipConnect>()
                .ForMember(dest => dest.ProtocolEnum, src => src.MapFrom(x => string.IsNullOrEmpty(x.ProtocolEnum) ? ConnType.ModbusTcp : ConvertStringToEnum(x.ProtocolEnum)));
            CreateMap<EquipConnectQueryDto, EquipConnect>()
                .ForMember(dest => dest.ProtocolEnum, src => src.MapFrom(x => string.IsNullOrEmpty(x.ProtocolEnum) ? ConnType.ModbusTcp : ConvertStringToEnum(x.ProtocolEnum)));
        }

        private ConnType ConvertStringToEnum(string status)
        {
            if (Enum.TryParse(status, true, out ConnType result))
            {
                return result;
            }
            throw new ArgumentException($"Invalid status: {status}");
        }
    }
}
