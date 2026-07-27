namespace AspDotnetBoilerplate.src.Shared.Utils;

/// <summary>
/// Interface <c>ICrudService</c> define contracts for domain Services Interfaces.
/// </summary>
public interface ICrudService<TEntity, TCreateDto, TUpdateDto, TListDto, TResponseDto, TId>
{
    Task<TResponseDto> Create(TCreateDto request);
    Task<TResponseDto> Update(TUpdateDto request);
    Task<TResponseDto?> GetById(TId id);
    Task<PaginatedResult<TResponseDto>> List(TListDto request);
    Task SoftDelete(TId id);
    Task Restore(TId id);
    Task HardDelete(TId id);
}
