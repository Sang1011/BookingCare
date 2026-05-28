namespace BookingCare.Application.Messages;

public record BookingAutoExpiredMessage(
    Guid BookingId,
    Guid PatientId,
    string PatientEmail,
    string PatientName,
    string DoctorName,
    DateOnly WorkDate,
    TimeOnly SlotStart
);