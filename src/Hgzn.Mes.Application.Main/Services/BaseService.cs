using AutoMapper;
using Hgzn.Mes.Application.Services.Base;

namespace Hgzn.Mes.Application.Services
{
    public class BaseService : IBaseService
    {
        public IMapper Mapper { get; init; } = null!;
    }
}