using BookingCare.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Common.Interfaces
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string hashedToken, CancellationToken ct = default);
        Task RevokeAllByUserIdAsync(Guid userId, CancellationToken ct = default);
    }
}
