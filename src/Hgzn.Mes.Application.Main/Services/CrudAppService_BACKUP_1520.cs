using AutoMapper;
<<<<<<< HEAD:src/HgznMes.Application/Services/CrudAppService.cs
using HgznMes.Infrastructure.DbContexts;

namespace HgznMes.Application.Services;
=======
using Hgzn.Mes.Application.Dtos.Base;
using Hgzn.Mes.Infrastructure.DbContexts;
using Hgzn.Mes.Infrastructure.Utilities;

namespace Hgzn.Mes.Application.BaseS;
>>>>>>> 948382fbbe02788b910f2cf9c47f3e39e0abb91b:src/Hgzn.Mes.Application.Main/Services/CrudAppService.cs

public abstract class CrudAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
    where TEntity : class
{
    protected ApiDbContext DbContext { get; }
    protected IMapper Mapper { get; init; } = null!;
    protected CrudAppService(ApiDbContext dbContext)
    {
        DbContext = dbContext;
    }

    /// <summary>
    /// 创建服务
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<TGetOutputDto> CreateAsync(TCreateInput input)
    {
        var entity = Mapper.Map<TEntity>(input);
        await DbContext.Set<TEntity>().AddAsync(entity);
        await DbContext.SaveChangesAsync();
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
        var entity = await DbContext.Set<TEntity>().FindAsync(key);
        if (entity == null)
        {
            return default;
        }
        Mapper.Map(input, entity);
        DbContext.Set<TEntity>().Update(entity);
        await DbContext.SaveChangesAsync();
        return Mapper.Map<TGetOutputDto>(entity);
    }

    /// <summary>
    /// 删除服务
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<TGetOutputDto?> DeleteAsync(TKey key)
    {
        var entity = await DbContext.Set<TEntity>().FindAsync(key);
        if (entity != null)
        {
            var delete = DbContext.Set<TEntity>().Remove(entity);
            return Mapper.Map<TGetOutputDto>(delete);
        }
        return default;
    }

    /// <summary>
    /// 根据主键获取实例
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<TGetOutputDto> GetAsync(TKey key)
    {
        var entity = await DbContext.Set<TEntity>().FindAsync(key);
        return Mapper.Map<TGetOutputDto>(entity);
    }

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public abstract Task<IEnumerable<TGetListOutputDto>> GetListAsync(TGetListInput input);
}