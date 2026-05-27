using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Common.Models;
using BookingCare.Domain.Events;
using MediatR;

namespace BookingCare.Application.Modules.Bookings.EventHandlers;

public class BookingCancelledEventHandler(
    IBookingRepository bookingRepository,
    IEmailService emailService
) : INotificationHandler<DomainEventNotification<BookingCancelledEvent>>
{
    public async Task Handle(
        DomainEventNotification<BookingCancelledEvent> notification,
        CancellationToken ct)
    {
        var ev = notification.Event;
        var detail = await bookingRepository.GetDetailByIdAsync(ev.BookingId, ct);
        if (detail is null) return;

        await emailService.SendBookingCancelledAsync(
            patientEmail: detail.PatientName,
            doctorName: detail.DoctorName,
            workDate: detail.WorkDate,
            slotStart: detail.SlotStart,
            reason: ev.Reason,
            ct: ct);
    }
}