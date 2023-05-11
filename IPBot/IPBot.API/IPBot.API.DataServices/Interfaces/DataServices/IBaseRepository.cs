using System.Linq.Expressions;

namespace IPBot.API.DataServices.Interfaces.DataServices;

public interface IBaseRepository<T> where T : class
{
    Task<T> GetByIdAsync(object id);
    Task<IList<T>> GetAllAsync();
    Task<IList<T>> GetAllWhereAsync(Expression<Func<T, bool>> expression);
    Task<T> GetWhereAsync(Expression<Func<T, bool>> expression);
    Task<bool> AddAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteAsync(T entity);
}