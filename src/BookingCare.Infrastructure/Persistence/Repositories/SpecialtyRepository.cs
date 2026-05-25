using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Domain.Entities.Doctor;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Infrastructure.Persistence.Repositories
{
    public class SpecialtyRepository : BaseRepository<Specialty>, ISpecialtyRepository
    {
        public SpecialtyRepository(AppDbContext context) : base(context) { }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
            => await _dbSet.AnyAsync(s => s.Id == id, ct);

        public async Task<IReadOnlyList<Specialty>> GetAllAsync(CancellationToken ct = default)
            => await _dbSet.OrderBy(s => s.Name).ToListAsync(ct);
    }
}