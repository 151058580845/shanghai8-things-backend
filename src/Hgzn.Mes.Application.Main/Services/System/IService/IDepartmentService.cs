using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System;

namespace Hgzn.Mes.Application.Main.Services.System.IService;

public interface IDepartmentService : ICrudAppService<
    Dept, Guid,
    DepartmentReadDto, DepartmentQueryDto,
    DepartmentCreateDto, DepartmentUpdateDto>
{
    Task<List<Guid>> GetChildListAsync(Guid deptId);
}