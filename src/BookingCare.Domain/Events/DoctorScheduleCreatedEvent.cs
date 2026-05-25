using BookingCare.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Events
{
    public record DoctorScheduleCreatedEvent(
        Guid ScheduleId,
        Guid DoctorId,
        DateOnly WorkDate,
        TimeOnly SlotStart
    ) : IDomainEvent;
}
