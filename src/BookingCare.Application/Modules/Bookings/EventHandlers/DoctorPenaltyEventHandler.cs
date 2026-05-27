using BookingCare.Application.Common.Models;
using BookingCare.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingCare.Application.Modules.Bookings.EventHandlers
{
    public class DoctorPenaltyEventHandler(
        ILogger<DoctorPenaltyEventHandler> logger)
        : INotificationHandler<DomainEventNotification<DoctorPenaltyEvent>>
    {
        public Task Handle(
            DomainEventNotification<DoctorPenaltyEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            logger.LogWarning(
                "Doctor penalty triggered. BookingId: {BookingId}, DoctorId: {DoctorId}, PatientId: {PatientId}, CancelledAt: {CancelledAt}, SlotStartTime: {SlotStartTime}",
                domainEvent.BookingId,
                domainEvent.DoctorId,
                domainEvent.PatientId,
                domainEvent.CancelledAt,
                domainEvent.SlotStartTime);

            return Task.CompletedTask;
        }
    }
}