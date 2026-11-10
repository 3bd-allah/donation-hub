using DonationHub.Domain.Entities;

namespace DonationHub.Application.Abstractions.Persistence;

public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenHashAsync(string hashToken, CancellationToken cancellationToken);
}
