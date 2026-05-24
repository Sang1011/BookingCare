using BookingCare.Domain.Common;
using BookingCare.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Entities.Doctor
{
    public class DoctorSchedule : BaseEntity
    {
        public Guid DoctorId { get; private set; }
        public DateOnly WorkDate { get; private set; }
        public TimeOnly SlotStart { get; private set; }
        public TimeOnly SlotEnd { get; private set; }
        public bool IsAvailable { get; private set; } = true;
        public byte[] RowVersion { get; private set; } = default!;

        private DoctorSchedule() { }

        public static Result<DoctorSchedule> Create(
            Guid doctorId, DateOnly workDate,
            TimeOnly slotStart, TimeOnly slotEnd)
        {
            if (slotEnd <= slotStart)
                return Result<DoctorSchedule>.Failure(DoctorScheduleErrors.InvalidSlotTime);

            return Result<DoctorSchedule>.Success(new DoctorSchedule
            {
                DoctorId = doctorId,
                WorkDate = workDate,
                SlotStart = slotStart,
                SlotEnd = slotEnd
            });
        }

        public bool IsExpired() =>
            WorkDate.ToDateTime(SlotStart) < DateTime.UtcNow;
    }
}
