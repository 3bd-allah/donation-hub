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
    

    public void Delete(TEntity entity)
        => _dbSet.Remove(entity);

    public IQueryable<TEntity> GetAll() 
        => _dbSet.AsNoTracking();
    

    public ValueTask<TEntity?> GetById(Guid id, CancellationToken cancellationToken)
        => _dbSet.FindAsync(id, cancellationToken);
    

    public void Update(TEntity entity)
        =>_dbSet.Update(entity);
    
    public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> expression)
        =>_dbSet.Where(expression);


    //TODO: is Exist
    public Task<bool> IsExistAsync(Guid id, CancellationToken ct)
    {
        return _dbSet.AnyAsync(e => e.Id == id, ct);
    }
    
    // TODO: for updates
    public void SaveIncludes(Guid id, params string[] includes)
    {

    }

}
