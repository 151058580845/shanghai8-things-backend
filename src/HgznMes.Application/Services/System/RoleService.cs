using HgznMes.Application.Auth;
using HgznMes.Application.Dtos;
using HgznMes.Application.Services.Base;
using HgznMes.Domain.Shared.Exceptions;
using HgznMes.Domain.Utilities;
using Microsoft.EntityFrameworkCore;
using HgznMes.Infrastructure.DbContexts;
using HgznMes.Domain.Shared;
using HgznMes.Domain.Entities.System.Account;
using HgznMes.Domain.Entities.System.Authority;

namespace HgznMes.Application.Services
{
    [ScopeDefinition("manage all role resources", ManagedResource.Role)]
    public class RoleService : BaseService, IRoleService
    {
        public RoleService(
            ApiDbContext apiDbContext)
        {
            _apiDbContext = apiDbContext;
        }

        private readonly ApiDbContext _apiDbContext;

        [ScopeDefinition("create a role", $"{ManagedResource.Role}.{ManagedAction.Add}.New")]
        public async Task<RoleReadDto?> CreateRoleAsync(RoleCreateDto roleDto)
        {
            if (!RequireScopeUtil.Scopes.Any(s => !roleDto.MenuIds.Contains(s.Name)))
            {
                throw new NotAcceptableException("unsupported scope find");
            }
            var entity = Mapper.Map<Role>(roleDto);
            await _apiDbContext.Roles.AddAsync(entity);
            var index = await _apiDbContext.SaveChangesAsync();
            return index == 0 ? null : Mapper.Map<RoleReadDto>(entity);
        }

        [ScopeDefinition("get role info by id", $"{ManagedResource.Role}.{ManagedAction.Get}.Id")]
        public async Task<RoleReadDto?> GetRoleAsync(Guid id)
        {
            var role = await _apiDbContext.Roles
                .Where(r => r.Id == id)
                .Include(r => r.Menus)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            return Mapper.Map<RoleReadDto>(role);
        }

        [ScopeDefinition("get all roles", $"{ManagedResource.Role}.{ManagedAction.Get}.All")]
        public async Task<IEnumerable<RoleReadDto>> GetRolesAsync()
        {
            var roles = await _apiDbContext.Roles
                .Include(r => r.Menus)
                .AsNoTracking()
                .ToArrayAsync();
            return Mapper.Map<IEnumerable<RoleReadDto>>(roles);
        }

        [ScopeDefinition("change role manage scope", $"{ManagedResource.Role}.{ManagedAction.Put}.Scopes")]
        public async Task<int> ModifyRoleMenuAsync(Guid roleId, List<Guid> menuIds)
        {
            var roleMenuns = menuIds.Select(m => new RoleMenu
            {
                RoleId = roleId,
                MenuId = m
            });
            var role = (await _apiDbContext.Roles.FindAsync(roleId)) ??
                throw new NotFoundException("role is not exist");
            await _apiDbContext.Set<RoleMenu>().Where(s => s.RoleId == roleId).ExecuteDeleteAsync();
            await _apiDbContext.Set<RoleMenu>().AddRangeAsync(roleMenuns);
            var result = await _apiDbContext.SaveChangesAsync();
            return result;
        }

        [ScopeDefinition("get all supported scopes", $"{ManagedResource.Role}.{ManagedAction.Get}.Scopes")]
        public IEnumerable<ScopeDefReadDto> GetScopes() =>
            Mapper.Map<IEnumerable<ScopeDefReadDto>>(RequireScopeUtil.Scopes);

        public async Task<PaginatedList<UserReadDto>> GetRoleUsersAsync(Guid roleId, UserQueryDto query)
        {
            var users = await _apiDbContext.Roles
                .Where(r => r.Id == roleId)
                .Include(r => r.Users == null ? null : r.Users
                    .Where(u => query.Filter == null ||
                    (u.Phone != null && u.Phone.Contains(query.Filter)) ||
                    u.Username.Contains(query.Filter) ||
                    (u.Nick != null && u.Nick.Contains(query.Filter)))
                    .Where(u => query == null || u.State == query.State ))
                .SelectMany(r => r.Users ?? Array.Empty<User>())
                .ToArrayAsync();
            return Mapper.Map<PaginatedList<UserReadDto>>(users);
        }
    }
}