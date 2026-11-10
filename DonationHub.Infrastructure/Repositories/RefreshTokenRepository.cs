using DonationHub.Application.Abstractions.Persistence;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Infrastructure.Repositories;

public class RefreshTokenRepository(AppDbContext context) : BaseRepository<RefreshToken>(context), IRefreshTokenRepository
{
    private readonly AppDbContext _context = context;

    public async Task<RefreshToken?> GetByTokenHashAsync(string hashToken, CancellationToken ct)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == hashToken, ct);
    }
}
