using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_307_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_307.ZXWL_SL_1;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.Equip
{
    public class TestAnalyseServer : SugarCrudAppService<
    TestAnalyse, Guid,
    TestAnalyseReadDto, TestAnalyseQueryDto>, ITestAnalyseServer
    {
        private XT_307_SL_1_TestAnalyseJob _testAnalyseJob;

        public TestAnalyseServer(XT_307_SL_1_TestAnalyseJob testAnalyseJob)
        {
            _testAnalyseJob = testAnalyseJob;
        }

        public override async Task<IEnumerable<TestAnalyseReadDto>> GetListAsync(TestAnalyseQueryDto? queryDto = null)
        {
            // 本应去数据库查询,但是没有数据,先模拟一些数据出来
            XT_307_SL_1_ReceiveData receiveData = await DbContext.Queryable<XT_307_SL_1_ReceiveData>().OrderByDescending(x => x.CreationTime).FirstAsync();
            if (receiveData == null) receiveData = new XT_307_SL_1_ReceiveData(); // 如果没有查询到则全部返回0
            if (queryDto == null) return null!;
            ApiResponse ret = _testAnalyseJob.GetResponse(receiveData, queryDto.FormTypes!);
            TestAnalyseReadDto readDto = new TestAnalyseReadDto() { Response = ret };
            return await Task.FromResult(new List<TestAnalyseReadDto>() { readDto });
        }

        public async override Task<PaginatedList<TestAnalyseReadDto>> GetPaginatedListAsync(TestAnalyseQueryDto queryDto)
        {
            // 本应去数据库查询,但是没有数据,先模拟一些数据出来
            XT_307_SL_1_ReceiveData receiveData = new XT_307_SL_1_ReceiveData();
            if (queryDto == null) return null!;
            ApiResponse ret = _testAnalyseJob.GetResponse(receiveData, queryDto.FormTypes!);
            TestAnalyseReadDto readDto = new TestAnalyseReadDto() { Response = ret };
            return await Task.FromResult(new PaginatedList<TestAnalyseReadDto>(new List<TestAnalyseReadDto>() { readDto }, 1, 1, 1));
        }


    }
}
