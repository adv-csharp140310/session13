using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace AppNLayer.DAL.Repositories;

public class DataAccess : IDataAccess // Implementing the new interface
{
    protected readonly AppNLayerDbContext _context;

    public DataAccess(AppNLayerDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    private DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
        => _context.Set<TEntity>();

    // --- Querying Methods ---
    public virtual async Task<TEntity?> GetByIdAsync<TEntity>(object id, CancellationToken cancellationToken = default) where TEntity : class
    {
        // FindAsync is optimized for finding by primary key(s)
        return await GetDbSet<TEntity>().FindAsync(new object?[] { id }, cancellationToken: cancellationToken);
    }

    public virtual async Task<List<TEntity>> GetAllAsync<TEntity>(CancellationToken cancellationToken = default) where TEntity : class
    {
        return await GetDbSet<TEntity>().AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>>? predicate = null, bool noTracking = false) where TEntity : class
    {
        IQueryable<TEntity> query = GetDbSet<TEntity>();
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        if (noTracking)
        {
            query = query.AsNoTracking();
        }
        return query;
    }

    // --- Command Methods ---
    public virtual async Task<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entity);
        return await GetDbSet<TEntity>().AddAsync(entity, cancellationToken);
    }

    public virtual EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entity);
        // Handles attaching if detached and marking as modified
        return GetDbSet<TEntity>().Update(entity);
    }

    public virtual EntityEntry<TEntity> Remove<TEntity>(TEntity entity) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entity);
        // Handles attaching if detached and marking as deleted
        return GetDbSet<TEntity>().Remove(entity);
    }

    public virtual void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entities);
        GetDbSet<TEntity>().RemoveRange(entities);
    }


    // --- Transaction/Commit Method ---
    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Optional: Add Cross-cutting concerns before saving (Auditing, etc.)
        // await ApplyAuditInfo(); // Example
        return await _context.SaveChangesAsync(cancellationToken);
    }
}