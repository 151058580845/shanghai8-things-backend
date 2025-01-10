using Hgzn.Mes.Application.Dtos.Base;

namespace Hgzn.Mes.Application.BaseS;

public interface ICrudAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
    where TEntity : class
{
    Task<TGetOutputDto> CreateAsync(TCreateInput input);
    Task<TGetOutputDto?> UpdateAsync(TKey key, TUpdateInput input);
    Task<TGetOutputDto?> DeleteAsync(TKey key);
    Task<TGetOutputDto> GetAsync(TKey key);
    Task<IEnumerable<TGetListOutputDto>> GetListAsync(TGetListInput input);
}