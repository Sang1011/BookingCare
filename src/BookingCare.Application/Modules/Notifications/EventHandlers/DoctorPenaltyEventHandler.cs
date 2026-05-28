using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Common.Models;
using BookingCare.Application.Messages;
using BookingCare.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingCare.Application.Modules.Notifications.EventHandlers;

public class DoctorPenaltyEventHandler(
    IBookingRepository bookingRepository,
    IMessagePublisher publisher,
    ILogger<DoctorPenaltyEventHandler> logger)
    : INotificationHandler<DomainEventNotification<DoctorPenaltyEvent>>
{
    public async Task Handle(
        DomainEventNotification<DoctorPenaltyEvent> notification,
        CancellationToken ct)
    {
        var ev = notification.DomainEvent;

        logger.LogInformation(
            "Handling DoctorPenaltyEvent. BookingId: {BookingId}, DoctorId: {DoctorId}",
            ev.BookingId, ev.DoctorId);

        var detail = await bookingRepository.GetDetailByIdAsync(ev.BookingId, ct);
        if (detail is null)
        {
            logger.LogWarning(
                "DoctorPenaltyEvent: booking not found. BookingId: {BookingId}", ev.BookingId);
            return;
        }

        await publisher.PublishAsync(new DoctorPenaltyMessage(
            ev.BookingId,
            ev.DoctorId,
            detail.DoctorEmail,
            detail.DoctorName,
            ev.PatientId,
            detail.PatientEmail,
            detail.PatientName,
            detail.WorkDate,
            detail.SlotStart,
            ev.CancelledAt), ct);

        logger.LogInformation(
            "DoctorPenaltyMessage published. BookingId: {BookingId}", ev.BookingId);
    }
}