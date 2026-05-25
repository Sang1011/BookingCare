using BookingCare.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Common.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<bool> ExistsAsync(string email, CancellationToken ct = default);
    }
}
