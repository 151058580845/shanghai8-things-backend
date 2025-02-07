using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.Utilities;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Services;

public abstract class SugarCrudAppService<TEntity, TKey, TReadDto> : BaseService,
    ICrudAppService<TEntity, TKey, TReadDto>
    where TEntity : AggregateRoot, new()
    where TReadDto : ReadDto
{
    /// <summary>
    ///     aoc属性注入
    /// </summary>
    public ISqlSugarClient DbContext { get; init; } = null!;

    protected ISugarQueryable<TEntity> Queryable
    {
        get
        {
            Console.WriteLine((DbContext == null));
            return DbContext.Queryable<TEntity>();
        }
    }

    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [ScopeDefinition(ScopeMethodType.Remove)]
    public virtual async Task<int> DeleteAsync(TKey key)
    {
        var entity = await DbContext.Queryable<TEntity>().InSingleAsync(key);
        if (entity != null)
        {
            return await DbContext.Deleteable(entity).ExecuteCommandAsync();
        }
        return 0;
    }

    /// <summary>
    /// 根据主键获取实体
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [ScopeDefinition(ScopeMethodType.Query)]
    public virtual async Task<TReadDto?> GetAsync(TKey key)
    {
        var entity = await DbContext.Queryable<TEntity>().InSingleAsync(key);
        return Mapper.Map<TReadDto>(entity);
    }
}

public abstract class SugarCrudAppService<TEntity, TKey, TReadDto, TQueryDto> :
    SugarCrudAppService<TEntity, TKey, TReadDto>,
    ICrudAppService<TEntity, TKey, TReadDto, TQueryDto>
    where TEntity : AggregateRoot, new()
    where TReadDto : ReadDto
    where TQueryDto : QueryDto
{
    /// <summary>
    /// 查询实体列表
    /// </summary>
    /// <param name="queryDto"></param>
    /// <returns></returns>
    [ScopeDefinition(ScopeMethodType.List)]
    public abstract Task<IEnumerable<TReadDto>> GetListAsync(TQueryDto? queryDto = null);

    /// <summary>
    /// 分页查询实体列表
    /// </summary>
    /// <param name="queryDto"></param>
    /// <returns></returns>
    [ScopeDefinition(ScopeMethodType.List)]
    public abstract Task<PaginatedList<TReadDto>> GetPaginatedListAsync(TQueryDto queryDto);
}

public abstract class SugarCrudAppService<TEntity, TKey, TReadDto, TQueryDto, TCreateDto> :
    SugarCrudAppService<TEntity, TKey, TReadDto, TQueryDto>,
    ICrudAppService<TEntity, TKey, TReadDto, TQueryDto, TCreateDto>
    where TEntity : AggregateRoot, new()
    where TReadDto : ReadDto
    where TCreateDto : CreateDto
    where TQueryDto : QueryDto
{
    /// <summary>
    /// 创建实体
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [ScopeDefinition(ScopeMethodType.Add)]
    public virtual async Task<TReadDto> CreateAsync(TCreateDto dto)
    {
        var entity = Mapper.Map<TEntity>(dto);
        await DbContext.Insertable(entity).ExecuteCommandAsync();
        return Mapper.Map<TReadDto>(entity);
    }
}

public abstract class SugarCrudAppService<TEntity, TKey, TReadDto, TQueryDto, TCreateDto, TUpdateDto> :
    SugarCrudAppService<TEntity, TKey, TReadDto, TQueryDto, TCreateDto>,
    ICrudAppService<TEntity, TKey, TReadDto, TQueryDto, TCreateDto, TUpdateDto>
    where TEntity : AggregateRoot, new()
    where TReadDto : ReadDto
    where TQueryDto : QueryDto
    where TUpdateDto : UpdateDto
    where TCreateDto : CreateDto
{
    /// <summary>
    /// 修改实体
    /// </summary>
    /// <param name="key"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [ScopeDefinition(ScopeMethodType.Edit)]
    public virtual async Task<TReadDto?> UpdateAsync(TKey key, TUpdateDto dto)
    {
        var entity = await DbContext.Queryable<TEntity>().InSingleAsync(key);
        if (entity == null)
        {
            return default;
        }
        Mapper.Map(dto, entity);
        await DbContext.Updateable(entity).ExecuteCommandAsync();
        return Mapper.Map<TReadDto>(entity);
    }
}