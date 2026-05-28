using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Domain.Entities.Auth;
using BookingCare.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Infrastructure.Persistence.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
            => await _dbSet.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);

        public async Task<bool> ExistsAsync(string email, CancellationToken ct = default)
            => await _dbSet.AnyAsync(u => u.Email == email.ToLowerInvariant(), ct);

        public async Task<IReadOnlyList<string>> GetAdminEmailsAsync(CancellationToken ct = default)
            => await _dbSet
                .AsNoTracking()
                .Where(u => u.Role == UserRole.Admin && u.IsActive)
                .Select(u => u.Email.Value)
                .ToListAsync(ct);

        public async Task<string?> GetEmailByDoctorIdAsync(Guid doctorId, CancellationToken ct = default)
            => await _context.Doctors
                .AsNoTracking()
                .Where(d => d.Id == doctorId && d.User.IsActive)
                .Select(d => d.User.Email.Value)
                .FirstOrDefaultAsync(ct);

        public async Task<IReadOnlyList<string>> GetAllUserEmailsAsync(CancellationToken ct = default)
            => await _dbSet
                .AsNoTracking()
                .Where(u => u.Role != UserRole.Admin && u.IsActive)
                .Select(u => u.Email.Value)
                .ToListAsync(ct);
    }
}
