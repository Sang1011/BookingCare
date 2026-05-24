using BookingCare.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Events
{
    public record AccountLockedEvent(
        Guid UserId,
        string Email,
        DateTime LockedUntil
    ) : IDomainEvent;
}
