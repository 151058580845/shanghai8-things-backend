using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.System;

public class DeptDtoProfile:Profile
{
    public DeptDtoProfile()
    {
        CreateMap<Dept, DepartmentReadDto>();
    }
}