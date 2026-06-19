namespace FinanceControl.Infrastructure.Extensions;

public static class QueryablePagingExtensions
{
    public static IQueryable<T> Page<T>(this IQueryable<T> query, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }
}
