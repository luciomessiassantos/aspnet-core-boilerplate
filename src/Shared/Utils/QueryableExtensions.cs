namespace AspDotnetBoilerplate.src.Shared.Utils;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

public static class QueryableExtensions
{

    //<summary>
    //  Extensão do IQueryable para realizar, em uma linha, uma condição que, se verdadeira
    //  executa um Where no banco de dados com o predicado fornecido
    //  retorna o IQueryable incrementado
    //</summary>

    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source, 
        bool condition, 
        Expression<Func<T, bool>> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }

    //<summary>
    //  retorna qualquer listagem de consulta IQueryable em uma entidade de paginação
    //</summary>

    public static async Task<PaginatedResult<TResult>> ToPaginatedResultAsync<TSource, TResult>(
        this IQueryable<TSource> query,
        int pageNumber,
        Expression<Func<TSource, TResult>> selector,
        int pageSize = 12,
        CancellationToken cancellationToken = default
        )
    {
        int totalCount = await query.CountAsync(cancellationToken);

        var data = await query
            .Select(selector)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<TResult>
        {
            Data = data,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}