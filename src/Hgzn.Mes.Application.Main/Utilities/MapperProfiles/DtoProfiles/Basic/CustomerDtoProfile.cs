using AutoMapper;

using Hgzn.Mes.Application.Main.Dtos.Basic;
using Hgzn.Mes.Domain.Entities.Basic;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.Basic
{
    public  class CustomerDtoProfile: Profile
    {
        public CustomerDtoProfile()
        {
            CreateMap<CustomerCreateDto, Customer>();

            CreateMap<CustomerReadDto, Customer>();
            CreateMap<Customer, CustomerReadDto>();

            CreateMap<CustomerReadDto, CustomerUpdateDto>();
            CreateMap<CustomerUpdateDto, CustomerReadDto>();
            CreateMap<Customer, CustomerReadDto>();
        }
    }
}
