using AutoMapper;
using HgznMes.Application.Services.Base;

namespace HgznMes.Application.Services
{
    public class BaseService : IBaseService
    {
        public IMapper Mapper { get; init; } = null!;
    }
}