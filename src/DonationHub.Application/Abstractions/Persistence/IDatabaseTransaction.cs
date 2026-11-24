namespace DonationHub.Application.Abstractions.Persistence;

public interface IDatabaseTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);

}
