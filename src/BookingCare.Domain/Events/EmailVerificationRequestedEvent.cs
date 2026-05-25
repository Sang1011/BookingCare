using BookingCare.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Events
{
    public record EmailVerificationRequestedEvent(
        Guid UserId,
        string Email,
        string Token
    ) : IDomainEvent;
}
