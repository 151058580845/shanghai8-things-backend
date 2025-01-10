using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Entities.Base;

namespace Hgzn.Mes.Application.Main.Services;

public interface ICrudAppService<TEntity, TKey, TReadDto, TCreateDto, TUpdateDto> : IBaseService
    where TEntity : AggregateRoot
    where TReadDto : ReadDto
    where TUpdateDto : UpdateDto
    where TCreateDto : CreateDto
{
    Task<TReadDto> CreateAsync(TCreateDto dto);
    Task<TReadDto?> UpdateAsync(TKey key, TUpdateDto dto);
    Task<int> DeleteAsync(TKey key);
    Task<TReadDto> GetAsync(TKey key);
}