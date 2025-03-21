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
        public List<DataArea> Data { get; set; }
    }

    // 定义 DataArea 类
    public class DataArea
    {
        public string Title { get; set; }
        public List<Column> Columns { get; set; }
        public List<Dictionary<string, object>> Data { get; set; }
        public List<Span> Span { get; set; }
    }

    // 定义 Column 类
    public class Column
    {
        public string Field { get; set; }
        public string Name { get; set; }
    }

    // 定义 Span 类
    public class Span
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int Rowspan { get; set; }
        public int Colspan { get; set; }
    }

    public class DivisionTable
    {
        /// <summary>
        /// 抬头
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 协议上的字段号起始位置和中止位置(包头包尾,按协议内容从1开始而非0)
        /// </summary>
        public List<(int, int)> StartAndEnd { get; set; }
        /// <summary>
        /// 用于合并单元格
        /// </summary>
        public List<Span> Spans { get; set; }
    }
}
