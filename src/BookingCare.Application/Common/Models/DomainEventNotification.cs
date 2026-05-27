using BookingCare.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Application.Common.Models
{
    public record DomainEventNotification<T>(T DomainEvent) : INotification
    where T : IDomainEvent;
}
