using BookingCare.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Common.Models
{
    public record DomainEventNotification<T>(T Event) : INotification
    where T : IDomainEvent;
}
