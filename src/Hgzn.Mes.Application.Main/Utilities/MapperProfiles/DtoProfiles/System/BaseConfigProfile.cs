using AutoMapper;

using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Code;
using Hgzn.Mes.Domain.Entities.System.Config;
using Hgzn.Mes.Domain.ValueObjects;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.System
{
    public class BaseConfigProfile : Profile
    {
        public BaseConfigProfile()
        {
            CreateMap<BaseConfigCreateDto, BaseConfig>();

            CreateMap<BaseConfigReadDto, BaseConfig>();
            CreateMap<BaseConfig, BaseConfigReadDto>();

            CreateMap<BaseConfigReadDto, BaseConfigUpdateDto>();
            CreateMap<BaseConfigUpdateDto, BaseConfigReadDto>();
        }
    }
}
