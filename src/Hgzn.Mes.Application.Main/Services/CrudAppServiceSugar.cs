using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Services;

public abstract class CrudAppServiceSugar<TEntity, TKey, TReadDto, TCreateDto, TUpdateDto> : BaseService
    where TEntity : AggregateRoot, new()
    where TReadDto : ReadDto
    where TUpdateDto : UpdateDto
    where TCreateDto : CreateDto
{
    protected SqlSugarContext SqlSugarContext { get; } = null!;
    protected ISqlSugarClient DbContext { get; } = null!;

    protected ISugarQueryable<TEntity> Queryable()
    {
        return DbContext.Queryable<TEntity>();
    } 
    /// <summary>
    /// 创建服务
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<TReadDto> CreateAsync(TCreateDto dto)
    {
        var entity = Mapper.Map<TEntity>(dto);
        await DbContext.Insertable(entity).ExecuteCommandAsync();
        return Mapper.Map<TReadDto>(entity);
    }

    /// <summary>
    /// 修改服务
    /// </summary>
    /// <param name="key"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<TReadDto?> UpdateAsync(TKey key, TUpdateDto dto)
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

    /// <summary>
    /// 删除服务
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<int> DeleteAsync(TKey key)
    {
        var entity = await DbContext.Queryable<TEntity>().InSingleAsync(key);
        if (entity != null)
        {
            return await DbContext.Deleteable(entity).ExecuteCommandAsync();
        }
        return 0;
    }

    /// <summary>
    /// 根据主键获取实例
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<TReadDto> GetAsync(TKey key)
    {
        var entity = await DbContext.Queryable<TEntity>().InSingleAsync(key);
        return Mapper.Map<TReadDto>(entity);
    }
}