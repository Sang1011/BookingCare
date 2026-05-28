using BookingCare.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Common.Interfaces.Persistence
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<bool> ExistsAsync(string email, CancellationToken ct = default);
        Task<IReadOnlyList<string>> GetAdminEmailsAsync(CancellationToken ct = default);
        Task<string?> GetEmailByDoctorIdAsync(Guid doctorId, CancellationToken ct = default);
        Task<IReadOnlyList<string>> GetAllUserEmailsAsync(CancellationToken ct = default);
    }
}
