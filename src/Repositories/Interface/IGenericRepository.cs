using Data.Common;
using Data.Entities;
using System.Linq.Expressions;

namespace Repositories.Interface
{
    /// <summary>
    /// One repository for every master table. Register once as an open generic;
    /// a new master (State, City, ...) needs no new repository class.
    /// Soft-deleted rows are hidden automatically by the global query filter.
    /// </summary>
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default);
        /// <summary>One page, ordered by Id. pageNumber starts at 1.</summary>
        Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default);
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
