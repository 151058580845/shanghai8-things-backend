using AutoMapper;

using Hgzn.Mes.Application.Main.Dtos.Basic;
using Hgzn.Mes.Domain.Entities.Basic;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.Basic
{
    public class SupplierDtoProfile : Profile
    {
        public SupplierDtoProfile()
        {
            CreateMap<SupplierCreateDto, Supplier>();

            CreateMap<SupplierReadDto, Supplier>();
            CreateMap<Supplier, SupplierReadDto>();

            CreateMap<SupplierReadDto, SupplierUpdateDto>();
            CreateMap<SupplierUpdateDto, SupplierReadDto>();
            CreateMap<Supplier, SupplierReadDto>();
        }
    }
}
