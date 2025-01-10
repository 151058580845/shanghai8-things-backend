using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Shared;

namespace Hgzn.Mes.Application.Main.Services.System.IService
{
    public interface IRoleService : IBaseService,ICrudAppService<Role, Guid,
        RoleReadDto, RoleCreateDto,
        RoleUpdateDto>
    {
        Task<RoleReadDto?> GetRoleAsync(Guid id);

        Task<IEnumerable<RoleReadDto>> GetRolesAsync();

        Task<RoleReadDto?> CreateRoleAsync(RoleCreateDto roleDto);

        Task<bool> ModifyRoleMenuAsync(Guid roleId, List<Guid> menuIds);

        IEnumerable<ScopeDefReadDto> GetScopes();

        Task<PaginatedList<UserReadDto>> GetRoleUsersAsync(Guid roleId, UserQueryDto dto);
    }
}