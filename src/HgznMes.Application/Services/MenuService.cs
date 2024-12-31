using Microsoft.EntityFrameworkCore;
using HgznMes.Application.Dtos;
using HgznMes.Application.Services.Base;
using HgznMes.Domain.Shared.Exceptions;
using HgznMes.Infrastructure.DbContexts;
using HgznMes.Infrastructure.Utilities;
using HgznMes.Domain.Shared;
using HgznMes.Domain.Entities.Authority;
using System.Security.Claims;

namespace HgznMes.Application.Services
{
    public class MenuService : BaseService, IMenuService
    {
        public MenuService(
            ApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private readonly ApiDbContext _dbContext;

        public async Task<PaginatedList<MenuReadDto>> QueryMenusAsync(QueryMenuDto query)
        {
            var entities = await _dbContext.Menus
                .Where(m => query.State == null || query.State == m.State)
                .Where(m => query.Filter == null || m.Code.Contains(query.Filter) || m.Name.Contains(query.Filter))
                .OrderByDescending(m => m.OrderNum)
                .ToPaginatedListAsync(query.PageIndex, query.PageSize);
            return Mapper.Map<PaginatedList<MenuReadDto>>(entities);
        }

        public async Task<IEnumerable<MenuReadDto>> GetCurrentUserMenusAsTreeAsync(IEnumerable<Claim> claims)
        {
            var roleId = Guid.Parse(claims.FirstOrDefault(c => c.Type == CustomClaimsType.RoleId)!.Value);

            var roles = await _dbContext.Roles
                .Where(r => r.Id == roleId)
                .Include(r => r.Menus)
                .ToArrayAsync();
            var targets = roles.Where(r => r.Menus != null).SelectMany(r => r.Menus!);
            var menus = Mapper.Map<IEnumerable<MenuReadDto>>(targets);
            var root = AsTree(menus.Single(m => m.ParentId == null));
            return root.Children!;

            MenuReadDto AsTree(MenuReadDto parent)
            {
                parent.Children = menus
                    .Where(m => m.ParentId == parent.Id)
                    .OrderBy(m => m.Order)
                    .ThenBy(m => m.Level)
                    .Select(m => AsTree(m));
                return parent;
            }
        }

        public async Task<IEnumerable<MenuReadDto>> GetRootMenusAsTreeAsync()
        {
            var entities = await _dbContext.Menus.ToArrayAsync();
            var menus = Mapper.Map<IEnumerable<MenuReadDto>>(entities);
            var root = AsTree(menus.Single(m => m.ParentId == null));
            return root.Children!;

            MenuReadDto AsTree(MenuReadDto parent)
            {
                parent.Children = menus
                    .Where(m => m.ParentId == parent.Id)
                    .OrderBy(m => m.Order)
                    .ThenBy(m => m.Level)
                    .Select(m => AsTree(m));
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
            var menu = await _dbContext.Menus
                .FindAsync(id) ?? throw new NotFoundException("id not exist");

            if (menu.ParentId is null) throw new NotAcceptableException("root menu con't delete");

            if (force)
            {
                count = await _dbContext.Menus.Where(m => m.Id == id).ExecuteDeleteAsync();
            }
            else if(await _dbContext.Menus.AnyAsync(m => m.ParentId == id))
            {
                throw new NotAcceptableException("child menu exist");
            }
            return count;
        }

        public async Task<MenuReadDto> CreateMenuAsync(MenuCreateDto dto)
        {
            var entity = Mapper.Map<Menu>(dto);
            await _dbContext.Menus.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return Mapper.Map<MenuReadDto>(dto);
        }

        public async Task<MenuReadDto> UpdateMenuAsync(Guid id, MenuUpdateDto dto)
        { 
            var target = await _dbContext.Menus.FindAsync(id);
            if(target is null)
            {
                throw new NotFoundException("menu not exsit");
            }
            Mapper.Map(dto, target);
            _dbContext.Menus.Update(target);
            await _dbContext.SaveChangesAsync();
            return Mapper.Map<MenuReadDto>(dto);
        }
    }
}
