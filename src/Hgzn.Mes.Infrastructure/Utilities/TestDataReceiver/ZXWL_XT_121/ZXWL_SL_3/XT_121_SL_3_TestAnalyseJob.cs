using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_121_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_307_ReceiveDatas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_121.ZXWL_SL_3
{
    public class XT_121_SL_3_TestAnalyseJob
    {
        public List<DivisionTable> DivisionTables = new List<DivisionTable>();

        public XT_121_SL_3_TestAnalyseJob()
        {
            DivisionTables = InitializeDivisionTables();
        }

        public ApiResponse GetResponseForPushData(XT_121_SL_3_ReceiveData receiveData, List<string> tableNames)
        {
            ApiResponse ret = new ApiResponse();
            // 未知的展示表达
            return ret;
        }

        private List<DivisionTable> InitializeDivisionTables()
        {
            return new List<DivisionTable>
            {
                new DivisionTable
                {
                    startAndEnd = new List<(int, int)>{ (1, 10)},
                    title = "ST-36转台物理量",
                }
            };
        }

        private List<DivisionTable> GetTables(List<string> queryTables)
        {
            List<DivisionTable> ret = DivisionTables;
            if (queryTables == null) return ret;
            if (queryTables != null && queryTables.Count > 0)
            {
                ret = new List<DivisionTable>();
                foreach (string item in queryTables)
                {
                    foreach (DivisionTable table in DivisionTables)
                    {
                        if (item == table.title)
                            ret.Add(table);
                    }
                }
            }
            return ret;
        }
    }
}
