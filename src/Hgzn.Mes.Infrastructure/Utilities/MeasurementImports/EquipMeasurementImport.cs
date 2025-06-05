using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.MeasurementImports
{
    /// <summary>
    /// 设备计量导入
    /// </summary>
    public class EquipMeasurementImport
    {
        public static void CheckExpiringMeasurementDevices(string filePath)
        {
            // 用于存储结果的列表
            var results = new List<string>();

            IWorkbook workbook;
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                // 根据文件扩展名选择不同的 Workbook 实现
                if (filePath.EndsWith(".xlsx"))
                    workbook = new XSSFWorkbook(fileStream);
                else if (filePath.EndsWith(".xls"))
                    workbook = new HSSFWorkbook(fileStream);
                else
                    throw new NotSupportedException("文件格式不支持，仅支持 .xls 或 .xlsx 文件。");
            }

            // 获取第一个工作表
            ISheet sheet = workbook.GetSheetAt(0);

            // 获取标题行（假设第一行是标题）
            IRow headerRow = sheet.GetRow(0);
            if (headerRow == null)
                throw new InvalidDataException("Excel 文件没有标题行！");

            // 构建列名到列索引的映射
            Dictionary<string, int> columnIndices = new Dictionary<string, int>();
            for (int col = 0; col < headerRow.LastCellNum; col++)
            {
                string header = headerRow.GetCell(col)?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(header))
                    columnIndices[header] = col;
            }

            // 检查必要的列是否存在
            string[] requiredColumns = { "是否计量设备", "责任人", "本地化资产编号", "型号", "资产名称", "有效期" };
            foreach (var column in requiredColumns)
            {
                if (!columnIndices.ContainsKey(column))
                    throw new InvalidDataException($"Excel 文件中缺少必要的列：{column}");
            }

            // 获取当前日期
            DateTime currentDate = DateTime.Now;

            // 遍历每一行（从第 2 行开始）
            for (int row = 1; row <= sheet.LastRowNum; row++)
            {
                IRow dataRow = sheet.GetRow(row);
                if (dataRow == null)
                    continue; // 跳过空行

                // 检查是否是计量设备
                string isMeasurementDevice = dataRow.GetCell(columnIndices["是否计量设备"])?.ToString()?.Trim();
                if (isMeasurementDevice != "是")
                    continue;

                // 检查有效期
                string expiryDateStr = dataRow.GetCell(columnIndices["有效期"])?.ToString()?.Trim();
                if (string.IsNullOrEmpty(expiryDateStr))
                    continue;

                // 尝试解析日期（NPOI 可能返回不同的格式）
                if (DateTime.TryParse(expiryDateStr, out DateTime expiryDate))
                {
                    // 计算剩余天数
                    TimeSpan timeRemaining = expiryDate - currentDate;

                    // 如果有效期在 1 个月内（包括已过期的）
                    if (timeRemaining.TotalDays <= 30)
                    {
                        // 获取相关字段
                        string responsiblePerson = dataRow.GetCell(columnIndices["责任人"])?.ToString()?.Trim();
                        string localAssetNumber = dataRow.GetCell(columnIndices["本地化资产编号"])?.ToString()?.Trim();
                        string model = dataRow.GetCell(columnIndices["型号"])?.ToString()?.Trim();
                        string assetName = dataRow.GetCell(columnIndices["资产名称"])?.ToString()?.Trim();

                        // 构建输出字符串
                        string result = $"{responsiblePerson} {localAssetNumber} {model} {assetName} {expiryDate:yyyy-MM-dd} 即将到期，请及时计量";
                        results.Add(result);
                    }
                }
            }

            // 输出结果
            if (results.Count == 0)
            {
                Console.WriteLine("没有即将到期的计量设备。");
            }
            else
            {
                Console.WriteLine("以下计量设备即将到期：");
                foreach (var result in results)
                {
                    Console.WriteLine(result);
                }
            }
        }
    }
}
