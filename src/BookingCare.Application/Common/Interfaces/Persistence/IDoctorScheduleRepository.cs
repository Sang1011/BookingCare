using BookingCare.Domain.Entities.Doctor;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Common.Interfaces.Persistence
{
    public interface IDoctorScheduleRepository : IRepository<DoctorSchedule>
    {
        Task<bool> HasConflictAsync(
            Guid doctorId,
            DateOnly workDate,
            TimeOnly slotStart,
            TimeOnly slotEnd,
            CancellationToken ct = default);

        Task<IReadOnlyList<DoctorSchedule>> GetAvailableSlotsByDateAsync(
            Guid doctorId,
            DateOnly date,
            CancellationToken ct = default);

        Task<bool> HasActiveBookingsAsync(Guid scheduleId, CancellationToken ct = default);
    }
}
