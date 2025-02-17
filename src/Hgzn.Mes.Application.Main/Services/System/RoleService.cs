using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Entities.System.Authority;
using Hgzn.Mes.Domain.Entities.System.Code;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Exceptions;
using Hgzn.Mes.Domain.Shared.Extensions;
using Hgzn.Mes.Domain.Utilities;
using Hgzn.Mes.Infrastructure.Utilities;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Services.System
{
    public class RoleService : SugarCrudAppService<
        Role, Guid,
        RoleReadDto, RoleQueryDto,
        RoleCreateDto, RoleUpdateDto>,
        IRoleService
    {
        public async Task<RoleReadDto?> CreateRoleAsync(RoleCreateDto roleDto)
        {
            return await base.CreateAsync(roleDto);
        }

        public async Task<RoleReadDto?> GetRoleAsync(Guid id)
        {
            return await GetAsync(id);
        }
        



        public async Task<IEnumerable<RoleReadDto>> GetRolesAsync()
        {
            var roles = await Queryable
                .Includes(r => r.Menus)
                .ToArrayAsync();
            return Mapper.Map<IEnumerable<RoleReadDto>>(roles);
        }

        public async Task<bool> ModifyRoleMenuAsync(Guid roleId, List<Guid> menuIds)
        {
            var roleMenuns = await menuIds.Select(m => new RoleMenu
            {
                RoleId = roleId,
                MenuId = m
            }).ToListAsync();
            var role = await GetAsync(roleId) ??
                throw new NotFoundException("role is not exist");
            
            var result = await DbContext.Ado.UseTranAsync(async () =>
            {
                // 删除角色菜单
                await DbContext.Deleteable<RoleMenu>().Where(s => s.RoleId == roleId).ExecuteCommandAsync();
    
                // 插入新的角色菜单
                await DbContext.Insertable(roleMenuns).ExecuteCommandAsync(); 
            });
            if (result.IsSuccess)
            {
                return true;
            }
            throw result.ErrorException;
        }

        public IEnumerable<ScopeDefReadDto> GetScopes() =>
            Mapper.Map<IEnumerable<ScopeDefReadDto>>(RequireScopeUtil.Scopes);

        public async Task<PaginatedList<UserReadDto>> GetRoleUsersAsync(Guid roleId, UserQueryDto query)
        {
            var users = await Queryable
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

        public async Task<bool> ModifyRoleUserAsync(Guid roleId, List<Guid> userIds)
        {
            var roleUsers = await userIds.Select(m => new UserRole()
            {
                RoleId = roleId,
                UserId = m
            }).ToListAsync();
            var role = await GetAsync(roleId) ??
                       throw new NotFoundException("role is not exist");
            
            var result = await DbContext.Ado.UseTranAsync(async () =>
            {
                // 删除角色菜单
                await DbContext.Deleteable<UserRole>().Where(s => s.RoleId == roleId).ExecuteCommandAsync();
    
                // 插入新的角色菜单
                await DbContext.Insertable(roleUsers).ExecuteCommandAsync(); 
            });
            if (result.IsSuccess)
            {
                return true;
            }
            throw result.ErrorException;
        }

        public override async Task<IEnumerable<RoleReadDto>> GetListAsync(RoleQueryDto? queryDto = null)
        {
            var roles = await Queryable
                .Includes(r => r.Menus)
                .ToListAsync();
            return Mapper.Map<IEnumerable<RoleReadDto>>(roles);
        }

        public override async Task<PaginatedList<RoleReadDto>> GetPaginatedListAsync(RoleQueryDto queryDto)
        {
            var roles = await Queryable
                .WhereIF(!string.IsNullOrEmpty(queryDto.Name), x => x.Name.Contains(queryDto.Name))
                .WhereIF(!string.IsNullOrEmpty(queryDto.Code), x => x.Code.Contains(queryDto.Code))
                .WhereIF(queryDto.State!=null , x => x.State== queryDto.State)
                .Includes(r => r.Menus)
                .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
            return Mapper.Map<PaginatedList<RoleReadDto>>(roles);
        }

        ///// <summary>
        ///// 修改状态
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="state"></param> 
        ///// <returns></returns>
        //public async Task<RoleReadDto> UpdateStateAsync(Guid id, bool state)
        //{
        //    var oldRoleData = await GetAsync(id);
        //    oldRoleData.State = state;

        //    Role info = Mapper.Map<Role>(oldRoleData);

        //    var data = DbContext.Updateable<Role>(info).ExecuteCommand();
        //    return oldRoleData;
        //}

        public override Task<RoleReadDto?> UpdateAsync(Guid key, RoleUpdateDto dto)
        {
            var saverole= base.UpdateAsync(key, dto);
            if (dto.MenuIds != null && dto.MenuIds.Count() > 0) {
                List<RoleMenu> roleMenus = new List<RoleMenu>();
                var deleteCount = DbContext.Deleteable<RoleMenu>().Where(a => a.RoleId == key).ExecuteCommand();
                foreach (var item in dto.MenuIds)
                {
                    var data = DbContext.Insertable<RoleMenu>(new RoleMenu() { 
                        MenuId = item, RoleId = key })
                        .ExecuteCommand();
                }
            }

            if (dto.UserIds != null && dto.UserIds.Count() > 0)
            {
                List<UserRole> roleMenus = new List<UserRole>();
                var deleteCount = DbContext.Deleteable<UserRole>().Where(a => a.RoleId == key).ExecuteCommand();
                foreach (var item in dto.UserIds)
                {
                    var data = DbContext.Insertable<UserRole>(new UserRole()
                    {
                        UserId = item,
                        RoleId = key
                    })
                    .ExecuteCommand();
                }
            }

            return saverole;
        }
    }
}