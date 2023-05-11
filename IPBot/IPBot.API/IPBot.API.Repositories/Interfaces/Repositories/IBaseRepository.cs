using System.Linq.Expressions;

namespace IPBot.API.Repositories.Interfaces.Repositories;

public interface IBaseRepository<T> where T : class
{
    Task<T> GetByIdAsync(object id);
    Task<IList<T>> GetAllAsync();
    Task<IList<T>> GetAllWhereAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes);
    Task<T> GetWhereAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes);
    Task<bool> AddAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteAsync(T entity);
}