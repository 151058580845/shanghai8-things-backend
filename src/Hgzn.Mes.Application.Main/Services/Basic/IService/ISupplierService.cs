using Hgzn.Mes.Application.Main.Dtos.Basic;
using Hgzn.Mes.Domain.Entities.Basic;

namespace Hgzn.Mes.Application.Main.Services.Basic.IService
{
    /// <summary>
    /// 供应商服务
    /// </summary>
    public interface ISupplierService : ICrudAppService<
    Supplier, Guid,
    SupplierReadDto, SupplierQueryDto,
    SupplierCreateDto, SupplierUpdateDto>
    {
        Task<List<SupplierDetilDto>> GetGetSupplierDetilInfos(Guid id);
    }
}
