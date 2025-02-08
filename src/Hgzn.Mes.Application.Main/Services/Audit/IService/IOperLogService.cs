using Hgzn.Mes.Application.Main.Dtos.Audit;
using Hgzn.Mes.Domain.Entities.Audit;

namespace Hgzn.Mes.Application.Main.Services.Audit.IService;

public interface IOperLogService : ICrudAppService<
    OperatorLog, Guid,
    OperatorLogReadDto, OperatorLogQueryDto>
{
}