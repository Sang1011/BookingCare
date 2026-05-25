using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Domain.Entities.Auth;
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
    }
}
