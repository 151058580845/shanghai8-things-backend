using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.Equip
{
    public class AssetDataDtoProfile : Profile
    {
        public AssetDataDtoProfile()
        {
            CreateMap<AssetData, AssetDataReadDto>();
            CreateMap<AssetDataProjectItem, ProjectItemDto>().ReverseMap();
            CreateMap<AssetDataCreateDto, AssetData>()
                .ForMember(dest => dest.Projects, opt => opt.Ignore()); // 在服务层手动处理
            CreateMap<AssetDataUpdateDto, AssetData>()
                .ForMember(dest => dest.Projects, opt => opt.Ignore()); // 在服务层手动处理
        }
    }
}
