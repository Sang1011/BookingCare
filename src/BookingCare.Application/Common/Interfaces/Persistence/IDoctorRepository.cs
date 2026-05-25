using BookingCare.Domain.Entities.Doctor;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Common.Interfaces.Persistence
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        Task<Doctor?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid userId, CancellationToken ct = default);
    }
}
