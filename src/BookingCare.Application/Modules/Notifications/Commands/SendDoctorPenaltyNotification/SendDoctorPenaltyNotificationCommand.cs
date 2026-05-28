using BookingCare.Domain.Common;
using MediatR;

namespace BookingCare.Application.Modules.Notifications.Commands.SendDoctorPenaltyNotification
{
    public record SendDoctorPenaltyNotificationCommand(
        Guid BookingId,
        Guid DoctorId,
        Guid PatientId,
        string DoctorName,
        string DoctorEmail,
        string PatientName,
        string PatientEmail,
        DateOnly WorkDate,
        TimeOnly SlotStart,
        DateTime CancelledAt
    ) : IRequest<Result>;
}