using HgznMes.Application.Dtos;
using HgznMes.Domain.Shared;

namespace HgznMes.Application.Services.Base
{
    public interface IRoleService : IBaseService
    {
        Task<RoleReadDto?> GetRoleAsync(Guid id);

        Task<IEnumerable<RoleReadDto>> GetRolesAsync();

        Task<RoleReadDto?> CreateRoleAsync(RoleCreateDto roleDto);

        Task<int> ModifyRoleMenuAsync(Guid roleId, List<Guid> menuIds);

        IEnumerable<ScopeDefReadDto> GetScopes();

        Task<PaginatedList<UserReadDto>> GetRoleUsersAsync(Guid roleId, UserQueryDto dto);
    }
}