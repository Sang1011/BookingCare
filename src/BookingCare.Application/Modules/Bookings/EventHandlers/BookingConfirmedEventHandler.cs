using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Common.Models;
using BookingCare.Application.Messages;
using BookingCare.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingCare.Application.Modules.Bookings.EventHandlers;

public class BookingConfirmedEventHandler(
    IBookingRepository bookingRepository,
    IMessagePublisher publisher,
    ILogger<BookingConfirmedEventHandler> logger)
    : INotificationHandler<DomainEventNotification<BookingConfirmedEvent>>
{
    public async Task Handle(
        DomainEventNotification<BookingConfirmedEvent> notification,
        CancellationToken ct)
    {
        var ev = notification.DomainEvent;

        logger.LogInformation(
            "Handling BookingConfirmedEvent. BookingId: {BookingId}", ev.BookingId);

        var detail = await bookingRepository.GetDetailByIdAsync(ev.BookingId, ct);
        if (detail is null)
        {
            logger.LogWarning(
                "BookingConfirmedEvent: booking not found. BookingId: {BookingId}", ev.BookingId);
            return;
        }

        await publisher.PublishAsync(new BookingConfirmedMessage(
            ev.BookingId,
            ev.PatientId,
            detail.PatientEmail,
            detail.PatientName,
            detail.DoctorName,
            detail.WorkDate,
            detail.SlotStart,
            detail.SlotEnd), ct);

        logger.LogInformation(
            "BookingConfirmedMessage published. BookingId: {BookingId}", ev.BookingId);
    }
}