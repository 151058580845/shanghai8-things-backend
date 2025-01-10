using Hgzn.Mes.Application.BaseS;
using Hgzn.Mes.Application.Dtos;
using Hgzn.Mes.Application.Services;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Shared;

namespace Hgzn.Mes.Application.Main.Services.System.IService
{
    public interface IRoleService : IBaseService,ICrudAppService<Role
        , RoleReadDto, RoleReadDto, Guid, RoleQueryDto, RoleCreateDto,
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