namespace DonationHub.Application.Abstractions.Persistence;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
