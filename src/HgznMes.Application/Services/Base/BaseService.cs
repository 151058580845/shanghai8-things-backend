using AutoMapper;

namespace HgznMes.Application.Services.Base
{
    public class BaseService : IBaseService
    {
        public IMapper Mapper { get; init; } = null!;
    }
}