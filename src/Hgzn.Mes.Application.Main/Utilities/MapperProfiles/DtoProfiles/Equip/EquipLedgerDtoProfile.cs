using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.Equip;

public class EquipLedgerDtoProfile : Profile
{
    public EquipLedgerDtoProfile()
    {
        // CreateMap<EquipLedgerCreateDto, EquipLedger>()
        //     .ForMember(d => d.DeviceStatus, opt => opt.MapFrom(x => ConvertStringToDeviceStatus(x.DeviceStatus!)));
        // CreateMap<EquipLedgerUpdateDto, EquipLedger>()
        //     .ForMember(d => d.DeviceStatus, opt => opt.MapFrom(x => ConvertStringToDeviceStatus(x.DeviceStatus!)));
        CreateMap<EquipLedger, EquipLedgerReadDto>()
            .ForMember(d => d.DeviceStatus, opt => opt.MapFrom(x => x.DeviceStatus.ToString()))
            .ForMember(d=>d.TypeName,opt=>opt.MapFrom(x=>x.EquipType == null?"":x.EquipType!.TypeName))
            .ForMember(d=>d.RoomName,opt=>opt.MapFrom(x=>x.Room==null?"":x.Room!.Name));

    }

    private DeviceStatus ConvertStringToDeviceStatus(string status)
    {
        if (Enum.TryParse(status, true, out DeviceStatus result))
        {
            return result;
        }
        throw new ArgumentException($"Invalid status: {status}");
    }
}