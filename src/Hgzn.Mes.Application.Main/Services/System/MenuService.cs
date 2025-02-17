using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Entities.System.Authority;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Exceptions;
using System.Security.Claims;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.Shared.Extensions;
using Hgzn.Mes.Infrastructure.Utilities.CurrentUser;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Services.System
{
    public class MenuService : SugarCrudAppService<
            Menu, Guid,
            MenuReadDto, MenuQueryDto,
            MenuCreateDto, MenuUpdateDto>,
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

        public async Task<IEnumerable<MenuReaderRouterDto>> GetCurrentUserMenusAsTreeAsync(IEnumerable<Claim> claims)
        {
            var roleId = Guid.Parse(claims.FirstOrDefault(c =>
                c.Type == ClaimType.RoleId)!.Value);
            IEnumerable<Menu> entities;
            if (roleId == Role.DevRole.Id)
            {
                entities = await DbContext.Queryable<Menu>()
                    .Where(t=>t.Type != MenuType.Component).ToArrayAsync();
            }
            else
            {
                var roles = await DbContext.Queryable<Role>()
                    .Where(r => r.Id == roleId)
                    .Includes(r => r.Menus == null ? null : r.Menus.Where(t=>t.Type != MenuType.Component) )
                    .ToArrayAsync();
                if (roles.Length == 0) throw new NotFoundException("role not found");
                entities = roles.Where(r => r.Menus != null).SelectMany(r => r.Menus!);
                if (!entities.Any()) return [];
            }
            var allRoutes = await entities.Select(t => new MenuReaderRouterDto()
            {
                Path = t.Route == null ? "" : t.Route.StartsWith('/') ? t.Route : '/' + t.Route,
                // Name = t.IsLink ? "Link" : t.Name,
                Name = t.IsLink ? t.Route : t.Name,
                Id = t.Id,
                component = t.Component,
                Meta = new MetaDto()
                {
                    showLink = t.Visible,
                    FrameSrc = t.IsLink ? t.Route : null,
                    Auths = [],
                    Icon = t.IconUrl ?? "",
                    Title = t.Name
                },
                Children = null,
                ParentId = t.ParentId
            }).ToListAsync();

            var root = AsTree(allRoutes.Single(m => m.ParentId == null));
            return root.Children!;

            MenuReaderRouterDto AsTree(MenuReaderRouterDto parent)
            {
                parent.Children = allRoutes
                    .Where(m => m.ParentId == parent.Id)
                    .Select(AsTree).ToList();
                if (parent.Children.Count == 0)
                {
                    parent.Children = null;
                }

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
                    .OrderBy(m => m.OrderNum)
                    .Select(AsTree);
                if (parent.Children.Count() == 0)
                {
                    parent.Children = null;
                }

                return parent;
            }
        }

        public Task<int> SetMenuRouteAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<MenuReadDto>> GetListByRoleIdAsync(Guid id)
        {
            var list = await Queryable.Where(t=> SqlFunc.Subqueryable<RoleMenu>().Where(m=>m.RoleId == id && m.MenuId == t.Id).Any()).ToListAsync();
            return Mapper.Map<IEnumerable<MenuReadDto>>(list);
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

        public override async Task<IEnumerable<MenuReadDto>> GetListAsync(MenuQueryDto? queryDto)
        {
            var entities = await Queryable.Where(t=>t.Name != "Root").ToListAsync();
            return Mapper.Map<IEnumerable<MenuReadDto>>(entities);
        }
    }
}