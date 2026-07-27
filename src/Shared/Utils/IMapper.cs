namespace AspDotnetBoilerplate.src.Shared.Utils;

/// <summary>
/// Interface <c>Mapper</c> define contracts for DTO Mappers.
/// </summary>
public interface IMapper<TEntity, TDto>
{
    static TDto ToDto(TEntity entity) => throw new NotImplementedException();
    
    static List<TDto> ToDto(List<TEntity> entities) => entities.Select(e => ToDto(e)).ToList();
}