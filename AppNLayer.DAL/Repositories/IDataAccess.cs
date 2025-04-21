using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AppNLayer.DAL.Repositories;
public interface IDataAccess // Renamed from IGenericRepository for clarity
{
    // Querying Methods
    Task<TEntity?> GetByIdAsync<TEntity>(object id, CancellationToken cancellationToken = default) where TEntity : class;
    Task<List<TEntity>> GetAllAsync<TEntity>(CancellationToken cancellationToken = default) where TEntity : class; // Return List<T> commonly useful
    IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>>? predicate = null, bool noTracking = false) where TEntity : class;

    // Command Methods (operate on DbContext's change tracker)
    Task<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class;
    EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class;
    EntityEntry<TEntity> Remove<TEntity>(TEntity entity) where TEntity : class;
    void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;


    // Transaction/Commit Method
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

}
