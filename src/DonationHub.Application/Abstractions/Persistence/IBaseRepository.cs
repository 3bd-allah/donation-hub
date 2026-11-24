using DonationHub.Domain.Primitives;
using System.Linq.Expressions;

namespace DonationHub.Application.Abstractions.Persistence;

public interface IBaseRepository<TEntity> where TEntity : Entity
{

    ValueTask<TEntity?> GetByIdAsync(Guid entityId, CancellationToken cancellationToken=default);


    IQueryable<TEntity> GetAll();

    public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> expression);

    void Add(TEntity entity);

    void Update(TEntity entity);

    void Delete(TEntity entity);

    Task<int> DeleteAsync(Guid id, CancellationToken ct = default);

    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
    Task<bool> IsExistAsync(Guid entityId, CancellationToken cancellationToken = default);


    // Retrieve entities
    Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    // Retrieve projected data
    Task<TResult?> FirstOrDefaultAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResult>> selectors,
        CancellationToken cancellationToken = default,
        bool ignoreQueryFilter = false,
        bool noTracking = false
        );

    // Retrieve multiple entities
    Task<List<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    // Retrieve projected data
    Task<List<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, TResult>> selectors,
        CancellationToken cancellationToken = default,
        bool ignoreQueryFilter = false);

}
