using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Common.Interfaces.Services
{
    public interface IEmailVerificationService
    {
        Task<string> GenerateTokenAsync(Guid userId, CancellationToken ct = default);
        Task<Guid?> ValidateTokenAsync(string token, CancellationToken ct = default);
        Task RemoveTokenAsync(Guid userId, CancellationToken ct = default);
    }
}
