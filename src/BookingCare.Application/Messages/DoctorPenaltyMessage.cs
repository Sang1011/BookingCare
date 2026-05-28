namespace BookingCare.Application.Messages;

public record DoctorPenaltyMessage(
    Guid BookingId,
    Guid DoctorId,
    string DoctorEmail,
    string DoctorName,
    Guid PatientId,
    string PatientEmail,
    string PatientName,
    DateOnly WorkDate,
    TimeOnly SlotStart,
    DateTime CancelledAt
);