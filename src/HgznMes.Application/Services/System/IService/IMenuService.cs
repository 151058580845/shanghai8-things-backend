
using HgznMes.Application.Dtos;
using HgznMes.Domain.Shared;
using System.Security.Claims;

namespace HgznMes.Application.Services.Base
{
    public interface IMenuService : IBaseService
    {
        Task<PaginatedList<MenuReadDto>> QueryMenusAsync(QueryMenuDto query);

        Task<IEnumerable<MenuReadDto>> GetCurrentUserMenusAsTreeAsync(IEnumerable<Claim> claims);

        Task<IEnumerable<MenuReadDto>> GetRootMenusAsTreeAsync();

        Task<int> DeleteMenuAsync(Guid id, bool force);

        Task<MenuReadDto> CreateMenuAsync(MenuCreateDto dto);

        Task<MenuReadDto> UpdateMenuAsync(Guid id, MenuUpdateDto dto);

        Task<int> SetMenuRouteAsync();
    }
}
