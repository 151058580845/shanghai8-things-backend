using AutoMapper;
using Hgzn.Mes.Infrastructure.SqlSugarContext;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Services;

public abstract class CrudAppServiceSugar<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
    where TEntity : class, new()
{
    protected SqlSugarContext SqlSugarContext { get; }
    protected ISqlSugarClient DbContext { get; }
    public IMapper Mapper { get; init; } = null!;
    protected CrudAppServiceSugar(SqlSugarContext dbContext)
    {
        SqlSugarContext = dbContext;
        DbContext=dbContext.DbContext;
    }

    protected ISugarQueryable<TEntity> Queryable()
    {
        return DbContext.Queryable<TEntity>();
    } 
    /// <summary>
    /// 创建服务
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<TGetOutputDto> CreateAsync(TCreateInput input)
    {
        var entity = Mapper.Map<TEntity>(input);
        await DbContext.Insertable(entity).ExecuteCommandAsync();
        return Mapper.Map<TGetOutputDto>(entity);
    }

    /// <summary>
    /// 修改服务
    /// </summary>
    /// <param name="key"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<TGetOutputDto?> UpdateAsync(TKey key,TUpdateInput input)
    {
        var entity = await DbContext.Queryable<TEntity>().InSingleAsync(key);
        if (entity == null)
        {
            return default;
        }
        Mapper.Map(input, entity);
        await DbContext.Updateable(entity).ExecuteCommandAsync();
        return Mapper.Map<TGetOutputDto>(entity);
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
    public async Task<TGetOutputDto> GetAsync(TKey key)
    {
        var entity = await DbContext.Queryable<TEntity>().InSingleAsync(key);
        return Mapper.Map<TGetOutputDto>(entity);
    }

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public abstract Task<IEnumerable<TGetListOutputDto>> GetListAsync(TGetListInput input);
}