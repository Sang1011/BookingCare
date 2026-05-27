using BookingCare.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Events
{
    public record BookingAutoExpiredEvent(
        Guid BookingId,
        Guid PatientId,
        Guid DoctorScheduleId
    ) : IDomainEvent;
}
