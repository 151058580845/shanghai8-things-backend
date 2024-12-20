using Microsoft.EntityFrameworkCore;
using HgznMes.Application.Dtos;
using HgznMes.Application.Services.Base;
using HgznMes.Domain.Shared.Exceptions;
using HgznMes.Infrastructure.DbContexts;

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

        public Task<IEnumerable<MenuReadDto>> GetAllMenusAsync()
        {
            throw new NotImplementedException();
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

        public Task<MenuReadDto> CreateMenuAsync(MenuCreateDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
