using HgznMes.Application.Dtos;

namespace HgznMes.Application.Services.Base
{
    public interface IRoleService : IBaseService
    {
        public Task<RoleReadDto?> GetRoleAsync(Guid id);

        public Task<IEnumerable<RoleReadDto>> GetRolesAsync();

        public Task<RoleReadDto?> CreateRoleAsync(RoleCreateDto roleDto);

        public Task<int> ModifyRoleScopeAsync(Guid roleId, List<string> scopeName);

        public IEnumerable<RoleScopeReadDto> GetScopes();
    }
}