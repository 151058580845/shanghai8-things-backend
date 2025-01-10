using AutoMapper;

namespace Hgzn.Mes.Application.Main.Services
{
    public class BaseService : IBaseService
    {
        public IMapper Mapper { get; init; } = null!;
    }
}