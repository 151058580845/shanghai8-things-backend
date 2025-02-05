using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Domain.Entities.System.Authority;
using System.Security.Claims;
using Hgzn.Mes.Application.Main.Dtos.System;

namespace Hgzn.Mes.Application.Main.Services.System.IService
{
    public interface IMenuService : ICrudAppService<
        Menu, Guid,
        MenuReadDto, MenuQueryDto,
        MenuCreateDto, MenuUpdateDto>
    {
        Task<IEnumerable<MenuReaderRouterDto>> GetCurrentUserMenusAsTreeAsync(IEnumerable<Claim> claims);

        Task<IEnumerable<MenuReadDto>> GetRootMenusAsTreeAsync();

        Task<int> DeleteMenuAsync(Guid id, bool force);

        Task<int> SetMenuRouteAsync();
    }
}