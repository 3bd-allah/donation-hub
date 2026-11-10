using DonationHub.Domain.Primitives;
using System.Linq.Expressions;

namespace DonationHub.Application.Abstractions.Persistence;

public interface IBaseRepository<TEntity> where TEntity : Entity
{

    ValueTask<TEntity?> GetById(Guid entityId, CancellationToken cancellationToken=default);

    IQueryable<TEntity> GetAll();

    public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> expression);

    void Add(TEntity entity);

    void Update(TEntity entity);

    void Delete(TEntity entity);

    public Task<bool> IsExistAsync(Guid entityId, CancellationToken cancellationToken=default);


}
