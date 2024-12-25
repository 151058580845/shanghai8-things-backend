using HgznMes.Application.Dtos;
using HgznMes.Domain.Shared;

namespace HgznMes.Application.Services.Base
{
    public interface IRoleService : IBaseService
    {
        public Task<RoleReadDto?> GetRoleAsync(Guid id);

        public Task<IEnumerable<RoleReadDto>> GetRolesAsync();

        public Task<RoleReadDto?> CreateRoleAsync(RoleCreateDto roleDto);

        public Task<int> ModifyRoleMenuAsync(Guid roleId, List<Guid> menuIds);

        public IEnumerable<ScopeDefReadDto> GetScopes();

        public Task<PaginatedList<UserReadDto>> GetRoleUsersAsync(Guid roleId, UserQueryDto dto);
    }
}