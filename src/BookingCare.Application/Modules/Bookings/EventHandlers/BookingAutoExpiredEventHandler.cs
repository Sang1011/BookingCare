using BookingCare.Application.Common.Models;
using BookingCare.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingCare.Application.Modules.Bookings.EventHandlers
{
    public class BookingAutoExpiredEventHandler(
        ILogger<BookingAutoExpiredEventHandler> logger)
        : INotificationHandler<DomainEventNotification<BookingAutoExpiredEvent>>
    {
        public Task Handle(
            DomainEventNotification<BookingAutoExpiredEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            logger.LogInformation(
                "Booking auto expired. BookingId: {BookingId}, PatientId: {PatientId}, DoctorScheduleId: {DoctorScheduleId}",
                domainEvent.BookingId,
                domainEvent.PatientId,
                domainEvent.DoctorScheduleId);

            return Task.CompletedTask;
        }
    }
}