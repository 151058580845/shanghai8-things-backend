using System.Reflection;
using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.ValueObjects;

namespace Hgzn.Mes.WebApi.Utilities;

/// <summary>
/// 遵循命名规范的，统一进行映射，后续单独配置的映射会自动覆盖该映射
/// ReadDto
/// CreateDto
/// UpdateDto
/// </summary>
public class BaseProfile : Profile
{
    public BaseProfile()
    {
        //映射权限
        CreateMap<ScopeDefinition, ScopeDefReadDto>();
        
        var dtoType = Assembly.Load("Hgzn.Mes." + nameof(Application)+".Main")
            .GetTypes()
            .Where(t => t.Namespace != null && t.Namespace.Contains("Dtos") && !t.Namespace.Contains("Base"))
            .ToList();
        var entityTypes = Assembly.Load("Hgzn.Mes." + nameof(Domain))
            .GetTypes()
            .Where(t => (typeof(AggregateRoot)).IsAssignableFrom(t) && !t.Namespace!.Contains("Base"))
            .ToArray();
        var readType = typeof(ReadDto);
        var createType = typeof(CreateDto);
        var updateType = typeof(UpdateDto);
        
        foreach (var dto in dtoType)
        {
            if (dto.IsSubclassOf(readType))
            {
                var entityName = dto.Name.Replace(nameof(ReadDto), string.Empty);
                var entity = entityTypes.FirstOrDefault(t => t.Name == entityName);
                if (entity != null)
                    CreateMap(entity, dto);
            }
            else if (dto.IsSubclassOf(createType))
            {
                var entityName = dto.Name.Replace(nameof(CreateDto), string.Empty);
                var entity = entityTypes.FirstOrDefault(t => t.Name == entityName);
                if (entity != null)
                    CreateMap(dto, entity);
            }
            else if (dto.IsSubclassOf(updateType))
            {
                var entityName = dto.Name.Replace(nameof(UpdateDto), string.Empty);
                var entity = entityTypes.FirstOrDefault(t => t.Name == entityName);
                if (entity != null)
                    CreateMap(dto, entity);
            }
        }
    }
}