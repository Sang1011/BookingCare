using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Common.Models;
using BookingCare.Application.Messages;
using BookingCare.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingCare.Application.Modules.Bookings.EventHandlers;

public class BookingRescheduledEventHandler(
    IBookingRepository bookingRepository,
    IMessagePublisher publisher,
    ILogger<BookingRescheduledEventHandler> logger)
    : INotificationHandler<DomainEventNotification<BookingRescheduledEvent>>
{
    public async Task Handle(
        DomainEventNotification<BookingRescheduledEvent> notification,
        CancellationToken ct)
    {
        var ev = notification.DomainEvent;

        logger.LogInformation(
            "Handling BookingRescheduledEvent. BookingId: {BookingId}", ev.BookingId);

        var detail = await bookingRepository.GetDetailByIdAsync(ev.BookingId, ct);
        if (detail is null)
        {
            logger.LogWarning(
                "BookingRescheduledEvent: booking not found. BookingId: {BookingId}", ev.BookingId);
            return;
        }

        await publisher.PublishAsync(new BookingRescheduledMessage(
            ev.BookingId,
            ev.PatientId,
            detail.PatientEmail,
            detail.PatientName,
            detail.DoctorName,
            detail.WorkDate,
            detail.SlotStart,
            detail.SlotEnd), ct);

        logger.LogInformation(
            "BookingRescheduledMessage published. BookingId: {BookingId}", ev.BookingId);
    }
}