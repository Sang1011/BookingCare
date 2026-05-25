using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context) { }

        public async Task<RefreshToken?> GetByTokenAsync(string hashedToken, CancellationToken ct = default)
            => await _dbSet.FirstOrDefaultAsync(r => r.Token == hashedToken, ct);

        public async Task RevokeAllByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            var tokens = await _dbSet
                .Where(r => r.UserId == userId && !r.IsRevoked)
                .ToListAsync(ct);

            tokens.ForEach(t => t.Revoke());
        }
    }
}
