using BookingCare.Application.Common.Interfaces.Persistence;
using BookingCare.Application.Common.Interfaces.Services;
using BookingCare.Application.Common.Models;
using BookingCare.Application.Messages;
using BookingCare.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingCare.Application.Modules.Doctors.EventHandlers;

public class DoctorScheduleCreatedEventHandler(
    IUserRepository userRepository,
    IMessagePublisher publisher,
    ILogger<DoctorScheduleCreatedEventHandler> logger)
    : INotificationHandler<DomainEventNotification<DoctorScheduleCreatedEvent>>
{
    public async Task Handle(
        DomainEventNotification<DoctorScheduleCreatedEvent> notification,
        CancellationToken ct)
    {
        var ev = notification.DomainEvent;

        logger.LogInformation(
            "Handling DoctorScheduleCreatedEvent. DoctorId: {DoctorId}", ev.DoctorId);

        var doctorEmail = await userRepository.GetEmailByDoctorIdAsync(ev.DoctorId, ct);
        if (doctorEmail is null)
        {
            logger.LogWarning(
                "DoctorScheduleCreatedEvent: doctor email not found. DoctorId: {DoctorId}", ev.DoctorId);
            return;
        }

        await publisher.PublishAsync(new DoctorScheduleCreatedMessage(
            ev.DoctorId,
            doctorEmail,
            ev.DoctorName,
            ev.WorkDate,
            ev.SlotStart,
            ev.SlotEnd), ct);

        logger.LogInformation(
            "DoctorScheduleCreatedMessage published. DoctorId: {DoctorId}", ev.DoctorId);
    }
}