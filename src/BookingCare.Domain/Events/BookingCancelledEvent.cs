using BookingCare.Domain.Common;
using BookingCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Events
{
    public record BookingCancelledEvent(
        Guid BookingId,
        Guid PatientId,
        string Reason,
        CancelledBy CancelledBy
    ) : IDomainEvent;
}
