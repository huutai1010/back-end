using Common.Models;

using System.Linq.Expressions;

namespace DAL.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {

        Task<PagedResult<TResult>> GetAsync<TResult>(QueryParameters queryParameters,
            bool caching = false,
            bool includeDeleted = false,
            Expression<Func<T, object>>? orderBy = null,
            bool descending = true);
        Task<List<TResult>> GetAsync<TResult>(
            bool includeDeleted = false,
            bool caching = false,
            Expression<Func<T, object>>? orderBy = null,
            bool descending = true);
        Task<bool> CreateAsync(T entity);
        Task<bool> CreateManyAsync(params T[] entities);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        Task<T?> FindByIdAsync(params object[] keys);
        Task<bool> Exist(params object[] keys);
        void DetachedPlaceInstance(T entity);

        Task<PagedResult<TResult>> GetAsyncWithConditions<TResult>(QueryParameters queryParameters, bool includeDeleted = false, Func<IQueryable<T>, IQueryable<T>>? queryConditions = null);
        Task<List<TResult>> GetAsyncWithConditions<TResult>(bool includeDeleted = false, Func<IQueryable<T>, IQueryable<T>>? queryConditions = null);
    }
}
