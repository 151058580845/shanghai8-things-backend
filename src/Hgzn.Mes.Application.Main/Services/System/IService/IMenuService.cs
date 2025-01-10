using System.Security.Claims;
using Hgzn.Mes.Application.BaseS;
using Hgzn.Mes.Application.Dtos;
using Hgzn.Mes.Application.Services;
using Hgzn.Mes.Domain.Entities.System.Authority;
using Hgzn.Mes.Domain.Shared;

namespace Hgzn.Mes.Application.Main.Services.System.IService
{
    public interface IMenuService : IBaseService,ICrudAppService<Menu
        , MenuReadDto, MenuReadDto, Guid, MenuQueryDto, MenuCreateDto,
        MenuUpdateDto>
    {
        Task<PaginatedList<MenuReadDto>> QueryMenusAsync(MenuQueryDto query);

        Task<IEnumerable<MenuReadDto>> GetCurrentUserMenusAsTreeAsync(IEnumerable<Claim> claims);

        Task<IEnumerable<MenuReadDto>> GetRootMenusAsTreeAsync();

        Task<int> DeleteMenuAsync(Guid id, bool force);
        
        Task<int> SetMenuRouteAsync();
    }
}
