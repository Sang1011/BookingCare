using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Modules.Doctors.DTOs;
using BookingCare.Domain.Entities.Doctor;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Infrastructure.Persistence.Repositories
{
    public class DoctorRepository : BaseRepository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(AppDbContext context) : base(context) { }

        public async Task<Doctor?> GetByIdWithSpecialtyAsync(Guid id, CancellationToken ct = default)
            => await _dbSet
                .Include(d => d.Specialty)
                .FirstOrDefaultAsync(d => d.Id == id, ct);

        public async Task<bool> ExistsAsync(Guid userId, CancellationToken ct = default)
            => await _dbSet.AnyAsync(d => d.Id == userId, ct);

        public async Task<bool> LicenseNumberExistsAsync(string licenseNumber, CancellationToken ct = default)
            => await _dbSet.AnyAsync(d => d.LicenseNumber == licenseNumber, ct);

        public async Task<(IReadOnlyList<DoctorDto> Items, int TotalCount)> GetPagedAsync(
            string? searchName,
            Guid? specialtyId,
            DateOnly? availableDate,
            int page,
            int pageSize,
            CancellationToken ct = default)
        {
            var query = _context.Doctors
                .Join(_context.Users,
                    d => d.Id,
                    u => u.Id,
                    (d, u) => new { d, u })
                .Include(x => x.d.Specialty)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchName))
                query = query.Where(x =>
                    x.u.FullName.ToLower().Contains(searchName.ToLower()));

            if (specialtyId.HasValue)
                query = query.Where(x => x.d.SpecialtyId == specialtyId.Value);

            if (availableDate.HasValue)
                query = query.Where(x =>
                    x.d.Schedules.Any(s =>
                        s.WorkDate == availableDate.Value &&
                        s.IsAvailable));

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new DoctorDto(
                    x.d.Id,
                    x.u.FullName,
                    x.u.Email,
                    x.d.Specialty.Name,
                    x.d.LicenseNumber,
                    x.d.YearsOfExperience,
                    x.d.ConsultationFee,
                    x.d.Bio,
                    x.d.AvatarUrl,
                    x.d.IsVerified))
                .ToListAsync(ct);

            return (items, totalCount);
        }
    }
}