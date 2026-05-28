using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Common.Models;
using BookingCare.Application.Messages;
using BookingCare.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingCare.Application.Modules.Bookings.EventHandlers;

public class BookingCancelledEventHandler(
    IBookingRepository bookingRepository,
    IMessagePublisher publisher,
    ILogger<BookingCancelledEventHandler> logger)
    : INotificationHandler<DomainEventNotification<BookingCancelledEvent>>
{
    public async Task Handle(
        DomainEventNotification<BookingCancelledEvent> notification,
        CancellationToken ct)
    {
        var ev = notification.DomainEvent;

        logger.LogInformation(
            "Handling BookingCancelledEvent. BookingId: {BookingId}", ev.BookingId);

        var detail = await bookingRepository.GetDetailByIdAsync(ev.BookingId, ct);
        if (detail is null)
        {
            logger.LogWarning(
                "BookingCancelledEvent: booking not found. BookingId: {BookingId}", ev.BookingId);
            return;
        }

        await publisher.PublishAsync(new BookingCancelledMessage(
            ev.BookingId,
            ev.PatientId,
            detail.PatientEmail,
            detail.PatientName,
            detail.DoctorEmail,
            detail.DoctorName,
            ev.Reason,
            ev.CancelledBy,
            detail.WorkDate,
            detail.SlotStart), ct);

        logger.LogInformation(
            "BookingCancelledMessage published. BookingId: {BookingId}", ev.BookingId);
    }
}