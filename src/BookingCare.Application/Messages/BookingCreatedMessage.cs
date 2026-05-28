namespace BookingCare.Application.Messages;

public record BookingCreatedMessage(
    Guid BookingId,
    Guid PatientId,
    string PatientEmail,
    string PatientName,
    string DoctorName,
    DateOnly WorkDate,
    TimeOnly SlotStart,
    TimeOnly SlotEnd
);