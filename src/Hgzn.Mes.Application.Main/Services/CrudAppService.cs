using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Infrastructure.DbContexts.Ef;

namespace Hgzn.Mes.Application.Main.Services;

public abstract class CrudAppService<TEntity, TKey, TReadDto, TCreateDto, TUpdateDto> : BaseService
    where TEntity : AggregateRoot
    where TReadDto : ReadDto
    where TUpdateDto : UpdateDto
    where TCreateDto : CreateDto
{
    protected ApiDbContext DbContext { get; }
    protected CrudAppService(ApiDbContext dbContext)
    {
        DbContext = dbContext;
    }

    protected IQueryable<TEntity> Queryable()
    {
        return DbContext.Set<TEntity>().AsQueryable();
    }

    /// <summary>
    /// 创建服务
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<TReadDto> CreateAsync(TCreateDto dto)
    {
        var entity = Mapper.Map<TEntity>(dto);
        await DbContext.Set<TEntity>().AddAsync(entity);
        await DbContext.SaveChangesAsync();
        return Mapper.Map<TReadDto>(entity);
    }

    /// <summary>
    /// 修改服务
    /// </summary>
    /// <param name="key"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<TReadDto?> UpdateAsync(TKey key,TUpdateDto dto)
    {
        var entity = await DbContext.Set<TEntity>().FindAsync(key);
        if (entity == null)
        {
            return default;
        }
        Mapper.Map(dto, entity);
        DbContext.Set<TEntity>().Update(entity);
        await DbContext.SaveChangesAsync();
        return Mapper.Map<TReadDto>(entity);
    }

    /// <summary>
    /// 删除服务
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<TReadDto?> DeleteAsync(TKey key)
    {
        var entity = await DbContext.Set<TEntity>().FindAsync(key);
        if (entity != null)
        {
            var delete = DbContext.Set<TEntity>().Remove(entity);
            return Mapper.Map<TReadDto>(delete);
        }
        return default;
    }

    /// <summary>
    /// 根据主键获取实例
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<TReadDto> GetAsync(TKey key)
    {
        var entity = await DbContext.Set<TEntity>().FindAsync(key);
        return Mapper.Map<TReadDto>(entity);
    }

}