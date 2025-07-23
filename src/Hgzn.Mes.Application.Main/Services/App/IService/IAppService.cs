using Hgzn.Mes.Application.Main.Dtos.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.App.IService
{
    public interface IAppService
    {
        Task<ShowSystemHomeDataDto> GetTestListAsync();
        Task<ShowSystemDetailDto> GetTestDetailAsync(ShowSystemDetailQueryDto showSystemDetailQueryDto);
    }
}
