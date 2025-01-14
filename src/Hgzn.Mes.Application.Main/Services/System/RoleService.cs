using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Entities.System.Authority;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Exceptions;
using Hgzn.Mes.Domain.Utilities;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Services.System
{
    [ScopeDefinition("manage all role resources", ManagedResource.Role)]
    public class RoleService : CrudAppServiceSugar<Role, Guid, RoleQueryDto,
        RoleReadDto, RoleCreateDto, RoleUpdateDto>,
        IRoleService
    {
        [ScopeDefinition("create a role", $"{ManagedResource.Role}.{ManagedAction.Add}.New")]
        public async Task<RoleReadDto?> CreateRoleAsync(RoleCreateDto roleDto)
        {
            return await base.CreateAsync(roleDto);
        }

        [ScopeDefinition("get role info by id", $"{ManagedResource.Role}.{ManagedAction.Get}.Id")]
        public async Task<RoleReadDto?> GetRoleAsync(Guid id)
        {
            return await GetAsync(id);
        }

        [ScopeDefinition("get all roles", $"{ManagedResource.Role}.{ManagedAction.Get}.All")]
        public async Task<IEnumerable<RoleReadDto>> GetRolesAsync()
        {
            var roles = await Queryable()
                .Includes(r => r.Menus)
                .ToArrayAsync();
            return Mapper.Map<IEnumerable<RoleReadDto>>(roles);
        }

        [ScopeDefinition("change role manage scope", $"{ManagedResource.Role}.{ManagedAction.Put}.Scopes")]
        public async Task<bool> ModifyRoleMenuAsync(Guid roleId, List<Guid> menuIds)
        {
            var roleMenuns = menuIds.Select(m => new RoleMenu
            {
                RoleId = roleId,
                MenuId = m
            });
            var role = await GetAsync(roleId) ??
                       throw new NotFoundException("role is not exist");
            var result = await DbContext.Ado.UseTranAsync(async () =>
            {
                // 获取现有的菜单 ID
                await DbContext.Deleteable<RoleMenu>().Where(s => s.RoleId == roleId).ExecuteCommandAsync();
                await DbContext.Insertable<RoleMenu>(roleMenuns).ExecuteCommandAsync();
            });
            if (result.IsSuccess)
            {
                return true;
            }
            throw result.ErrorException;
        }

        [ScopeDefinition("get all supported scopes", $"{ManagedResource.Role}.{ManagedAction.Get}.Scopes")]
        public IEnumerable<ScopeDefReadDto> GetScopes() =>
            Mapper.Map<IEnumerable<ScopeDefReadDto>>(RequireScopeUtil.Scopes);

        public async Task<PaginatedList<UserReadDto>> GetRoleUsersAsync(Guid roleId, UserQueryDto query)
        {
            var users = await Queryable()
                .Where(r => r.Id == roleId)
                .Includes(r => r.Users == null ? null : r.Users
                    .Where(u => query.Filter == null ||
                    (u.Phone != null && u.Phone.Contains(query.Filter)) ||
                    u.Username.Contains(query.Filter) ||
                    (u.Nick != null && u.Nick.Contains(query.Filter)))
                    .Where(u => query == null || u.State == query.State))
                .Select(r => r.Users ?? Array.Empty<User>())
                .ToArrayAsync();
            return Mapper.Map<PaginatedList<UserReadDto>>(users);
        }

        public override Task<PaginatedList<RoleReadDto>> GetListAsync(RoleQueryDto queryDto)
        {
            throw new NotImplementedException();
        }
    }
}