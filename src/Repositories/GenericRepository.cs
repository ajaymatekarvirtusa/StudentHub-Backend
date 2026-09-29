using Data.Common;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.DBContext;
using Repositories.Interface;
using System.Linq.Expressions;

namespace Repositories
{
    public class GenericRepository<T>(ApplicationDbContext _context, ILogger<GenericRepository<T>> _logger)
        : IGenericRepository<T> where T : BaseEntity
    {
        private readonly DbSet<T> _dbSet = _context.Set<T>();
        private static readonly string EntityName = typeof(T).Name;

        public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();
            if (filter is not null)
                query = query.Where(filter);

            return await query.OrderBy(e => e.Id).ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();
            if (filter is not null)
                query = query.Where(filter);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(e => e.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<T> { Items = items, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };
        }

        // Tracked, so the caller can change properties and then call UpdateAsync.
        public Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
            _dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
            _dbSet.AnyAsync(predicate, cancellationToken);

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("{Entity} {Id} inserted", EntityName, entity.Id);
            return entity;
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("{Entity} {Id} updated", EntityName, entity.Id);
        }

        /// <summary>Soft delete: sets IsDeleted = true (the row stays in the table).</summary>
        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            if (entity is null)
                return false;

            entity.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("{Entity} {Id} soft-deleted", EntityName, id);
            return true;
        }
    }
}
