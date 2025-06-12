using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipMeasurementManager;
using Hgzn.Mes.Domain.Shared;
using Microsoft.AspNetCore.Http;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.Equip
{
    public class EquipMeasurementService : SugarCrudAppService<
    EquipMeasurement, Guid,
    EquipMeasurementReadDto, EquipMeasurementQueryDto, EquipMeasurementCreateDto, EquipMeasurementUpdateDto>,
    IEquipMeasurementService
    {
        public Task<EquipMeasurementReadDto> EquipMeasurementRefreshAsync(EquipMeasurementCreateDto refresh)
        {
            throw new NotImplementedException();
        }

        public async Task<List<EquipMeasurementReadDto>> GetMeasurementDue()
        {
            EquipMeasurement[] equipMeasurements = await DbContext.Queryable<EquipMeasurement>().ToArrayAsync();
            List<EquipMeasurementReadDto> ret = new List<EquipMeasurementReadDto>();
            foreach (EquipMeasurement item in equipMeasurements)
            {
                if (item.IsMeasurementDevice == true && item.ExpiryDate != null && (item.ExpiryDate - DateTime.Now).Value.TotalDays < 30)
                {
                    // 进入此判断说明需要计量
                    ret.Add(Mapper.Map<EquipMeasurementReadDto>(ret));
                }
            }
            return ret;
        }

        public async Task CheckExpiringMeasurementDevices(string filePath)
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
                string? header = headerRow.GetCell(col)?.ToString()?.Trim();
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

            EquipMeasurement[] oldEquipMeasurements = await DbContext.Queryable<EquipMeasurement>().ToArrayAsync();

            // 遍历每一行（从第 2 行开始）
            for (int row = 1; row <= sheet.LastRowNum; row++)
            {
                IRow dataRow = sheet.GetRow(row);
                if (dataRow == null)
                    continue; // 跳过空行

                // 检查是否是计量设备
                bool isMeasurementDevice = dataRow.GetCell(columnIndices["是否计量设备"])?.ToString()?.Trim() == "是" ? true : false;
                // 检查有效期
                string? expiryDateStr = dataRow.GetCell(columnIndices["有效期"])?.ToString()?.Trim();
                // 责任人
                string? responsiblePersonStr = dataRow.GetCell(columnIndices["责任人"])?.ToString()?.Trim();
                // 本地化资产编号
                string? localAssetNumberStr = dataRow.GetCell(columnIndices["本地化资产编号"])?.ToString()?.Trim();
                // 型号
                string? modelStr = dataRow.GetCell(columnIndices["型号"])?.ToString()?.Trim();
                // 资产名称
                string? assetNameStr = dataRow.GetCell(columnIndices["资产名称"])?.ToString()?.Trim();

                bool isExist = false;
                foreach (EquipMeasurement item in oldEquipMeasurements)
                {
                    if (item.AssetName == assetNameStr && item.Model == modelStr && item.LocalAssetNumber == localAssetNumberStr && item.IsMeasurementDevice == isMeasurementDevice &&
                        !string.IsNullOrEmpty(expiryDateStr) && item.ExpiryDate.ToString() != expiryDateStr)
                    {
                        // 进入此判断说明有效期不一致,需要更新
                        DbContext.Updateable(item).SetColumns(it => new EquipMeasurement()
                        {
                            ExpiryDate = DateTime.Parse(expiryDateStr),
                            ResponsiblePerson = responsiblePersonStr,
                            LocalAssetNumber = localAssetNumberStr,
                            Model = modelStr,
                            AssetName = assetNameStr
                        }).ExecuteCommand();
                        isExist = true;
                    }
                }
                if (!isExist && !string.IsNullOrEmpty(expiryDateStr))
                {
                    // 进入此判断说明没有找到,需要插入
                    EquipMeasurement equipMeasurement = new EquipMeasurement()
                    {
                        ExpiryDate = DateTime.Parse(expiryDateStr),
                        ResponsiblePerson = responsiblePersonStr,
                        LocalAssetNumber = localAssetNumberStr,
                        Model = modelStr,
                        AssetName = assetNameStr
                    };
                    DbContext.Insertable(equipMeasurement).ExecuteCommand();
                }
            }
        }

        public override Task<PaginatedList<EquipMeasurementReadDto>> GetPaginatedListAsync(EquipMeasurementQueryDto queryDto)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<EquipMeasurementReadDto>> GetListAsync(EquipMeasurementQueryDto? queryDto = null)
        {
            throw new NotImplementedException();
        }

        public async Task<bool?> EquipMeasurementRefreshAsync(IFormFile file)
        {
            var directoryPath = Path.Combine(Environment.CurrentDirectory, "attachs");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            var fullPath = Path.Combine(directoryPath, $"{DateTimeOffset.Now.ToUnixTimeMilliseconds}计量.xlsx");
            if (!File.Exists(fullPath))
            {
                using var reader = file.OpenReadStream();
                using var writer = new FileStream(fullPath, FileMode.Create);
                await reader.CopyToAsync(writer);
                writer.Close();
                reader.Close();
                await CheckExpiringMeasurementDevices(fullPath);
            }
            return true;
        }
    }
}
