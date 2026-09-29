using Data.Common;
using Data.Entities;
using Repositories.Interface;
using System.Linq.Expressions;

namespace StudentHub.Tests.Fakes
{
    /// <summary>In-memory IGenericRepository for any master, mimicking soft delete and identity ids.</summary>
    public class FakeGenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        public List<T> Rows { get; } = [];
        private int _nextId = 1;

        private IEnumerable<T> Visible => Rows.Where(r => !r.IsDeleted);

        public Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            var query = filter is null ? Visible : Visible.Where(filter.Compile());
            return Task.FromResult<IReadOnlyList<T>>(query.OrderBy(r => r.Id).ToList());
        }

        public Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            var matching = (filter is null ? Visible : Visible.Where(filter.Compile())).OrderBy(r => r.Id).ToList();
            return Task.FromResult(new PagedResult<T>
            {
                Items = matching.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = matching.Count
            });
        }

        public Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Visible.FirstOrDefault(r => r.Id == id));

        public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
            Task.FromResult(Visible.Any(predicate.Compile()));

        public Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            entity.Id = _nextId++;
            Rows.Add(entity);
            return Task.FromResult(entity);
        }

        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var row = Visible.FirstOrDefault(r => r.Id == id);
            if (row is null) return Task.FromResult(false);
            row.IsDeleted = true;
            return Task.FromResult(true);
        }
    }
}
