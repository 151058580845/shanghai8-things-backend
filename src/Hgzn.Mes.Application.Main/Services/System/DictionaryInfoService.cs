using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.System.Dictionary;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;

namespace Hgzn.Mes.Application.Main.Services.System;

/// <summary>
/// 字典详情
/// </summary>
public class DictionaryInfoService : SugarCrudAppService<
    DictionaryInfo, Guid,
    DictionaryInfoReadDto, DictionaryInfoQueryDto,
    DictionaryInfoCreateDto, DictionaryInfoUpdateDto>,
    IDictionaryInfoService
{
    
    public override async Task<IEnumerable<DictionaryInfoReadDto>> GetListAsync(DictionaryInfoQueryDto? queryDto)
    {
        var entities = await Queryable
            .WhereIF(queryDto?.DictLabel is not null, x => queryDto != null && x.DictLabel.Contains(queryDto.DictLabel!))
            .WhereIF(queryDto != null && queryDto.ParentId is not null, x => queryDto != null && x.ParentId == queryDto.ParentId)
            .WhereIF(queryDto != null && queryDto.State is not null, x => queryDto != null && x.State == queryDto.State)
            .ToListAsync();
        return Mapper.Map<IEnumerable<DictionaryInfoReadDto>>(entities);
    }
    public override async Task<PaginatedList<DictionaryInfoReadDto>> GetPaginatedListAsync(DictionaryInfoQueryDto queryDto)
    {
        var entities = await Queryable
            .WhereIF(queryDto.DictLabel is not null, x => x.DictLabel.Contains(queryDto.DictLabel!))
            .WhereIF(queryDto.ParentId is not null, x => x.ParentId == queryDto.ParentId)
            .WhereIF(queryDto.State is not null, x => x.State == queryDto.State)
            .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
        return Mapper.Map<PaginatedList<DictionaryInfoReadDto>>(entities);
    }

    /// <summary>
    /// 根据字典类型获取数据
    /// </summary>
    /// <param name="dictType"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>

    public  async Task<List<NameValueDto>> GetNameValueByTypeAsync(string dictType)
    {
        var type = await DbContext.Queryable<DictionaryType>()
            .FirstAsync(t => t.DictType == dictType);
        if (type == null)
        {
            throw new Exception();
        }

        return await GetNameValueByTypeIdAsync(type.Id);
    }

    /// <summary>
    /// 根据字典类型主键获取数据
    /// </summary>
    /// <param name="dictTypeId"></param>
    /// <returns></returns>
    public async Task<List<NameValueDto>> GetNameValueByTypeIdAsync(Guid dictTypeId)
    {
        return
            await Queryable
                .Where(t => t.ParentId == dictTypeId)
                .Select<NameValueDto>(t => new NameValueDto()
                {
                    Id = t.Id,
                    Name = t.DictLabel,
                    Value = t.DictValue
                }).ToListAsync();
    }


}