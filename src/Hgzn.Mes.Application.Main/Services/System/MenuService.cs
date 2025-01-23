using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Entities.System.Authority;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Exceptions;
using SqlSugar;
using System.Security.Claims;

namespace Hgzn.Mes.Application.Main.Services.System
{
    public class MenuService : SugarCrudAppService<
        Menu, Guid,
        MenuReadDto, MenuQueryDto,
        MenuCreateDto,MenuUpdateDto>,
        IMenuService
    {
        public override async Task<PaginatedList<MenuReadDto>> GetPaginatedListAsync(MenuQueryDto query)
        {
            var entities = await Queryable
                .Where(m => query.State == null || query.State == m.State)
                .Where(m => query.Filter == null || m.Code.Contains(query.Filter) || m.Name.Contains(query.Filter))
                .OrderByDescending(m => m.OrderNum)
                .ToPageListAsync(query.PageIndex, query.PageSize);
            return Mapper.Map<PaginatedList<MenuReadDto>>(entities);
        }

        public async Task<IEnumerable<MenuReadDto>> GetCurrentUserMenusAsTreeAsync(IEnumerable<Claim> claims)
        {
            var roleId = Guid.Parse(claims.FirstOrDefault(c =>
                c.Type == CustomClaimsType.RoleId)!.Value);

            IEnumerable<Menu> entities;
            if(roleId == Role.DevRole.Id)
            {
                entities = await DbContext.Queryable<Menu>().ToArrayAsync();
            }
            else
            {
                var roles = await DbContext.Queryable<Role>()
                    .Where(r => r.Id == roleId)
                    .Includes(r => r.Menus)
                    .ToArrayAsync();
                if (roles.Length == 0) throw new NotFoundException("role not found");
                entities = roles.Where(r => r.Menus != null).SelectMany(r => r.Menus!);
                if (entities.Count() == 0) return Enumerable.Empty<MenuReadDto>();
            }

            var menus = Mapper.Map<IEnumerable<MenuReadDto>>(entities);
            var menuReadDtos = menus as MenuReadDto[] ?? menus.ToArray();
            var root = AsTree(menuReadDtos.Single(m => m.ParentId == null));
            return root.Children!;

            MenuReadDto AsTree(MenuReadDto parent)
            {
                parent.Children = menuReadDtos
                    .Where(m => m.ParentId == parent.Id)
                    .OrderBy(m => m.Order)
                    .ThenBy(m => m.Level)
                    .Select(AsTree);
                return parent;
            }
        }

        public async Task<IEnumerable<MenuReadDto>> GetRootMenusAsTreeAsync()
        {
            var entities = await Queryable.ToArrayAsync();
            var menus = Mapper.Map<IEnumerable<MenuReadDto>>(entities);
            var menuReadDtos = menus as MenuReadDto[] ?? menus.ToArray();
            var root = AsTree(menuReadDtos.Single(m => m.ParentId == null));
            return root.Children!;

            MenuReadDto AsTree(MenuReadDto parent)
            {
                parent.Children = menuReadDtos
                    .Where(m => m.ParentId == parent.Id)
                    .OrderBy(m => m.Order)
                    .ThenBy(m => m.Level)
                    .Select(AsTree);
                return parent;
            }
        }

        public Task<int> SetMenuRouteAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<int> DeleteMenuAsync(Guid id, bool force)
        {
            var count = 0;
            var menu = await GetAsync(id) ?? throw new NotFoundException("id not exist");

            if (menu.ParentId is null) throw new NotAcceptableException("root menu con't delete");

            if (force)
            {
                count = await DeleteAsync(id);
            }
            else if (await Queryable.AnyAsync(m => m.ParentId == id))
            {
                throw new NotAcceptableException("child menu exist");
            }

            return count;
        }

        public override Task<IEnumerable<MenuReadDto>> GetListAsync(MenuQueryDto? queryDto)
        {
            throw new NotImplementedException();
        }
    }
}