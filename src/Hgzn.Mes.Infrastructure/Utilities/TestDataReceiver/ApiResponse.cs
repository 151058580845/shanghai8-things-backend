using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver
{
    // 定义 ApiResponse 类
    public class ApiResponse
    {
        public List<DataArea> data { get; set; }
    }

    // 定义 DataArea 类
    public class DataArea
    {
        public string title { get; set; }
        public List<Column> columns { get; set; }
        public List<Dictionary<string, object>> data { get; set; }
        public List<Span> span { get; set; }
    }

    // 定义 Column 类
    public class Column
    {
        public string field { get; set; }
        public string name { get; set; }
    }

    // 定义 Span 类
    public class Span
    {
        public int row { get; set; }
        public int col { get; set; }
        public int rowspan { get; set; }
        public int colspan { get; set; }
    }

    public class DivisionTable
    {
        /// <summary>
        /// 抬头
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 协议上的字段号起始位置和中止位置(包头包尾,按协议内容从1开始而非0)
        /// </summary>
        public List<(int, int)> startAndEnd { get; set; }
        /// <summary>
        /// 用于合并单元格
        /// </summary>
        public List<Span> spans { get; set; }
    }
}
