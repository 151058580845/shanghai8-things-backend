using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using static Mysqlx.Notice.Warning.Types;

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
            .ForMember(d => d.TypeName, opt => opt.MapFrom(x => x.EquipType == null ? "" : x.EquipType!.TypeName))
            .ForMember(d => d.RoomName, opt => opt.MapFrom(x => x.Room == null ? "" : x.Room!.Name))
            .ForMember(d => d.DeviceLevel, opt => opt.MapFrom(x => x.EquipLevel.ToString()));
        CreateMap<EquipLedgerCreateDto, EquipLedger>()
            .ForMember(d => d.EquipLevel, opt => opt.MapFrom(x => ConvertStringToDeviceLevel(x.DeviceLevel!)))
            .ForMember(d => d.DeviceStatus, opt => opt.MapFrom(x => ConvertStringToDeviceStatus(x.DeviceStatus!)));
        CreateMap<EquipLedgerUpdateDto, EquipLedger>()
            .ForMember(d => d.EquipLevel, opt => opt.MapFrom(x => ConvertStringToDeviceLevel(x.DeviceLevel!)))
            .ForMember(d => d.DeviceStatus, opt => opt.MapFrom(x => ConvertStringToDeviceStatus(x.DeviceStatus!)));

        CreateMap<LocationLabel, LocationLabelReadDto>();
        CreateMap<LocationLabelUpdateDto, LocationLabel>();
    }

    private EquipLevelEnum ConvertStringToDeviceLevel(string level)
    {
        if (Enum.TryParse(level, true, out EquipLevelEnum result))
        {
            return result;
        }
        if (level == "关键设备")
            return EquipLevelEnum.Important;
        else if (level == "一般设备")
            return EquipLevelEnum.General;
        else if (level == "普通设备")
            return EquipLevelEnum.Basic;
        throw new ArgumentException($"Invalid status: {level}");
    }

    private DeviceStatus ConvertStringToDeviceStatus(string status)
    {
        if (Enum.TryParse(status, true, out DeviceStatus result))
        {
            return result;
        }
        if (status == "正常")
            return DeviceStatus.Normal;
        else if (status == "丢失")
            return DeviceStatus.Lost;
        throw new ArgumentException($"Invalid status: {status}");
    }
}