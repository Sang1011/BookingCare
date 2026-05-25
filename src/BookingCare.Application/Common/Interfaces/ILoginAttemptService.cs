using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Common.Interfaces
{
    public interface ILoginAttemptService
    {
        Task<int> GetFailedAttemptsAsync(string email, CancellationToken ct = default);
        Task<int> IncrementAsync(string email, CancellationToken ct = default);
        Task ResetAsync(string email, CancellationToken ct = default);
        Task<bool> IsLockedAsync(string email, CancellationToken ct = default);
    }
}
