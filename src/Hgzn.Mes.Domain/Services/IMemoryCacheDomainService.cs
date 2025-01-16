using Hgzn.Mes.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Services
{
    public interface IMemoryCacheDomainService
    {
        Task<EquipStatus> GetOrAddAsync(string key, Func<Task<EquipStatus>> valueFactory);
    }
}
