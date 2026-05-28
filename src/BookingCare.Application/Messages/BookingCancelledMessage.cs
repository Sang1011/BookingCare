using BookingCare.Domain.Enums;

namespace BookingCare.Application.Messages;

public record BookingCancelledMessage(
    Guid BookingId,
    Guid PatientId,
    string PatientEmail,
    string PatientName,
    string DoctorEmail,
    string DoctorName,
    string Reason,
    CancelledBy CancelledBy,
    DateOnly WorkDate,
    TimeOnly SlotStart
);