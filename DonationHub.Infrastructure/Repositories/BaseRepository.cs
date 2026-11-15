using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Domain.Primitives;
using DonationHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DonationHub.Infrastructure.Repositories;

public class BaseRepository<TEntity>(AppDbContext context) : IBaseRepository<TEntity> where TEntity : Entity
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public void Add(TEntity entity)
        => _dbSet.Add(entity);

    public Task<int> DeleteAsync (Guid id, CancellationToken ct = default)
    {
        return _dbSet.Where(e => e.Id == id)
                .ExecuteDeleteAsync(ct);
    }

    public void Delete(TEntity entity)
        => _dbSet.Remove(entity);

    public IQueryable<TEntity> GetAll() 
        => _dbSet;
    

    public ValueTask<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbSet.FindAsync([id], cancellationToken);
    

    public void Update(TEntity entity)
        =>_dbSet.Update(entity);
    
    public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> expression)
        =>_dbSet.Where(expression);

    public Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public Task<bool> IsExistAsync(Guid id, CancellationToken ct = default)
    {
        return _dbSet.AnyAsync(e => e.Id == id, ct);
    }
    
    // TODO: for updates
    public void SaveIncludes(Guid id, params string[] includes)
    {

    }

    public Task<TResult?> FirstOrDefaultAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate, 
        Expression<Func<TEntity, TResult>> selectors, 
        CancellationToken cancellationToken)
    {

        return _dbSet.Where(predicate)
            .Select(selectors)
            .FirstOrDefaultAsync(cancellationToken);

    }
    public Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {

        return _dbSet
            .Where(predicate)
            .FirstOrDefaultAsync(cancellationToken);
        
    }

    public Task<List<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate = null, 
        CancellationToken cancellationToken= default)
    {
        var query = _dbSet.AsNoTracking();

        if (predicate is not null)
            query = query.Where(predicate);

        return query.ToListAsync(cancellationToken);
    }


    public Task<List<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, TResult>> selectors,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking();

        if (predicate is not null)
            query = query.Where(predicate);

        return query
            .Select(selectors)
            .ToListAsync(cancellationToken);
    }
}
