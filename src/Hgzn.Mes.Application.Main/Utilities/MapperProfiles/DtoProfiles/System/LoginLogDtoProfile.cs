using AutoMapper;

using Hgzn.Mes.Application.Main.Dtos.Audit;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.Audit;
using Hgzn.Mes.Domain.Entities.System.Code;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.System
{
    public class LoginLogDtoProfile : Profile
    {
        public LoginLogDtoProfile()
        {
            CreateMap<LoginLogReadDto, LoginLog>();
            CreateMap<LoginLog, LoginLogReadDto>();

            CreateMap<LoginLogCreateDto, LoginLogDto>();
            CreateMap<LoginLogDto, LoginLogCreateDto>();

            CreateMap<LoginLogCreateDto, LoginLog>();
            CreateMap<LoginLog, LoginLogCreateDto>();

        }
    }
}
