using System.Linq.Expressions;
using IPBot.API.Domain.Data;
using IPBot.API.Domain.ExtensionMethods;
using IPBot.API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IPBot.API.Domain.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    private readonly IIPBotDataContext _ipBotDataContext;

    protected BaseRepository(IIPBotDataContext ipBotDataContext)
    {
        _ipBotDataContext = ipBotDataContext;
    }

    public async Task<T> GetByIdAsync(object id)
    {
        return await _ipBotDataContext.Set<T>().FindAsync(id);
    }

    public async Task<IList<T>> GetAllAsync()
    {
        return await _ipBotDataContext.Set<T>().ToListAsync();
    }

    public async Task<IList<T>> GetAllWhereAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
    {
        return await _ipBotDataContext.Set<T>()
            .Where(expression)
            .IncludeProperties(includes)
            .ToListAsync();
    }

    public async Task<T> GetWhereAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
    {
        return await _ipBotDataContext.Set<T>()
            .Where(expression)
            .IncludeProperties(includes)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> AddAsync(T entity)
    {
        if (await _ipBotDataContext.Set<T>().ContainsAsync(entity))
            return false;

        await _ipBotDataContext.Set<T>().AddAsync(entity);
        return await _ipBotDataContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        _ipBotDataContext.Set<T>().Attach(entity).State = EntityState.Modified;
        return await _ipBotDataContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(T entity)
    {
        _ipBotDataContext.Set<T>().Remove(entity);
        return await _ipBotDataContext.SaveChangesAsync() > 0;
    }
}