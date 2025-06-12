using System.Collections;
using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Shared;

namespace Hgzn.Mes.Application.Main.Services.System.IService
{
    public interface IRoleService : ICrudAppService<
        Role, Guid,
        RoleReadDto, RoleQueryDto,
        RoleCreateDto, RoleUpdateDto>
    {
        Task<bool> ModifyRoleMenuAsync(Guid roleId, List<Guid> menuIds);

        IEnumerable<ScopeDefReadDto> GetScopes();
        Task<IEnumerable<string>> GetRoleMenuCodeAsync(List<Guid> roleIds);

        Task<PaginatedList<UserReadDto>> GetRoleUsersAsync(Guid roleId, UserQueryDto dto);
        Task<bool> ModifyRoleUserAsync(Guid roleId, List<Guid> userIds);
    }
}