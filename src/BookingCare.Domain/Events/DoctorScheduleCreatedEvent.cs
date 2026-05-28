using BookingCare.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Events
{
    public record DoctorScheduleCreatedEvent(
        Guid ScheduleId,
        Guid DoctorId,
        string DoctorName,
        DateOnly WorkDate,
        TimeOnly SlotStart,
        TimeOnly SlotEnd
    ) : IDomainEvent;
}
