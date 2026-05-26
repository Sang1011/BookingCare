using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Domain.Entities.Doctor;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Infrastructure.Persistence.Repositories
{
    public class DoctorScheduleRepository : BaseRepository<DoctorSchedule>, IDoctorScheduleRepository
    {
        public DoctorScheduleRepository(AppDbContext context) : base(context) { }

        public async Task<bool> HasConflictAsync(
            Guid doctorId,
            DateOnly workDate,
            TimeOnly slotStart,
            TimeOnly slotEnd,
            CancellationToken ct = default)
            => await _dbSet.AnyAsync(s =>
                s.DoctorId == doctorId &&
                s.WorkDate == workDate &&
                s.SlotStart < slotEnd &&
                s.SlotEnd > slotStart, ct);

        public async Task<IReadOnlyList<DoctorSchedule>> GetAvailableSlotsByDateAsync(
            Guid doctorId,
            DateOnly date,
            CancellationToken ct = default)
            => await _dbSet
                .Where(s =>
                    s.DoctorId == doctorId &&
                    s.WorkDate == date &&
                    s.IsAvailable)
                .OrderBy(s => s.SlotStart)
                .ToListAsync(ct);

        public async Task<bool> HasActiveBookingsAsync(
            Guid scheduleId,
            CancellationToken ct = default)
            => await _context.Bookings.AnyAsync(b =>
                b.DoctorScheduleId == scheduleId &&
                b.Status != Domain.Enums.BookingStatus.Cancelled, ct);

        public async Task<DoctorSchedule?> GetByIdWithDoctorAsync(
            Guid scheduleId,
            CancellationToken ct = default)
            => await _dbSet
                .Include(s => s.Doctor)
                .FirstOrDefaultAsync(s => s.Id == scheduleId, ct);
    }
}