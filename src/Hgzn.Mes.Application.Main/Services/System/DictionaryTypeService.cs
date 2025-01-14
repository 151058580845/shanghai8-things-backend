using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.System.Dictionary;
using Hgzn.Mes.Domain.Shared;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Services.System;

/// <summary>
/// 字典类型
/// </summary>
public class DictionaryTypeService:CrudAppServiceSugar<DictionaryType
    ,Guid,DictionaryTypeQueryDto,DictionaryTypeReadDto,DictionaryTypeCreateDto,DictionaryTypeUpdateDto>,IDictionaryTypeService
{
    public override async Task<PaginatedList<DictionaryTypeReadDto>> GetListAsync(DictionaryTypeQueryDto input)
    {
        var entities = await Queryable()
            .WhereIF(input.DictName is not null, x => x.DictName.Contains(input.DictName!))
            .WhereIF(input.DictType is not null, x => x.DictType.Contains(input.DictType!))
            .WhereIF(input.State is not null, x => x.State == input.State)
            .WhereIF(input.StartTime is not null && input.EndTime is not null,
                x => x.CreationTime >= input.StartTime && x.CreationTime <= input.EndTime)
            .ToPageListAsync(input.PageIndex, input.PageSize);
        return Mapper.Map<PaginatedList<DictionaryTypeReadDto>>(entities);
    }
    /// <summary>
    /// 获取树形列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>

    public async Task<IEnumerable<TreeDto>> GetTreeListAsync(DictionaryTypeQueryDto input)
    {
        var p = await Queryable()
            .WhereIF(input.DictName is not null, x => x.DictName.Contains(input.DictName!))
            .Select(t => new TreeDto()
            {
                Id = t.Id,
                Name = t.DictName
            })
            .ToListAsync();
        var c = await DbContext.Queryable<DictionaryInfo>()
            .Select(t => new TreeDto()
            {
                Id = t.Id,
                Name = t.DictLabel,
                ParentId = t.ParentId
            })
            .ToListAsync();
        var list = new List<TreeDto>();
        list.AddRange(p);
        list.AddRange(c);
        return list;
    }
}