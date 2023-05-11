using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace IPBot.API.Repositories.ExtensionMethods;

public static class QueryableExtensions
{
    public static IQueryable<T> IncludeProperties<T>(this IQueryable<T> query, params Expression<Func<T, object>>[] includes)
        where T : class
    { 
        return includes.Aggregate(query, (current, include) => current.Include(include));
    }
}