using BookingCare.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Events
{
    public record BookingCreatedEvent(
        Guid BookingId,
        Guid PatientId,
        Guid DoctorScheduleId
    ) : IDomainEvent;
}
