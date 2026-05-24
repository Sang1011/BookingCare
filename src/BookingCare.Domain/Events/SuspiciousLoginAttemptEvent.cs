using BookingCare.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Events
{
    public record SuspiciousLoginAttemptEvent(
        Guid UserId,
        string Email
    ) : IDomainEvent;
}
