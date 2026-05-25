using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Domain.Entities.Doctor;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Infrastructure.Persistence.Repositories
{
    public class DoctorRepository : BaseRepository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(AppDbContext context) : base(context) { }

        public async Task<Doctor?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
            => await _dbSet.FirstOrDefaultAsync(d => d.Id == userId, ct);

        public async Task<bool> ExistsAsync(Guid userId, CancellationToken ct = default)
            => await _dbSet.AnyAsync(d => d.Id == userId, ct);
    }
}
