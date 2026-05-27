using BookingCare.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Events
{
    public record DoctorPenaltyEvent(
        Guid BookingId,
        Guid DoctorId,
        Guid PatientId,
        DateTime CancelledAt,
        DateTime SlotStartTime
    ) : IDomainEvent;
}
