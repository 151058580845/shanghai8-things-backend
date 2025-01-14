using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Domain.Entities.System.Authority;
using Hgzn.Mes.Domain.Shared;
using System.Security.Claims;

namespace Hgzn.Mes.Application.Main.Services.System.IService
{
    public interface IMenuService : IBaseService, ICrudAppService<
        Menu, Guid, MenuQueryDto, MenuReadDto, MenuCreateDto, MenuUpdateDto>
    {
        Task<PaginatedList<MenuReadDto>> QueryMenusAsync(MenuQueryDto query);

        Task<IEnumerable<MenuReadDto>> GetCurrentUserMenusAsTreeAsync(IEnumerable<Claim> claims);

        Task<IEnumerable<MenuReadDto>> GetRootMenusAsTreeAsync();

        Task<int> DeleteMenuAsync(Guid id, bool force);

        Task<int> SetMenuRouteAsync();
    }
}