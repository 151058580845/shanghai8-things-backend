using Hgzn.Mes.Application.Main.Dtos.Basic;
using Hgzn.Mes.Domain.Entities.Basic;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.Basic.IService
{
    public interface ICustomerService : ICrudAppService<
    Customer, Guid,
    CustomerReadDto, CustomerQueryDto,
    CustomerCreateDto, CustomerUpdateDto>
    {
    }
}
